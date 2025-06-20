﻿import React, { useEffect, useState } from "react";
import "bootstrap/dist/css/bootstrap.min.css";

interface WatchedMovieDTO {
    movieTitle: string;
    posterUrl?: string;
    releaseYear: number;
    description?: string;
    averageRating: number;
    friendName: string;
    watchedAt: string;
}

interface ReviewFeedDTO {
    movieTitle: string;
    posterUrl?: string;
    releaseYear: number;
    description?: string;
    averageRating: number;
    friendName: string;
    reviewedAt: string;
    reviewText: string;
    friendRating: number;
}

interface EventFeedDTO {
    id: string;
    title: string;
    description?: string;
    location: string;
    date: string;
    maxParticipants: number;
    freeSeats: number;
    organizerId: string;
    movieId: string;
    createdAt: string;
    movieTitle: string;
    moviePosterUrl?: string;
    friendName: string;
}

type Tab = "watched" | "reviews" | "events";

const Feed: React.FC = () => {
    const [tab, setTab] = useState<Tab>("events");

    const [watched, setWatched] = useState<WatchedMovieDTO[]>([]);
    const [reviews, setReviews] = useState<ReviewFeedDTO[]>([]);
    const [events, setEvents] = useState<EventFeedDTO[]>([]);

    const token = localStorage.getItem("token");
    const userId = localStorage.getItem("userId");

    useEffect(() => {
        if (!token || !userId) return;

        const headers = { Authorization: `Bearer ${token}` };

        fetch(`http://localhost:5000/api/feed/watched/${userId}`, { headers })
            .then(res => res.json())
            .then(setWatched)
            .catch(console.error);

        fetch(`http://localhost:5000/api/feed/reviews/${userId}`, { headers })
            .then(res => res.json())
            .then(setReviews)
            .catch(console.error);

        fetch(`http://localhost:5000/api/feed/events/${userId}`, { headers })
            .then(res => res.json())
            .then(setEvents)
            .catch(console.error);
    }, [token, userId]);

    return (
        <div className="container py-4 text-white">
            <h2 className="mb-4">📰 Feed</h2>

            <div className="btn-group mb-4">
                <button className={`btn btn-outline-info ${tab === "watched" ? "active" : ""}`} onClick={() => setTab("watched")}>Watched Movies</button>
                <button className={`btn btn-outline-warning ${tab === "reviews" ? "active" : ""}`} onClick={() => setTab("reviews")}>Reviews</button>
                <button className={`btn btn-outline-success ${tab === "events" ? "active" : ""}`} onClick={() => setTab("events")}>Events</button>
            </div>

            {tab === "watched" && (
                <div className="row g-4">
                    {watched.map((item, index) => (
                        <div className="col-md-4" key={index}>
                            <div className="card bg-dark text-white h-100 border-secondary">
                                <img src={item.posterUrl ?? "https://via.placeholder.com/300x450?text=No+Image"} className="card-img-top" alt={item.movieTitle} style={{ height: 300, objectFit: "cover" }} />
                                <div className="card-body">
                                    <h5 className="card-title">{item.movieTitle}</h5>
                                    <p className="card-text"><strong>👤 {item.friendName}</strong> watched on {new Date(item.watchedAt).toLocaleDateString()}</p>
                                    <p className="card-text">🎬 Year: {item.releaseYear} | ⭐ {item.averageRating.toFixed(1)}</p>
                                    {item.description && <p className="card-text">{item.description}</p>}
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {tab === "reviews" && (
                <div className="row g-4">
                    {reviews.map((item, index) => (
                        <div className="col-md-4" key={index}>
                            <div className="card bg-dark text-white h-100 border-warning">
                                <img src={item.posterUrl ?? "https://via.placeholder.com/300x450?text=No+Image"} className="card-img-top" alt={item.movieTitle} style={{ height: 300, objectFit: "cover" }} />
                                <div className="card-body">
                                    <h5 className="card-title">{item.movieTitle}</h5>
                                    <p className="card-text"><strong>👤 {item.friendName}</strong> reviewed on {new Date(item.reviewedAt).toLocaleDateString()}</p>
                                    <p className="card-text">⭐ {item.friendRating}/10</p>
                                    <blockquote className="blockquote">
                                        <p className="mb-0">"{item.reviewText}"</p>
                                    </blockquote>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {tab === "events" && (
                <div className="row g-4">
                    {events.map((event) => (
                        <div key={event.id} className="col-md-4">
                            <div className="card h-100 bg-dark text-white border-secondary shadow">
                                <img
                                    src={event.moviePosterUrl ?? "https://via.placeholder.com/300x450?text=No+Image"}
                                    alt={event.movieTitle ?? "No title"}
                                    className="card-img-top object-fit-cover"
                                    style={{ height: 300 }}
                                />
                                <div className="card-body">
                                    <h5 className="card-title">{event.title}</h5>
                                    <p className="card-text"><strong>👤 Organizer:</strong> {event.friendName}</p>
                                    <p className="card-text"><strong>🎬 Movie:</strong> {event.movieTitle ?? "Unknown"}</p>
                                    <p className="card-text">{event.description}</p>
                                    <p className="card-text"><strong>📍 Location:</strong> {event.location}</p>
                                    <p className="card-text"><strong>🗓️ Date:</strong> {new Date(event.date).toLocaleString()}</p>
                                    <p className="card-text"><strong>🕒 Created At:</strong> {new Date(event.createdAt).toLocaleString()}</p>
                                    <p className="card-text"><strong>👥 Seats:</strong> {event.freeSeats}/{event.maxParticipants}</p>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};

export default Feed;
