import React from "react";
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
    onUnrecommended?: (id: string) => void;
    showUnwatchButton?: boolean;
    onUnwatch?: (id: string) => void;
}

const MovieCardWatched: React.FC<MovieCardProps> = ({
                                                        id,
                                                        title,
                                                        year,
                                                        averageRating,
                                                        genres,
                                                        posterUrl,
                                                        isRecommended = false,
                                                        onRecommended,
                                                        onUnrecommended,
                                                        showUnwatchButton = false,
                                                        onUnwatch,
                                                    }) => {
    const navigate = useNavigate();

    const handleMarkAsRecommended = async () => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in.");

        try {
            const res = await fetch("https://licenta-backend-nf1m.onrender.com/api/UserMovie/MarkAsRecommended/recommend", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ title, year }),
            });

            if (!res.ok) {
                let errorMessage = "Error marking movie as recommended.";
                try {
                    const data = await res.json();
                    errorMessage = data?.error?.message || errorMessage;
                } catch {}
                throw new Error(errorMessage);
            }

            onRecommended?.(id);
            alert("Movie successfully marked as recommended.");
        } catch (err) {
            alert((err as Error).message);
        }
    };

    const handleUnrecommend = async () => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in.");

        try {
            const res = await fetch("https://licenta-backend-nf1m.onrender.com/api/UserMovie/UnmarkAsRecommended/unrecommend", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ title, year }),
            });

            if (!res.ok) {
                let errorMessage = "Failed to unrecommend movie.";
                try {
                    const data = await res.json();
                    errorMessage = data?.error?.message || errorMessage;
                } catch {}
                throw new Error(errorMessage);
            }

            onUnrecommended?.(id);
            alert("Movie was removed from recommendations.");
        } catch (err) {
            alert((err as Error).message);
        }
    };

    const handleUnwatch = async () => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in.");

        try {
            const res = await fetch("https://licenta-backend-nf1m.onrender.com/api/UserMovie/unmarkaswatched", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ title, year }),
            });

            if (!res.ok) {
                let errorMessage = "Failed to unmark movie as watched.";
                try {
                    const data = await res.json();
                    errorMessage = data?.error?.message || errorMessage;
                } catch {}
                throw new Error(errorMessage);
            }

            onUnwatch?.(id);
            alert("Movie was unmarked as watched.");
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

                <div className="d-flex gap-2 mt-2 flex-wrap">
                    <button
                        className="btn btn-outline-info btn-sm"
                        onClick={() => navigate(`/movies/${id}`)}
                    >
                        Details
                    </button>

                    {!isRecommended && (
                        <button
                            className="btn btn-outline-warning btn-sm"
                            onClick={handleMarkAsRecommended}
                        >
                            Mark as Recommended
                        </button>
                    )}

                    {isRecommended && (
                        <button
                            className="btn btn-outline-danger btn-sm"
                            onClick={handleUnrecommend}
                        >
                            Unrecommend
                        </button>
                    )}

                    {showUnwatchButton && (
                        <button
                            className="btn btn-outline-secondary btn-sm"
                            onClick={handleUnwatch}
                        >
                            Unwatch
                        </button>
                    )}
                </div>
            </div>
        </div>
    );
};

export default MovieCardWatched;
