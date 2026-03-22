import { useState, type FormEvent } from "react"
import { useNavigate, Link, Navigate } from "react-router-dom"
import { useAuth } from "../../contexts/AuthContext"
import { useLanguage } from "../../contexts/LanguageContext"

import { Input } from "../../components/Input"
import { Button } from "../../components/Button"

import "./LoginPage.css"


export function LoginPage() {
    const { t } = useLanguage();
    const { user, login } = useAuth();
    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    if (user) return (<Navigate to="/dashboard" replace />);
    
    const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        try {
            await login(email, password);
        } catch (error: any) {
            window.alert(error.message)
            return;
        }
        navigate("/dashboard");
    }

    return (
        <form className="auth-form" onSubmit={handleSubmit}>
            <h1>{t("loginTitle")}</h1>

            <Input
                label={t("email")}
                type="email"
                value={email}
                onChange={setEmail}
            />

            <Input
                label={t("password")}
                type="password"
                value={password}
                onChange={setPassword}
            />

            <Button
                type="submit"
                className="primary-button"
            >{t("loginButton")}</Button>

            <p>
                {t("noAccount")}
                <Link to="/register">
                    {t("goRegister")}
                </Link>
            </p>
        </form>
    )

}