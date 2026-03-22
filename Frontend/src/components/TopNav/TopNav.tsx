import { useNavigate } from "react-router-dom"
import { useAuth } from "../../contexts/AuthContext"
import { useLanguage } from "../../contexts/LanguageContext"

import { LanguageSwitcher } from "../LanguageSwitcher"
import { Button } from "../Button"

import "./TopNav.css"


export function TopNav() {
    const { user, logout } = useAuth()
    const { t } = useLanguage()
    const navigate = useNavigate()

    const handleLogout = () => {
        logout()
        navigate("/login")
    }

    return (
        <header className="top-nav">
            <span>File Converter</span>

            <div className="nav-actions">
                <span className="welcome-text">
                    {t('welcome')}, {user?.username} :D
                </span>

                <LanguageSwitcher />

                <Button
                    onClick={handleLogout}
                    className="secondary-button"
                >{t("logout")}</Button>
            </div>
        </header>
    )

}