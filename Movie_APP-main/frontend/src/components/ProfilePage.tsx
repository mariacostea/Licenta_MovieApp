import React, { useEffect, useState } from "react";

interface User {
    id: string;
    name: string;
    email: string;
    profilePictureUrl?: string;
}

const API = "https://licenta-backend-nf1m.onrender.com/api";

export default function ProfilePage() {
    const token = localStorage.getItem("token");
    const userId = localStorage.getItem("userId");

    const [user, setUser] = useState<User | null>(null);
    const [counts, setCounts] = useState<{ watched: number; recommended: number }>({ watched: 0, recommended: 0 });
    const [uploading, setUploading] = useState(false);

    useEffect(() => {
        if (!userId || !token) return;
        const auth = { Authorization: `Bearer ${token}` };

        fetch(`${API}/User/GetById/${userId}`, { headers: auth })
            .then((res) => res.json())
            .then((data) => setUser(data.response ?? data.result));

        fetch(`${API}/User/Count/${userId}`, { headers: auth })
            .then((res) => res.json())
            .then((data) => setCounts(data));
    }, [userId, token]);

    const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file || !token || !userId) return;

        const formData = new FormData();
        formData.append("file", file);
        setUploading(true);

        try {
            const res = await fetch(`${API}/User/UploadProfilePicture/upload-profile-picture`, {
                method: "POST",
                body: formData,
            });
            const data = await res.json();
            if (res.ok && user) {
                setUser({ ...user, profilePictureUrl: data.url });
            }
        } catch (err) {
            console.error("Upload failed", err);
        } finally {
            setUploading(false);
        }
    };

    return (
        <div style={{ minHeight: "100vh", backgroundColor: "#111", color: "white", padding: "2rem" }}>
            <h2 className="mb-4">
                <i className="bi bi-person-circle me-2" /> Profile
            </h2>

            {user && (
                <div>
                    <div>
                        {user.profilePictureUrl ? (
                            <img
                                src={user.profilePictureUrl}
                                alt="Profile"
                                style={{ width: 100, height: 100, borderRadius: "50%" }}
                            />
                        ) : (
                            <div style={{ width: 100, height: 100, backgroundColor: "#444", borderRadius: "50%" }}></div>
                        )}
                    </div>

                    <div className="my-2">
                        <label className="btn btn-outline-light btn-sm">
                            {uploading ? "Uploading..." : "Upload New Picture"}
                            <input type="file" accept="image/*" hidden onChange={handleFileChange} />
                        </label>
                    </div>

                    <p><strong>Name:</strong> {user.name}</p>
                    <p><strong>Email:</strong> {user.email}</p>
                    <p><strong>Watched Movies:</strong> {counts.watched}</p>
                    <p><strong>Recommended Movies:</strong> {counts.recommended}</p>
                </div>
            )}
        </div>
    );
}
