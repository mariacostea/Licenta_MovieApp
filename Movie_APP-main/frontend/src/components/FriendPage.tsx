import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';

const FriendPage: React.FC = () => {
    const { id } = useParams();
    const [profile, setProfile] = useState<any>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const token = localStorage.getItem('token');
                const res = await fetch(`https://licenta-backend-nf1m.onrender.com/api/User/ExtendedProfile/${id}`, {
                    headers: {
                        Authorization: `Bearer ${token}`
                    }
                });

                const json = await res.json();
                setProfile(json.response ?? json.result ?? null);
            } catch (error) {
                console.error('Failed to fetch profile', error);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, [id]);

    if (loading) return <div className="text-light p-4">Loading...</div>;
    if (!profile) return <div className="text-danger p-4">Failed to load profile.</div>;

    return (
        <div
            className="text-light d-flex flex-column"
            style={{ minHeight: "100vh", backgroundColor: "#111" }}
        >
            <div className="container mt-4 flex-grow-1 d-flex flex-column">
                {/* Header */}
                <div className="d-flex align-items-center gap-3 mb-3">
                    {profile.profilePictureUrl ? (
                        <div
                            style={{
                                width: 80,
                                height: 80,
                                borderRadius: "50%",
                                overflow: "hidden",
                                flexShrink: 0,
                            }}
                        >
                            <img
                                src={profile.profilePictureUrl}
                                alt="Profile"
                                style={{
                                    width: "100%",
                                    height: "auto",
                                    objectFit: "cover"
                                }}
                            />
                        </div>
                    ) : (
                        <div
                            className="rounded-circle bg-secondary"
                            style={{ width: 80, height: 80 }}
                        />
                    )}

                    <div>
                        <h3>{profile.name}</h3>
                        <p>{profile.email}</p>
                        <p>Watched: {profile.watchedCount} | Recommended: {profile.recommendedCount}</p>
                    </div>
                </div>

                <hr className="bg-light" />

                {/* Content Sections */}
                <div className="row flex-grow-1">
                    <div className="col-md-4 d-flex flex-column">
                        <h5>🎬 Watched Movies</h5>
                        <div className="flex-grow-1 overflow-auto">
                            {profile.watchedMovies.map((m: any) => (
                                <div key={m.id} className="mb-2">
                                    <img src={m.posterUrl} alt={m.title} width={50} /> {m.title}
                                </div>
                            ))}
                        </div>
                    </div>

                    <div className="col-md-4 d-flex flex-column">
                        <h5>⭐ Recommended Movies</h5>
                        <div className="flex-grow-1 overflow-auto">
                            {profile.recommendedMovies.map((m: any) => (
                                <div key={m.id} className="mb-2">
                                    <img src={m.posterUrl} alt={m.title} width={50} /> {m.title}
                                </div>
                            ))}
                        </div>
                    </div>

                    <div className="col-md-4 d-flex flex-column">
                        <h5>📅 Events</h5>
                        <div className="flex-grow-1 overflow-auto">
                            {profile.organizedEvents.map((e: any) => (
                                <div key={e.id} className="mb-2">
                                    {e.title} - {new Date(e.date).toLocaleDateString()}
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default FriendPage;
