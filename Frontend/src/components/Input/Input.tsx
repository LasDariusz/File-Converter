import "./Input.css"


type Props = {
    label: string
    type: "text" | "email" | "password"
    value: string
    onChange: (value: string) => void
    required?: boolean
}

export function Input({ label, type, value, onChange, required = true }: Props) {

    return (
        <label>
            {label}
            <input
                type={type}
                value={value}
                onChange={(e) => onChange(e.target.value)}
                required={required}
            />
        </label>
    )

}