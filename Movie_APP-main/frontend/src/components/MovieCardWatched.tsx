import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

export interface MovieCardProps {
    id: string;
    title: string;
    year: number | null;
    averageRating: number;
    genres: string[];
    posterUrl?: string;
    isRecommended?: boolean;
    onRecommended?: (id: string) => void;
}

const MovieCardWatched: React.FC<MovieCardProps> = ({
                                                        id, title, year, averageRating, genres, posterUrl,
                                                        isRecommended: initialIsRecommended,
                                                        onRecommended
                                                    }) => {
    const navigate = useNavigate();
    const [isRecommended, setIsRecommended] = useState(initialIsRecommended ?? false);

    const markAsRecommended = async () => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in.");

        try {
            const res = await fetch("https://licenta-backend-nf1m.onrender.com/api/UserMovie/recommend", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ title, year }),
            });

            if (!res.ok) {
                const data = await res.json();
                throw new Error(data?.error?.message || "Error marking movie as recommended.");
            }

            setIsRecommended(true);
            onRecommended?.(id);
            alert("Movie successfully marked as recommended.");
        } catch (err) {
            alert((err as Error).message);
        }
    };

    return (
        <div className="card h-100 text-white bg-dark border-secondary">
            <img
                src={posterUrl ?? "https://via.placeholder.com/300x450?text=No+Image"}
                alt={title}
                className="card-img-top object-fit-cover"
                style={{ height: 300 }}
            />
            <div className="card-body">
                <h5 className="card-title">{title}</h5>
                <p className="card-text mb-1">{year ?? "—"} • {genres.join(", ")}</p>
                <p className="card-text">⭐ {averageRating.toFixed(1)}</p>

                <div className="d-flex gap-2 mt-2">
                    <button className="btn btn-outline-info btn-sm" onClick={() => navigate(`/movies/${id}`)}>Details</button>
                    <button
                        className={`btn btn-sm ${isRecommended ? "btn-success" : "btn-outline-warning"}`}
                        onClick={markAsRecommended}
                        disabled={isRecommended}
                    >
                        {isRecommended ? "Recommended" : "Mark as Recommended"}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default MovieCardWatched;
