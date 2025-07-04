﻿import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { MovieCardProps } from "./MovieCard";
import ActorCard from "./ActorCard";
import { getUserIdFromToken, getUserNameFromToken } from "../utils/jwt";

interface Review {
    id: string;
    content: string;
    rating: number;
    userId: string;
    author?: string;
    isOwnReview?: boolean;
    movieId?: string;
}

interface CrewMember {
    id: string;
    firstName: string;
    lastName: string;
    imageUrl?: string | null;
}

const StarRating: React.FC<{
    rating: number;
    onChange: (r: number) => void;
}> = ({ rating, onChange }) => {
    const [hover, setHover] = useState<number | null>(null);

    return (
        <div>
            {[...Array(10)].map((_, i) => (
                <span
                    key={i}
                    onClick={() => onChange(i + 1)}
                    onMouseEnter={() => setHover(i + 1)}
                    onMouseLeave={() => setHover(null)}
                    className="star"
                    style={{
                        color: (hover ?? rating) > i ? "#ffc107" : "#e4e5e9",
                    }}
                >
                    ★
                </span>
            ))}
        </div>
    );
};

const MovieDetails: React.FC = () => {
    const { id } = useParams();
    const userId = getUserIdFromToken();
    const userName = getUserNameFromToken()?.toLowerCase().trim();

    const [movie, setMovie] = useState<(MovieCardProps & { description: string }) | null>(null);
    const [reviews, setReviews] = useState<Review[]>([]);
    const [crew, setCrew] = useState<CrewMember[]>([]);
    const [reviewText, setReviewText] = useState("");
    const [reviewRating, setReviewRating] = useState(1);
    const [editingReviewId, setEditingReviewId] = useState<string | null>(null);
    const [editingContent, setEditingContent] = useState("");
    const [editingRating, setEditingRating] = useState(1);

    const fetchReviews = async (title: string, year: number) => {
        try {
            const res = await fetch(
                `https://licenta-backend-nf1m.onrender.com/api/Review/GetByMovieTitleAndYear?title=${encodeURIComponent(title)}&year=${year}`
            );
            const json = await res.json();

            const reviewsWithOwnership = json.result.map((r: Review) => {
                const matchById = r.userId === userId;
                const matchByAuthor = r.author?.toLowerCase().trim() === userName;
                return { ...r, isOwnReview: matchById || matchByAuthor };
            });

            setReviews(reviewsWithOwnership);
        } catch (err) {
            console.error("Error fetching reviews:", err);
        }
    };

    useEffect(() => {
        if (!id) return;
        fetch(`https://licenta-backend-nf1m.onrender.com/api/movie/${id}`)
            .then(res => res.json())
            .then(json => {
                setMovie(json.result);
                if (typeof json.result.year === "number") {
                    fetchReviews(json.result.title, json.result.year);
                }
            });
    }, [id]);

    useEffect(() => {
        if (!id) return;
        const token = localStorage.getItem("token");
        fetch(`https://licenta-backend-nf1m.onrender.com/api/Crew/movie/${id}`, {
            headers: { Authorization: `Bearer ${token}` },
        })
            .then(res => res.json())
            .then(json => setCrew(Array.isArray(json) ? json : []));
    }, [id]);

    const handleSubmitReview = async () => {
        const token = localStorage.getItem("token");
        if (!token || !movie?.id) return;

        if (reviews.some(r => r.isOwnReview)) {
            alert("You already submitted a review.");
            return;
        }

        if (reviewRating < 1 || reviewRating > 10) {
            alert("Rating must be between 1 and 10.");
            return;
        }

        try {
            const watchedRes = await fetch(
                `https://licenta-backend-nf1m.onrender.com/api/UserMovie/GetWatchedMovies/watched`,
                { headers: { Authorization: `Bearer ${token}` } }
            );
            const watchedJson = await watchedRes.json();
            const watchedIds = Array.isArray(watchedJson.result)
                ? watchedJson.result.map((m: any) => String(m.id).trim())
                : [];

            if (!watchedIds.includes(movie.id)) {
                alert("You must mark this movie as watched before leaving a review.");
                return;
            }

            const res = await fetch(
                `https://licenta-backend-nf1m.onrender.com/api/Review/Add`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                    body: JSON.stringify({
                        movieId: movie.id,
                        content: reviewText.trim(),
                        rating: reviewRating,
                    }),
                }
            );

            if (res.ok) {
                setReviewText("");
                setReviewRating(1);
                if (!movie?.title || movie.year == null) return;
                await fetchReviews(movie.title, movie.year);
            } else {
                console.error("Failed to submit review:", await res.text());
            }
        } catch (err) {
            console.error("Submit review error:", err);
        }
    };

    const averageRating = reviews.length
        ? (reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length).toFixed(1)
        : "N/A";

    return (
        <div className="container text-white py-4">
            <div className="row">
                <div className="col-md-4">
                    <img
                        src={movie?.posterUrl ?? "https://via.placeholder.com/300x450?text=No+Image"}
                        alt={movie?.title}
                        className="img-fluid rounded shadow"
                        style={{ width: "75%", maxWidth: "350px", height: "auto" }}
                    />
                </div>
                <div className="col-md-8">
                    <h2>{movie?.title}</h2>
                    <p><strong>Year:</strong> {movie?.year}</p>
                    <p><strong>Genres:</strong> {movie?.genres.join(", ")}</p>
                    <p><strong>Rating:</strong> ⭐ {averageRating}</p>
                    <p><strong>Description:</strong> {movie?.description}</p>
                </div>
            </div>

            <h3 className="mt-4">Cast & Crew</h3>
            {crew.length === 0 ? <p>No cast information.</p> : (
                <div className="d-flex overflow-auto py-2 gap-3">
                    {crew.map(member => (
                        <div style={{ flex: "0 0 auto", width: "150px" }} key={member.id}>
                            <ActorCard {...member} />
                        </div>
                    ))}
                </div>
            )}

            <h3 className="mt-4">Reviews</h3>
            {reviews.length === 0 ? <p>No reviews yet.</p> : reviews.map((rev) => (
                <div key={rev.id} className="mb-3">
                    <p><strong>{rev.isOwnReview ? "You" : rev.author || "Unknown"}</strong></p>
                    {editingReviewId === rev.id ? (
                        <>
                            <textarea
                                className="form-control mb-2"
                                value={editingContent}
                                onChange={(e) => setEditingContent(e.target.value)}
                            />
                            <StarRating
                                rating={editingRating}
                                onChange={setEditingRating}
                            />
                            <button
                                type="button"
                                className="btn btn-primary btn-sm me-2 mt-2"
                                onClick={async () => {
                                    const token = localStorage.getItem("token");
                                    if (!token || !movie?.id) return;

                                    const updatedReview = {
                                        id: rev.id,
                                        content: editingContent.trim(),
                                        rating: editingRating,
                                        movieId: movie.id,
                                        userId: rev.userId,
                                        author: rev.author,
                                    };

                                    const res = await fetch(
                                        `https://licenta-backend-nf1m.onrender.com/api/Review/Update/${rev.id}`,
                                        {
                                            method: "PUT",
                                            headers: {
                                                "Content-Type": "application/json",
                                                Authorization: `Bearer ${token}`,
                                            },
                                            body: JSON.stringify(updatedReview),
                                        }
                                    );

                                    if (res.ok) {
                                        setReviews(prev =>
                                            prev.map(r => r.id === rev.id ? { ...r, ...updatedReview } : r)
                                        );
                                        setEditingReviewId(null);
                                    } else {
                                        console.error("Failed to update review");
                                    }
                                }}
                            >Save</button>
                            <button
                                type="button"
                                className="btn btn-secondary btn-sm mt-2"
                                onClick={() => setEditingReviewId(null)}
                            >Cancel</button>
                        </>
                    ) : (
                        <>
                            <strong>⭐ {rev.rating}</strong> – {rev.content}
                            {rev.isOwnReview && (
                                <div className="mt-1">
                                    <button
                                        type="button"
                                        className="btn btn-warning btn-sm me-2"
                                        onClick={() => {
                                            setEditingReviewId(rev.id);
                                            setEditingContent(rev.content);
                                            setEditingRating(rev.rating);
                                        }}
                                    >Edit</button>
                                    <button
                                        type="button"
                                        className="btn btn-danger btn-sm"
                                        onClick={async () => {
                                            const token = localStorage.getItem("token");
                                            if (!token) return;
                                            await fetch(
                                                `https://licenta-backend-nf1m.onrender.com/api/Review/Delete/${rev.id}`,
                                                {
                                                    method: "DELETE",
                                                    headers: { Authorization: `Bearer ${token}` },
                                                }
                                            );
                                            if (movie?.title && movie.year != null) await fetchReviews(movie.title, movie.year);
                                        }}
                                    >Delete</button>
                                </div>
                            )}
                        </>
                    )}
                </div>
            ))}

            {!reviews.some(r => r.isOwnReview) && (
                <>
                    <h4 className="mt-4">Add a Review</h4>
                    <textarea
                        className="form-control mb-2"
                        value={reviewText}
                        onChange={(e) => setReviewText(e.target.value)}
                        placeholder="Write your review"
                    />
                    <StarRating
                        rating={reviewRating}
                        onChange={setReviewRating}
                    />
                    <button
                        type="button"
                        className="btn btn-success mt-2"
                        onClick={handleSubmitReview}
                    >Add Review</button>
                </>
            )}
        </div>
    );
};

export default MovieDetails;
