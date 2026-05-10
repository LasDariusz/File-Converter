import { createContext, useMemo, type ReactNode, useState, useContext } from "react"
import { useNavigate } from "react-router-dom"
import { API_BASE, AUTH_LOGIN_ENDPOINT, AUTH_REGISTER_ENDPOINT } from "../utils/constants"
import { apiFetch } from "../utils/apiClient"

type AuthUserData = {
    userId: string,
    email: string,
    username: string
}

type AuthContextVal = {
    user: AuthUserData | null,
    login: (email: string, password: string) => Promise<void>,
    register: (username: string, email: string, password: string) => Promise<void>,
    refresh: () => Promise<void>,
    logout: () => void,
    handleUnauthorized: () => void
};

const AuthContext = createContext<AuthContextVal | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const navigate = useNavigate()

    const [userData, setUserData] = useState<AuthUserData | null>(() => {
        const storedCreds = localStorage.getItem("converter-user")
        if (!storedCreds) return null
        return JSON.parse(storedCreds) as AuthUserData
    });

    const login = async (email: string, password: string) => {
        try {
            const res = await apiFetch(`${API_BASE}${AUTH_LOGIN_ENDPOINT}`, handleUnauthorized, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({email, password})
            })

            if (!res.ok) {
                const resTxt = await res.text();
                throw new Error(resTxt);
            }
                
            const loginData = await res.json();

            const loggedInUser: AuthUserData = {
                userId: loginData.userData.userId,
                email: loginData.userData.email,
                username: loginData.userData.username
            };

            setUserData(loggedInUser);
            localStorage.setItem("converter-user", JSON.stringify(loggedInUser));

            if (loginData.accessToken) {
                localStorage.setItem("auth-token", loginData.accessToken);
            }

        } catch (err) {
            console.log("Login error:", err)
            throw err;
        }
    }

    const register = async (username: string, email: string, password: string) => {
        try {
            const res = await apiFetch(`${API_BASE}${AUTH_REGISTER_ENDPOINT}`, handleUnauthorized, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({email, username, password})
            });

            if (!res.ok) {
                const resTxt = await res.text();
                throw new Error(resTxt);
            }

            const loginData = await res.json();

            const loggedInUser: AuthUserData = {
                userId: loginData.userId,
                username: loginData.username,
                email: email
            }

            setUserData(loggedInUser);
            localStorage.setItem("converter-user", JSON.stringify(loggedInUser));

            if (loginData.accessToken) {
                localStorage.setItem("auth-token", loginData.accessToken);
            }
        } catch (err) {
            console.log("Login error:", err)
            throw err;
        }
    };

    const refresh = async () => { };

    const logout = () => {
        setUserData(null);
        localStorage.removeItem("converter-user");
        localStorage.removeItem("auth-token");
    };

    const handleUnauthorized = () => {
        setUserData(null)
        localStorage.removeItem("converter-user")
        localStorage.removeItem("auth-token")
        navigate("/login")
    }

    const value = useMemo(
        () => ({ user: userData, login, register, refresh, logout, handleUnauthorized }),
        [userData]
    );

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
};

export function useAuth() {
    const context = useContext(AuthContext);
    if (!context) throw new Error();
    return context;
};