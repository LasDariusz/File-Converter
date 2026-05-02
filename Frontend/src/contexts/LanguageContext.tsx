import { createContext, useContext, useMemo, useState, type ReactNode } from "react";
import { translations } from "../translations/translations";
export type Language = "en" | "pl";
export type TranslationMap = Record<Language, Record<string, string>>;

type LanguageContextVal = {
    language: Language;
    setLanguage: (l: Language) => void;
    t: (key: string) => string;
};

const LanguageContext = createContext<LanguageContextVal | undefined>(undefined);

export function LanguageProvider({ children }: { children: ReactNode }) {
    const [language, setLanguageState] = useState<Language>(() => {
        const stored = localStorage.getItem("language");
        return stored === "pl" ? "pl" : "en";
    });

    const setLanguage = (l: Language) => {
        setLanguageState(l);
        localStorage.setItem("language", l);
    };

    const value = useMemo(
        () => ({ language, setLanguage, t: (key: string) => translations[language][key] ?? key }),
        [language]
    );

    return (
        <LanguageContext.Provider value={value}>
            {children}
        </LanguageContext.Provider>
    );
}

export function useLanguage() {
    const ctx = useContext(LanguageContext);
    if (!ctx) throw new Error("useLanguage must be used inside LanguageProvider");
    return ctx;
}