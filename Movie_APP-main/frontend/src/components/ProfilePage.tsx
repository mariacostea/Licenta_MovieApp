import React, { useEffect, useRef, useState } from "react";

const API = "https://licenta-backend-nf1m.onrender.com/api";

const ProfilePage: React.FC = () => {
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [profilePictureUrl, setProfilePictureUrl] = useState<string | null>(null);
    const [watchedCount, setWatchedCount] = useState(0);
    const [recommendedCount, setRecommendedCount] = useState(0);
    const fileInputRef = useRef<HTMLInputElement | null>(null);

    const userId = localStorage.getItem("userId");
    const token = localStorage.getItem("token");

    useEffect(() => {
        if (!userId || !token) return;

        const auth = { Authorization: `Bearer ${token}` };

        // Get user data
        fetch(`${API}/User/GetById/${userId}`, { headers: auth })
            .then(res => res.json())
            .then(data => {
                const user = data.response || data.result || {};
                setName(user.name);
                setEmail(user.email);
                setProfilePictureUrl(user.profilePictureUrl);
            });

        // Get movie stats
        fetch(`${API}/User/Count/${userId}`, { headers: auth })
            .then(res => res.json())
            .then(data => {
                setWatchedCount(data.watched || 0);
                setRecommendedCount(data.recommended || 0);
            });
    }, [userId, token]);

    const handleUploadClick = () => {
        fileInputRef.current?.click();
    };

    const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
        if (!e.target.files || e.target.files.length === 0) return;
        const file = e.target.files[0];

        const formData = new FormData();
        formData.append("file", file);

        if (!token) return;

        const res = await fetch(`${API}/User/UploadProfilePicture/upload-profile-picture`, {
            method: "POST",
            headers: {
                Authorization: `Bearer ${token}`
            },
            body: formData
        });

        const result = await res.json();

        if (res.ok && result.url) {
            setProfilePictureUrl(result.url);
        } else {
            console.error("Upload failed:", result);
            alert("Image upload failed.");
        }
    };

    return (
        <div style={{ minHeight: "100vh", background: "#121212", color: "white", padding: "2rem" }}>
            <div style={{
                maxWidth: "600px",
                margin: "auto",
                background: "#1e1e1e",
                padding: "2rem",
                borderRadius: "1rem",
                textAlign: "center"
            }}>
                <h2 style={{ marginBottom: "2rem" }}>👤 Profile</h2>

                <div style={{ marginBottom: "1rem" }}>
                    {profilePictureUrl ? (
                        <img
                            src={profilePictureUrl}
                            alt="Profile"
                            style={{
                                width: 150,
                                height: 150,
                                borderRadius: "50%",
                                objectFit: "cover",
                                marginBottom: "1rem"
                            }}
                        />
                    ) : (
                        <p>No profile picture uploaded.</p>
                    )}
                    <button
                        onClick={handleUploadClick}
                        style={{
                            backgroundColor: "#333",
                            color: "white",
                            padding: "0.5rem 1rem",
                            border: "1px solid white",
                            borderRadius: "0.5rem",
                            cursor: "pointer"
                        }}
                    >
                        Upload New Picture
                    </button>
                    <input
                        type="file"
                        accept="image/*"
                        ref={fileInputRef}
                        onChange={handleFileChange}
                        style={{ display: "none" }}
                    />
                </div>

                <p><strong>Name:</strong> {name}</p>
                <p><strong>Email:</strong> {email}</p>
                <p><strong>Watched Movies:</strong> {watchedCount}</p>
                <p><strong>Recommended Movies:</strong> {recommendedCount}</p>
            </div>
        </div>
    );
};

export default ProfilePage;
