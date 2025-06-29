import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';

const FriendPage: React.FC = () => {
    const { id } = useParams();
    const [profile, setProfile] = useState<any>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const token = localStorage.getItem('token');
                const response = await axios.get(`https://licenta-backend-nf1m.onrender.com/api/User/ExtendedProfile/${id}`, {
                    headers: {
                        Authorization: `Bearer ${token}`
                    }
                });
                setProfile(response.data.response); // <- corect: accesăm .response
            } catch (error) {
                console.error('Failed to fetch profile', error);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, [id]);

    if (loading) return <div className="text-light">Loading...</div>;
    if (!profile) return <div className="text-danger">Failed to load profile.</div>;

    return (
        <div className="container mt-4 text-light">
            <div className="d-flex align-items-center gap-3">
                {profile.profilePictureUrl ? (
                    <img src={profile.profilePictureUrl} alt="Profile" className="rounded-circle" width={80} height={80} />
                ) : (
                    <div className="rounded-circle bg-secondary" style={{ width: 80, height: 80 }}></div>
                )}
                <div>
                    <h3>{profile.name}</h3>
                    <p>{profile.email}</p>
                    <p>Watched: {profile.watchedCount} | Recommended: {profile.recommendedCount}</p>
                </div>
            </div>

            <hr className="bg-light" />

            <div className="row">
                <div className="col-md-4">
                    <h5>🎬 Watched Movies</h5>
                    <div style={{ maxHeight: 200, overflowY: 'auto' }}>
                        {profile.watchedMovies.map((m: any) => (
                            <div key={m.id} className="mb-2">
                                <img src={m.posterUrl} alt={m.title} width={50} /> {m.title}
                            </div>
                        ))}
                    </div>
                </div>

                <div className="col-md-4">
                    <h5>⭐ Recommended Movies</h5>
                    <div style={{ maxHeight: 200, overflowY: 'auto' }}>
                        {profile.recommendedMovies.map((m: any) => (
                            <div key={m.id} className="mb-2">
                                <img src={m.posterUrl} alt={m.title} width={50} /> {m.title}
                            </div>
                        ))}
                    </div>
                </div>

                <div className="col-md-4">
                    <h5>📅 Events</h5>
                    <div style={{ maxHeight: 200, overflowY: 'auto' }}>
                        {profile.organizedEvents.map((e: any) => (
                            <div key={e.id} className="mb-2">
                                {e.title} - {new Date(e.date).toLocaleDateString()}
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default FriendPage;
