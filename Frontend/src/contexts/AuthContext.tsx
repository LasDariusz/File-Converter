import { createContext, useMemo, type ReactNode, useState, useContext } from "react";


const AUTH_LOGIN_URL: string = "http://localhost:5194/api/Auth/login";
const AUTH_REGISTER_URL = "http://localhost:5194/api/Auth/register";

type AuthUser = {
    id: string,
    username: string,
    email: string
};

type AuthContextVal = {
    user: AuthUser | null,
    login: (email: string, password: string) => Promise<void>,
    register: (username: string, email: string, password: string) => Promise<void>,
    refresh: () => Promise<void>,
    logout: () => void
};

const AuthContext = createContext<AuthContextVal | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<AuthUser | null>(() => {
        const storedCreds = localStorage.getItem("converter-user");
        if (!storedCreds) return null;
        return JSON.parse(storedCreds) as AuthUser;
    });

    const login = async (email: string, password: string) => {
        try {
            const res = await fetch(AUTH_LOGIN_URL, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({email, password})
            });

            if (!res.ok) {
                const resTxt = await res.text();
                throw new Error(resTxt);
            }
                
            const loginData = await res.json();

            const loggedInUser: AuthUser = {
                id: loginData.id,
                username: loginData.username,
                email: email
            };

            setUser(loggedInUser);
            localStorage.setItem("converter-user", JSON.stringify(loggedInUser));

            if (loginData.token) {
                localStorage.setItem("auth-token", loginData.token);
            }

        } catch (err) {
            console.log("Login error:", err)
            throw err;
        }
    }

    const register = async (username: string, email: string, password: string) => {
        try {
            const res = await fetch(AUTH_REGISTER_URL, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({email, username, password})
            });

            if (!res.ok) {
                const resTxt = await res.text();
                throw new Error(resTxt);
            }

            const loginData = await res.json();

            const loggedInUser: AuthUser = {
                id: loginData.id,
                username: loginData.username,
                email: email
            }

            setUser(loggedInUser);
            localStorage.setItem("converter-user", JSON.stringify(loggedInUser));

            if (loginData.token) {
                localStorage.setItem("auth-token", loginData.token);
            }
        } catch (err) {
            console.log("Login error:", err)
            throw err;
        }
    };

    const refresh = async () => { };

    const logout = () => {
        setUser(null);
        localStorage.removeItem("converter-user");
        localStorage.removeItem("auth-token");
    };

    const value = useMemo(
        () => ({ user, login, register, refresh, logout }),
        [user]
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