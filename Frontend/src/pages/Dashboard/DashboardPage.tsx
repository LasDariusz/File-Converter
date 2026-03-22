import { useEffect, useState } from "react"

import { useLanguage } from "../../contexts/LanguageContext"
import { useAuth } from "../../contexts/AuthContext"

import { ConversionForm, type ConversionPayload } from "../../components/ConversionForm"
import { FileCard, type FileItem } from "../../components/FileCard"

import "./DashboardPage.css"


const API_BASE = "http://localhost:5194/api"

export function DashboardPage() {
    const { t } = useLanguage()
    const { user } = useAuth()

    const [files, setFiles] = useState<FileItem[]>([])

    const [isLoadingFiles, setIsLoadingFiles] = useState(true);
    const [isConverting, setIsConverting] = useState(false);

    const token = localStorage.getItem("auth-token");

    const fetchFiles = async () => {
        if (!user?.id || !token) {
            setIsLoadingFiles(false)
            return
        }

        try {
            setIsLoadingFiles(true);

            const res = await fetch(`${API_BASE}/Users/${user.id}/files`, {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            })

            if (!res.ok) {
                throw new Error("fetch files failed")
            }

            const data: FileItem[] = await res.json()
            setFiles(data)
        } catch (err) {
            console.error(err)
        } finally {
            setIsLoadingFiles(false)
        }
    }

    useEffect(() => {
        fetchFiles()
    }, [user?.id])

    const handleFileDownload = async (fileId: string, fileName: string) => {
        if (!token) {
            return
        }

        try {
            const res = await fetch(`${API_BASE}/Files/${fileId}`, {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            })

            if (!res.ok) {
                throw new Error(`fetch file ${fileName} failed`)
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

    const handleConvert = async (payload: ConversionPayload) => {
        if (isConverting || !token) {
            return
        }

        setIsConverting(true)

        try {
            const formData = new FormData()
            formData.append("TargetFormat", payload.targetFormat)
            formData.append("FormFile", payload.file)

            const res = await fetch(`${API_BASE}/Files/convert`, {
                method: "POST",
                headers: {
                    Authorization: `Bearer ${token}`
                },
                body: formData
            })

            if (!res.ok) {
                throw new Error(await res.text())
            }

            const blob = await res.blob()
            const ext = payload.targetFormat
            const name = payload.file.name.replace(/\.[^.]+$/, "") + `_converted.${ext}`;

            const url = URL.createObjectURL(blob)
            const link = document.createElement("a")
            link.href = url
            link.download = name

            document.body.appendChild(link)
            link.click()
            link.remove()

            URL.revokeObjectURL(url)

            await fetchFiles()
        } catch (err) {
            console.error(err);
            alert(t("convertError"));
        } finally {
            setIsConverting(false);
        }
    }

    return (
        <section className="dashboard-grid">
            <ConversionForm
                onConvert={handleConvert}
                isConverting={isConverting}
            />

            <div className="files-panel">
                <h2>{t("yourFiles")}</h2>

                {isLoadingFiles ?
                    (
                        <p>{t("loadingFiles")}</p>
                    )
                    : files.length === 0 ?
                    (
                        <p>{t("noFiles")}</p>
                    )
                    :
                    (
                        <div className="file-list">
                            {files.map(f => (
                                <FileCard
                                    key={f.id}
                                    file={f}
                                    onDownload={handleFileDownload}
                                />
                            ))}
                        </div>
                    )
                }
            </div>
        </section>
    )

}