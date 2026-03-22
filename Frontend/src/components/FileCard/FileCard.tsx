import { useLanguage } from "../../contexts/LanguageContext"
import { Button } from "../Button/Button"

import "./FileCard.css"


type FileItem = {
    id: string
    fileName: string
    contentType: string
    fileSizeBytes: number
    status: string
    createdAt: string
    downloadUrl: string
    sourceFileDownloadUrl?: string | null
}

type Props = {
    file: FileItem
    onDownload: (fileId: string, fileName: string) => void
}

function formatFileSize(bytes: number): string {
    if (bytes < 1024)
        return `${bytes} B`;

    if (bytes < 1024 * 1024)
        return `${Math.round(bytes / 1024)} KB`;

    if (bytes < 1024 * 1024 * 1024)
        return `${Math.round(bytes / (1024 * 1024)).toFixed(1)} MB`

    return `${(bytes / (1024 * 1024 * 1024)).toFixed(1)} GB`;
}

function formatFileDate(iso: string): string {
    return new Date(iso).toLocaleDateString(undefined, {
        year: "numeric",
        month: "short",
        day: "numeric",
        hour: "2-digit",
        minute: "2-digit",
    });
}

export function FileCard({ file, onDownload }: Props) {
    const { t } = useLanguage()

    const fileNameSplit = file.fileName.split(".")

    const fileExtensionUpper = fileNameSplit.length > 1 ?
        fileNameSplit.pop()?.toUpperCase() : ""

    return (
        <article className="file-card">
            <div className="file-header">
                <span className="file-extension">{fileExtensionUpper}</span>
                <h3 className="file-name">{file.fileName}</h3>
            </div>

            <div className="file-metadata">
                <span>{formatFileSize(file.fileSizeBytes)}</span>
                <span>{file.contentType}</span>
                <span>{formatFileDate(file.createdAt)}</span>
            </div>

            <div className="file-actions">
                <Button
                    type="button"
                    className="download-button"
                    onClick={() => onDownload(file.id, file.fileName)}
                >{t("download")}</Button>

                {file.sourceFileDownloadUrl && (
                    <Button
                        type="button"
                        className="download-button"
                        onClick={() => onDownload(
                            file.sourceFileDownloadUrl!.split("/").pop()!,
                            t("sourceFile")
                        )}
                    >t("sourceFile")</Button>
                )}
            </div>
        </article>
    )

}

export type { FileItem }