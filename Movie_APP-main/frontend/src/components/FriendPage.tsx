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
        <div className="text-light" style={{ minHeight: "100vh", backgroundColor: "#111" }}>
            <div className="container py-4" style={{ height: "100%" }}>
                {loading && <p>Loading...</p>}
                {error && <p className="text-danger">Error: {error}</p>}

                {data?.response && (
                    <>
                        <div className="d-flex align-items-center gap-3 mb-4">
                            <div
                                style={{
                                    width: 100,
                                    height: 100,
                                    borderRadius: "50%",
                                    overflow: "hidden",
                                    border: "2px solid #ccc"
                                }}
                            >
                                <img
                                    src={data.response.profilePictureUrl}
                                    alt="Profile"
                                    style={{ width: "100%", height: "100%", objectFit: "cover" }}
                                />
                            </div>
                            <div>
                                <h3 className="mb-0">{data.response.name}</h3>
                                <p className="mb-0">{data.response.email}</p>
                                <p className="mb-0">
                                    Watched: {data.response.watchedCount} | Recommended: {data.response.recommendedCount}
                                </p>
                            </div>
                        </div>

                        <hr className="bg-light" />

                        <div className="row" style={{ height: "calc(100vh - 250px)" }}>
                            <div className="col-md-4 d-flex flex-column">
                                <h5>🎬 Watched Movies</h5>
                                <div style={{ flex: 1, overflowY: "auto" }}>
                                    {data.response.watchedMovies.map((m: any) => (
                                        <div key={m.id} className="mb-2 d-flex align-items-start">
                                            <img src={m.posterUrl} alt={m.title} width={50} className="me-2" />
                                            <span>{m.title}</span>
                                        </div>
                                    ))}
                                </div>
                            </div>

                            <div className="col-md-4 d-flex flex-column">
                                <h5>⭐ Recommended Movies</h5>
                                <div style={{ flex: 1, overflowY: "auto" }}>
                                    {data.response.recommendedMovies.map((m: any) => (
                                        <div key={m.id} className="mb-2 d-flex align-items-start">
                                            <img src={m.posterUrl} alt={m.title} width={50} className="me-2" />
                                            <span>{m.title}</span>
                                        </div>
                                    ))}
                                </div>
                            </div>

                            <div className="col-md-4 d-flex flex-column">
                                <h5>📅 Events</h5>
                                <div style={{ flex: 1, overflowY: "auto" }}>
                                    {data.response.organizedEvents.map((e: any) => (
                                        <div key={e.id} className="mb-2">
                                            {e.title} – {new Date(e.date).toLocaleDateString()}
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </div>
                    </>
                )}
            </div>
        </div>
    );
};

export default FriendPage;
