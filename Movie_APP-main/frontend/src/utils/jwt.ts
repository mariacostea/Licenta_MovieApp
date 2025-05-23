export function getUserIdFromToken(): string | null {
    const token = localStorage.getItem("token");
    if (!token) return null;

    try {
        const payloadBase64 = token.split('.')[1];
        const payload = JSON.parse(atob(payloadBase64));
        const raw = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
        return raw ? String(raw).toLowerCase().trim() : null;
    } catch (err) {
        console.error("Failed to decode JWT", err);
        return null;
    }
}


export function getUserNameFromToken(): string | null {
    const token = localStorage.getItem("token");
    if (!token) return null;

    try {
        const payloadBase64 = token.split('.')[1];
        const payload = JSON.parse(atob(payloadBase64));
        return payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ?? null;
    } catch (err) {
        console.error("Failed to decode JWT", err);
        return null;
    }
}
