import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";

const FriendPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
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
                const res = await fetch(
                    `https://licenta-backend-nf1m.onrender.com/api/User/GetExtendedProfile/ExtendedProfile/${id}`,
                    {
                        headers: { Authorization: `Bearer ${token}` },
                    }
                );
                const json = await res.json();
                setData(json.response);
            } catch (err) {
                console.error("Fetch error:", err);
                setError("Network or fetch error");
            } finally {
                setLoading(false);
            }
        };

        fetchProfile();
    }, [id]);

    if (loading) return <div className="text-light">Loading...</div>;
    if (error || !data) return <div className="text-danger">Failed to load profile.</div>;

    return (
        <div className="container mt-4 text-light" style={{ minHeight: "100vh" }}>
            <div className="d-flex align-items-center gap-3 mb-4">
                {data.profilePictureUrl ? (
                    <img
                        src={data.profilePictureUrl}
                        alt="Profile"
                        style={{
                            width: 80,
                            height: 80,
                            borderRadius: "50%",
                            objectFit: "cover",
                        }}
                    />
                ) : (
                    <div
                        className="bg-secondary"
                        style={{
                            width: 80,
                            height: 80,
                            borderRadius: "50%",
                        }}
                    />
                )}
                <div>
                    <h3>{data.name}</h3>
                    <p>{data.email}</p>
                    <p>
                        Watched: {data.watchedCount} | Recommended: {data.recommendedCount}
                    </p>
                </div>
            </div>

            <hr className="bg-light" />

            <div className="row">
                <div className="col-md-4">
                    <h5>🎬 Watched Movies</h5>
                    <div style={{ maxHeight: "60vh", overflowY: "auto" }}>
                        {data.watchedMovies.map((m: any) => (
                            <div key={m.id} className="mb-2 d-flex align-items-center">
                                <img
                                    src={m.posterUrl}
                                    alt={m.title}
                                    width={50}
                                    height={75}
                                    className="me-2 rounded"
                                    style={{ objectFit: "cover", cursor: "pointer" }}
                                    onClick={() => navigate(`/movies/${m.id}`)}
                                />
                                <span>{m.title}</span>
                            </div>
                        ))}
                    </div>
                </div>

                <div className="col-md-4">
                    <h5>⭐ Recommended Movies</h5>
                    <div style={{ maxHeight: "60vh", overflowY: "auto" }}>
                        {data.recommendedMovies.map((m: any) => (
                            <div key={m.id} className="mb-2 d-flex align-items-center">
                                <img
                                    src={m.posterUrl}
                                    alt={m.title}
                                    width={50}
                                    height={75}
                                    className="me-2 rounded"
                                    style={{ objectFit: "cover", cursor: "pointer" }}
                                    onClick={() => navigate(`/movies/${m.id}`)}
                                />
                                <span>{m.title}</span>
                            </div>
                        ))}
                    </div>
                </div>

                <div className="col-md-4">
                    <h5>📅 Organized Events</h5>
                    <div style={{ maxHeight: "60vh", overflowY: "auto" }}>
                        {data.organizedEvents.map((e: any) => (
                            <div key={e.id} className="mb-3 d-flex">
                                {e.moviePosterUrl && (
                                    <img
                                        src={e.moviePosterUrl}
                                        alt={e.title}
                                        width={50}
                                        height={75}
                                        className="me-3 rounded"
                                        style={{ objectFit: "cover" }}
                                    />
                                )}
                                <div>
                                    <div className="fw-bold">{e.title}</div>
                                    <div>📍 {e.location || "Unknown location"}</div>
                                    <div>📆 {new Date(e.date).toLocaleString()}</div>
                                    <div>
                                        👥 {e.maxParticipants - e.freeSeats} / {e.maxParticipants} participants
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default FriendPage;
