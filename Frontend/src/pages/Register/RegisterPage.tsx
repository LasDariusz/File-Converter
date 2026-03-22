import { useState, type FormEvent } from "react"
import { Link, Navigate, useNavigate } from "react-router-dom"
import { useAuth } from "../../contexts/AuthContext"
import { useLanguage } from "../../contexts/LanguageContext"

import { Button } from "../../components/Button"
import { Input } from "../../components/Input"

import "./RegisterPage.css"


export function RegisterPage() {
    const { t } = useLanguage();
    const { user, register } = useAuth();
    const navigate = useNavigate();

    const [name, setName] = useState("");
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("");

    if (user) return <Navigate to="/dashboard" replace />
    
    const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        register(name, email, password);
        navigate("/dashboard");
    }

    return (
        <form className="auth-form" onSubmit={handleSubmit}>
            <h1>{t("registerTitle")}</h1>

            <Input
                label={t("username")}
                type="text"
                value={name}
                onChange={setName}
            />

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
            >{t("registerButton")}</Button>

            <p>
                {t("hasAccount")}
                <Link to="/login">
                    {t("goLogin")}
                </Link>
            </p>
        </form>
    )

}