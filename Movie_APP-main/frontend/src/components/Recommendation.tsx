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
            const res = await fetch(`http://localhost:5000/api/Recommendations/${type}?userId=${userId}`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
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
        <div className="container mt-4">
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

            <div className="row row-cols-1 row-cols-md-3 g-4">
                {movies.map((movie) => (
                    <div className="col" key={movie.id}>
                        <MovieCard {...movie} />
                    </div>
                ))}
            </div>
        </div>
    );
};

export default Recommendation;
