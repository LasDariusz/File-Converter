import { type TranslationMap } from "../contexts/LanguageContext"

export const translations: TranslationMap = {
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

        // Errors
        downloadError: "Download failed.",
        unkownServerError: "Unknown server error has occured"
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

        // Errors
        downloadError: "Download failed.",
        unkownServerError: "Wystąpił nieprzewidziany błąd serwera"
    }
}