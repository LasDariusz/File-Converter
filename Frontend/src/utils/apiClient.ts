export class UnauthorizedError extends Error { }

export const apiFetch = async (
    url: string,
    onUnauthorized: () => void,
    options?: RequestInit) => {
    const token = localStorage.getItem("auth-token")

    const res = await fetch(url, {
        ...options,
        headers: {
            ...options?.headers,
            Authorization: `Bearer ${token}`
        }
    })

    if (res.status === 401) {
        onUnauthorized()
    }

    return res
}