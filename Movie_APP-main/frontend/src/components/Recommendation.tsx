import React, { useEffect, useState } from "react";
import MovieCard, { MovieCardProps } from "../components/MovieCard";

const Recommendation: React.FC = () => {
    const [type, setType] = useState("genre");
    const [movies, setMovies] = useState<MovieCardProps[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const fetchRecommendations = async () => {
        setLoading(true);
        setError("");
        const token = localStorage.getItem("token");
        const userId = localStorage.getItem("userId");

        if (!token || !userId) {
            setError("You must be logged in.");
            setLoading(false);
            return;
        }

        try {
            const res = await fetch(`https://licenta-backend-nf1m.onrender.com/api/Recommendations/${type}?userId=${userId}`, {
                headers: { Authorization: `Bearer ${token}` },
            });

            if (!res.ok) throw new Error("Failed to fetch recommendations.");
            const data = await res.json();
            setMovies(data);
        } catch (err) {
            setError((err as Error).message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchRecommendations();
    }, [type]);

    return (
        <div style={{ minHeight: "100vh", backgroundColor: "#111", color: "white" }}>
            <div
                className="bg-dark py-3 border-bottom shadow"
                style={{
                    position: "sticky",
                    top: 0,
                    width: "100%",
                    zIndex: 1050,
                }}
            >
                <div className="d-flex justify-content-between align-items-center flex-wrap px-4">
                    <div className="d-flex flex-wrap gap-3">
                        <a href="/movies" className="btn btn-outline-light btn-sm">🎬 Movies</a>
                        <a href="/events" className="btn btn-outline-light btn-sm">📅 Events</a>
                        <a href="/feed" className="btn btn-outline-light btn-sm">📰 Feed</a>
                        <a href="/people" className="btn btn-outline-light btn-sm">👥 People</a>
                        <button
                            className="btn btn-outline-danger btn-sm"
                            onClick={() => {
                                localStorage.removeItem('token');
                                localStorage.removeItem('userId');
                                window.location.href = '/login';
                            }}
                        >
                            🚪 Logout
                        </button>
                    </div>

                    <a href="/profile" className="btn btn-secondary btn-sm">Profile</a>
                </div>
            </div>

            <div className="container py-4">
                <h2 className="mb-3">⭐ Recommendations</h2>

                <div className="mb-4">
                    <label htmlFor="recType" className="form-label">Select Recommendation Type:</label>
                    <select
                        id="recType"
                        className="form-select"
                        value={type}
                        onChange={(e) => setType(e.target.value)}
                    >
                        <option value="genre">By Genre</option>
                        <option value="actors">By Actors</option>
                        <option value="description">By Description</option>
                        <option value="combined">Combined</option>
                    </select>
                </div>

                {loading && <p>Loading recommendations...</p>}
                {error && <div className="alert alert-danger">{error}</div>}

                <div className="row g-3">
                    {movies.map((movie) => (
                        <div className="col-6 col-sm-4 col-md-3 col-lg-2" key={movie.id}>
                            <MovieCard {...movie} />
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default Recommendation;
