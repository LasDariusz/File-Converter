import { useLanguage } from "../../contexts/LanguageContext"

import { Button } from "../Button"

import "./LanguageSwitcher.css"


export function LanguageSwitcher() {
    const { language, setLanguage, t } = useLanguage()

    return (
        <div className="language-switcher">
            <span>{t("lang")}</span>

            <Button
                type="button"
                className={language === "en" ? "language-button-active" : "language-button"}
                onClick={() => setLanguage("en")}
            >EN</Button>

            <Button
                type="button"
                className={language === "pl" ? "language-button-active" : "language-button"}
                onClick={() => setLanguage("pl")}
            >PL</Button>
        </div>
    )

}