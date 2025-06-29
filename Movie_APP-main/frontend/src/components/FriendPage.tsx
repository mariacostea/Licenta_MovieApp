import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

const FriendPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [data, setData] = useState<any>(null);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchProfile = async () => {
            const token = localStorage.getItem("token");
            if (!token) {
                setError("Token missing");
                setLoading(false);
                return;
            }

            try {
                const res = await fetch(`https://licenta-backend-nf1m.onrender.com/api/User/GetExtendedProfile/ExtendedProfile/${id}`, {
                    headers: {
                        Authorization: `Bearer ${token}`
                    }
                });

                const json = await res.json();
                setData(json);
            } catch (err) {
                console.error("Fetch error:", err);
                setError("Network or fetch error");
            } finally {
                setLoading(false);
            }
        };

        fetchProfile();
    }, [id]);

    return (
        <div className="container text-light py-4">
            <h3>Test FriendPage</h3>
            <p>User ID from URL: <strong>{id}</strong></p>

            {loading && <p>Loading...</p>}
            {error && <p className="text-danger">Error: {error}</p>}

            {data?.response?.profilePictureUrl && (
                <div
                    style={{
                        width: 100,
                        height: 100,
                        borderRadius: "50%",
                        overflow: "hidden",
                        border: "2px solid #ccc",
                        marginBottom: "1rem"
                    }}
                >
                    <img
                        src={data.response.profilePictureUrl}
                        alt="Profile"
                        style={{
                            width: "100%",
                            height: "100%",
                            objectFit: "cover"
                        }}
                    />
                </div>
            )}

            {data && (
                <pre style={{ backgroundColor: "#222", padding: "1em", borderRadius: "5px" }}>
                {JSON.stringify(data, null, 2)}
            </pre>
            )}
        </div>
    );

};

export default FriendPage;
