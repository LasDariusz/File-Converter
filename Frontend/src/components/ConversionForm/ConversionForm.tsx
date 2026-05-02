import { useMemo, useState, type ChangeEvent, type FormEvent } from "react"
import { useLanguage } from "../../contexts/LanguageContext"
import { Button } from "../Button"
import "./ConversionForm.css"


const IMAGE_FORMATS = ["jpeg", "jpg", "png", "webp", "ppm", "bmp"]

const CONVERSION_MAP: Record<string, string[]> = {}

IMAGE_FORMATS.forEach(format => {
    CONVERSION_MAP[format] = IMAGE_FORMATS.filter(f => f !== format)
})

type ConversionPayload = {
    file: File
    targetExtension: string
}

type Props = {
    onConvert: (payload: ConversionPayload) => void
    isConverting: boolean
}

export function ConversionForm({ onConvert, isConverting }: Props) {
    const { t } = useLanguage()

    const [fromFormat, setFromFormat] = useState<string>(IMAGE_FORMATS[0])
    const [toFormat, setToFormat] = useState<string>(IMAGE_FORMATS[1])

    const [selectedFile, setSelectedFile] = useState<File | null>(null)

    const handleFileUpload = (evt: ChangeEvent<HTMLInputElement>) => {
        const file = evt.target.files?.[0]

        if (!file) {
            setSelectedFile(null)
            return
        }

        const fileNameSplit = file.name.split(".")

        const fileExtLower = fileNameSplit.length > 1 ?
            fileNameSplit.pop()?.toLowerCase() : ""

        if (fileExtLower && IMAGE_FORMATS.includes(fileExtLower)) {
            setSelectedFile(file)
            setFromFormat(fileExtLower)
        } else {
            alert("NIEDOZWOLONY FORMAT")
            setSelectedFile(null)
            evt.target.value = ""
        }
    };

    const availableToFormats = useMemo(() => CONVERSION_MAP[fromFormat], [fromFormat]);

    const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault()
        if (!selectedFile) return
        onConvert({ file: selectedFile, targetExtension: toFormat })
        setSelectedFile(null)
        event.currentTarget.reset()
    }

    return (
        <form className="conversion-form" onSubmit={handleSubmit}>
            <h2>{t("convertNow")}</h2>

            {selectedFile && fromFormat && (
                <div>
                    <label>
                        {t("detectedFormat")}
                        <select value={fromFormat} className="format-readonly">
                            <option value={fromFormat}>{fromFormat.toUpperCase()}</option>
                        </select>
                    </label>

                    <label>
                        {t("toFormat")}
                        <select value={toFormat} onChange={evt => setToFormat(evt.target.value)}>
                            {availableToFormats.map(f => (
                                <option key={f} value={f}>{f.toUpperCase()}</option>
                            ))}
                        </select>
                    </label>
                </div>
            )}

            <label>
                {t("selectFile")}
                <input
                    type="file"
                    accept="image/*,.heic,.heif"
                    onChange={evt => handleFileUpload(evt)}
                />
            </label>

            <Button
                type="submit"
                className="primary-button"
                disabled={isConverting || !selectedFile}
            >{isConverting ? t("converting") : t("convertButton")}</Button>
        </form>
    )

}

export type { ConversionPayload }