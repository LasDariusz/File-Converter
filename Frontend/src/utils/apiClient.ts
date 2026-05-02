
const clearSession = () => {
    localStorage.removeItem("auth-token")
    window.location.href = "/login"
}

export const apiFetch = async (url: string, options?: RequestInit) => {
    const token = localStorage.getItem("auth-token")

    const res = await fetch(url, {
        ...options,
        headers: {
            ...options?.headers,
            Authorization: `Bearer ${token}`
        }
    });

    if (res.status === 401) {
        clearSession()
    }

    return res;
}