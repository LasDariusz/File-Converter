import { Link, Outlet } from "react-router-dom"
import { LanguageSwitcher } from "../../components/LanguageSwitcher"
import { useLanguage } from "../../contexts/LanguageContext"
import "./AuthLayout.css"


export function AuthLayout() {
    const { t } = useLanguage()

    return (
        <div className="auth-wrapper">
            <div className="auth-layout">
                <div className="auth-top">
                    <Link
                        to="/login"
                        className="app-name">
                        <span>{t("appName")}</span>
                    </Link>

                    <LanguageSwitcher />
                </div>

                <Outlet />
            </div>
        </div>
    )

}