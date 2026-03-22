import { createContext, useContext, useMemo, useState, type ReactNode } from "react";

type Language = "en" | "pl";
type TranslationMap = Record<Language, Record<string, string>>;

const translations: TranslationMap = {
    en: {
        appName: "Converter",
        authSubtitle: "Convert your files easily",

        loginTitle: "Login",
        registerTitle: "Register",
        email: "Email",
        username: "Username",
        password: "Password",
        loginButton: "Login",
        registerButton: "Create account",
        noAccount: "No account? ",
        hasAccount: "Already have an account? ",
        goLogin: "Login",
        goRegister: "Create account",
        loginFailed: "Login failed",
        registerFailed: "Registration failed",
        wrongLoginData: "Incorrect credentials",
        emailInUse: "Email already in use",

        navDashboard: "Dashboard",
        welcome: "Welcome",
        logout: "Logout",
        lang: "Language",

        convertNow: "Convert file",
        fromFormat: "From",
        toFormat: "To",
        selectFile: "Select file",
        convertButton: "Convert",
        converting: "Converting…",
        convertError: "Conversion failed. Please try again.",

        yourFiles: "Your files",
        noFiles: "No converted files yet.",
        loadingFiles: "Loading files…",
        download: "Download",
        sourceFile: "Source file",
        downloadError: "Download failed.",
    },

    pl: {
        appName: "Konwerter",
        authSubtitle: "Konwertuj swoje pliki w prosty sposób",

        // Auth
        loginTitle: "Logowanie",
        registerTitle: "Rejestracja",
        email: "Email",
        username: "Nazwa użytkownika",
        password: "Hasło",
        loginButton: "Zaloguj się",
        registerButton: "Utwórz konto",
        noAccount: "Nie masz konta? ",
        hasAccount: "Masz już konto? ",
        goLogin: "Zaloguj się",
        goRegister: "Utwórz konto",
        loginFailed: "Logowanie nie powiodło się",
        registerFailed: "Rejestracja nie powiodła się",
        wrongLoginData: "Błędne dane logowania",
        emailInUse: "Email jest już w użyciu",

        // Dashboard / nav
        navDashboard: "Panel",
        welcome: "Witaj",
        logout: "Wyloguj",
        lang: "Język",

        convertNow: "Konwertuj plik",
        fromFormat: "Z formatu",
        toFormat: "Na format",
        selectFile: "Wybierz plik",
        convertButton: "Konwertuj",
        converting: "Konwertuję…",
        convertError: "Konwersja nie powiodła się. Spróbuj ponownie.",

        yourFiles: "Twoje pliki",
        noFiles: "Brak przekonwertowanych plików.",
        loadingFiles: "Ładowanie plików…",
        download: "Pobierz",
        sourceFile: "Plik źródłowy",
        downloadError: "Pobieranie nie powiodło się.",
    },
};

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