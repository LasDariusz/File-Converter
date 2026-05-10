import { useEffect, useState } from "react"
import { useLanguage } from "../../contexts/LanguageContext"
import { useAuth } from "../../contexts/AuthContext"
import { ConversionForm, type ConversionPayload } from "../../components/ConversionForm"
import { FileCard, type FileItem } from "../../components/FileCard"
import { apiFetch } from "../../utils/apiClient"
import { API_BASE, FILES_METADATA, FILES_CONVERT } from "../../utils/constants"
import styles from "./DashboardPage.module.css"

export function DashboardPage() {
    const { t } = useLanguage()
    const { user, handleUnauthorized } = useAuth()

    const [fileMetadata, setFileMetadata] = useState<FileItem[]>([])

    //const [isLoadingUserProfile, setIsLoadingUserProfile] = useState<boolean>(false);
    const [isLoadingUserFiles, setIsLoadingUserFiles] = useState<boolean>(true);
    const [isConvertingFile, setIsConvertingFile] = useState<boolean>(false);

    //const [loadingUserProfileError, setLoadingUserProfileError] = useState<string | null>(null)
    const [loadingUserFilesError, setLoadingUserFilesError] = useState<string | null>(null)
    const [fileConvertError, setFileConvertError] = useState<string | null>(null)

    const fetchUserFiles = async () => {
        setIsLoadingUserFiles(true)
        try {
            const res = await apiFetch(`${API_BASE}${FILES_METADATA}`, handleUnauthorized)
            if (!res.ok) {
                setLoadingUserFilesError(t("unkownServerError"))
                return
            }
            const data = await res.json()
            setFileMetadata(data.filesMetadata)
        } catch (err) {
            console.error(err)
        } finally {
            setIsLoadingUserFiles(false)
        }
    }

    useEffect(() => {
        fetchUserFiles()
    }, [user?.userId])

    const handleFileDownload = async (fileUrl: string, fileName: string) => {
        console.log(fileUrl)
        try {
            const res = await apiFetch(fileUrl, handleUnauthorized)
            if (res.status >= 500) {
                setFileConvertError(t(""))
                return;
            }
            const blob = await res.blob()
            const url = URL.createObjectURL(blob)

            const link = document.createElement("a")
            link.href = url
            link.download = fileName

            document.body.appendChild(link)
            link.click()
            link.remove()
            URL.revokeObjectURL(url)
        } catch (err) {
            console.error(err)
            alert(t("downloadError"))
        }
    }

    const handleFileConvert = async (payload: ConversionPayload) => {
        setIsConvertingFile(true)
        try {
            const formData = new FormData()
            formData.append("FormFile", payload.file)
            formData.append("TargetExtension", payload.targetExtension)

            const res = await apiFetch(`${API_BASE}${FILES_CONVERT}`, handleUnauthorized, {
                method: "POST",
                body: formData
            })

            if (!res.ok) {
                setFileConvertError("Convertion failed")
                return
            }

            const conversionData = await res.json()
            const fileName = conversionData.convertedFile.fileName
            const fileUrl = conversionData.convertedFile.downloadUrl

            const fileRes = await apiFetch(fileUrl, handleUnauthorized)

            if (!fileRes.ok) {
                setFileConvertError("Conversion failed")
                return
            }

            const blob = await fileRes.blob()
            const url = URL.createObjectURL(blob)
            const link = document.createElement("a")
            link.href = url
            link.download = fileName
            document.body.appendChild(link)
            link.click()
            link.remove()
            URL.revokeObjectURL(url)
        } catch (err) {
            console.error(err);
            alert(t("convertError"));
        } finally {
            setIsConvertingFile(false);
        }
    }

    return (
        <section className={styles.dashboardGrid}>
            <ConversionForm
                onConvert={handleFileConvert}
                isConverting={isConvertingFile}
            />

            {fileConvertError && (
                <p>{fileConvertError}</p>
            )}

            <div>
                <h2>{t("loadingFiles")}</h2>
                 
                {isLoadingUserFiles ? (
                    <p>{t("loadingFiles")}</p>
                ) : loadingUserFilesError ? (
                    <p>{loadingUserFilesError}</p>
                ) : fileMetadata.length === 0 ? (
                    <p>{t("noFiles")}</p>
                ) : (
                    <div className={styles.fileList}>
                        {fileMetadata.map(f => (
                            <FileCard
                                key={f.fileId}
                                file={f}
                                onDownload={handleFileDownload}
                            />
                        ))}
                    </div>
                )}
            </div>
        </section>
    )
}