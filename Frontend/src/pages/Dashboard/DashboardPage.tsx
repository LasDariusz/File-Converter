import { useEffect, useState } from "react"
import { useLanguage } from "../../contexts/LanguageContext"
import { useAuth } from "../../contexts/AuthContext"
import { ConversionForm, type ConversionPayload } from "../../components/ConversionForm"
import { FileCard, type FileItem } from "../../components/FileCard"
import { apiFetch } from "../../utils/apiClient"
import { API_BASE, FILES_METADATA_ENDPOINT } from "../../utils/constants"
import "./DashboardPage.css"

export function DashboardPage() {
    const { t } = useLanguage()
    const { user } = useAuth()

    const [fileMetadata, setFileMetadata] = useState<FileItem[]>([])

    //const [isLoadingUserProfile, setIsLoadingUserProfile] = useState<boolean>(false);
    const [isLoadingUserFiles, setIsLoadingUserFiles] = useState<boolean>(true);
    const [isConvertingFile, setIsConvertingFile] = useState<boolean>(false);

    //const [loadingUserProfileError, setLoadingUserProfileError] = useState<string | null>(null)
    const [loadingUserFilesError, setLoadingUserFilesError] = useState<string | null>(null)
    const [fileConvertError, setFileConvertError] = useState<string | null>(null)

    const token = localStorage.getItem("auth-token")

    const fetchUserFiles = async () => {
        if (!user?.id || !token) {
            setIsLoadingUserFiles(false)
            return
        }

        try {
            const res = await apiFetch(`${API_BASE}${FILES_METADATA_ENDPOINT}`);

            if (!res.ok) {
                setLoadingUserFilesError(t("unkownServerError"))
                return;
            }

            const data = await res.json()
            setFileMetadata(data.FilesMetadata)
        } catch (err) {
            console.error(err)
        } finally {
            setIsLoadingUserFiles(false)
        }
    }

    useEffect(() => {
        fetchUserFiles()
    }, [user?.id])

    const handleFileDownload = async (fileId: string, fileName: string) => {
        if (!token) {
            return
        }

        try {
            const res = await apiFetch(`${API_BASE}/Files/${fileId}`)

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
        if (isConvertingFile || !token) {
            return
        }

        setIsConvertingFile(true)
        try {
            const formData = new FormData()
            formData.append("FormFile", payload.file)
            formData.append("TargetExtension", payload.targetExtension)

            const res = await apiFetch(`${API_BASE}/Files/convert`, {
                method: "POST",
                body: formData
            })

            if (!res.ok) {
                setFileConvertError("Convertion failed")
            }

            const blob = await res.blob()
            const ext = payload.targetExtension
            const name = payload.file.name.replace(/\.[^.]+$/, "") + `_converted.${ext}`;

            const url = URL.createObjectURL(blob)
            const link = document.createElement("a")
            link.href = url
            link.download = name

            document.body.appendChild(link)
            link.click()
            link.remove()

            URL.revokeObjectURL(url)

            await fetchUserFiles()
        } catch (err) {
            console.error(err);
            alert(t("convertError"));
        } finally {
            setIsConvertingFile(false);
        }
    }

    return (
        <section className="dashboard-grid">
            <ConversionForm
                onConvert={handleFileConvert}
                isConverting={isConvertingFile}
            />

            {fileConvertError && (
                <p>{fileConvertError}</p>
            )}

            <div className="files-panel">
                <h2>{t("loadingFiles")}</h2>
                 
                {isLoadingUserFiles ? (
                    <p>{t("loadingFiles")}</p>
                ) : loadingUserFilesError ? (
                    <p>{loadingUserFilesError}</p>
                ) : fileMetadata.length === 0 ? (
                    <p>{t("noFiles")}</p>
                ) : (
                    <div className="file-list">
                        {fileMetadata.map(f => (
                            <FileCard
                                key={f.id}
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