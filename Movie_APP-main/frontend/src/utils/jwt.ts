export function getUserIdFromToken(): string | null {
    const token = localStorage.getItem("token");
    if (!token) return null;

    try {
        const payloadBase64 = token.split('.')[1];
        const payload = JSON.parse(atob(payloadBase64));
        return payload["nameid"] || null;
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
        return payload["unique_name"] || null;
    } catch (err) {
        console.error("Failed to decode JWT", err);
        return null;
    }
}
