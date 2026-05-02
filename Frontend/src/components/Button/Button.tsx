import "./Button.css"

type Props = {
    type?: "button" | "submit"
    className?: "button" | "primary-button" | "secondary-button" |
                "language-button" | "language-button-active" | "download-button"
    onClick?: () => void
    disabled?: boolean
    children: React.ReactNode
}

export function Button({type = "button", className, onClick, disabled, children}: Props) {
    return (
        <button
            type={type}
            className={className}
            onClick={onClick}
            disabled={disabled}
        >
            {children}
        </button>
    )
}