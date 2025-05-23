import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

export interface MovieCardProps {
    id: string;
    title: string;
    year: number | null;
    averageRating: number;
    genres: string[];
    posterUrl?: string;
    isWatched?: boolean;
    onMarked?: (id: string) => void;
}

const MovieCard: React.FC<MovieCardProps> = ({
                                                 id,
                                                 title,
                                                 year,
                                                 averageRating,
                                                 genres,
                                                 posterUrl,
                                                 isWatched: initialIsWatched,
                                                 onMarked,
                                             }) => {
    const navigate = useNavigate();
    const [isWatched, setIsWatched] = useState(initialIsWatched ?? false);
    const [showEventForm, setShowEventForm] = useState(false);

    // Event form state
    const [eventLocation, setEventLocation] = useState("");
    const [eventDate, setEventDate] = useState("");
    const [eventParticipants, setEventParticipants] = useState(0);
    const [eventdescription, setEventdescription] = useState("");

    useEffect(() => {
        setIsWatched(initialIsWatched ?? false);
    }, [initialIsWatched]);

    const markAsWatched = async () => {
        const token = localStorage.getItem("token");
        if (!token) {
            alert("You must be logged in.");
            return;
        }

        try {
            const res = await fetch("http://localhost:5000/api/UserMovie/MarkAsWatched", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ title, year }),
            });

            if (!res.ok) {
                const j = await res.json().catch(() => ({}));
                throw new Error(j?.error?.message ?? "Server error");
            }

            setIsWatched(true);
            onMarked?.(id);
        } catch (err) {
            console.error(err);
            alert(err instanceof Error ? err.message : "Server error");
        }
    };

    const createEvent = async () => {
        const token = localStorage.getItem("token");
        if (!token) {
            alert("You must be logged in.");
            return;
        }

        try {
            const res = await fetch("http://localhost:5000/api/Event/create", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({
                    title,
                    description: eventdescription,
                    location: eventLocation,
                    date: new Date(eventDate).toISOString(),
                    maxParticipants: eventParticipants,
                    movieId: id,
                }),
            });

            if (!res.ok) {
                const j = await res.json().catch(() => ({}));
                throw new Error(j?.error?.message ?? "Server error");
            }

            alert("Event created successfully!");
            setShowEventForm(false);
        } catch (err) {
            console.error(err);
            alert(err instanceof Error ? err.message : "Server error");
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
                <p className="card-text mb-1">
                    {year ?? "—"} • {genres.join(", ")}
                </p>
                <p className="card-text">⭐ {averageRating.toFixed(1)}</p>

                <div className="d-flex gap-2 mt-2">
                    <button
                        className="btn btn-outline-info btn-sm"
                        onClick={() => navigate(`/movies/${id}`)}
                    >
                        Details
                    </button>

                    <button
                        className={`btn btn-sm ${isWatched ? "btn-success" : "btn-outline-success"}`}
                        onClick={markAsWatched}
                        disabled={isWatched}
                    >
                        {isWatched ? "Watched" : "Mark as Watched"}
                    </button>

                    <button
                        className="btn btn-outline-warning btn-sm"
                        onClick={() => setShowEventForm(!showEventForm)}
                    >
                        Create Event
                    </button>
                </div>

                {showEventForm && (
                    <div className="mt-3 border-top pt-3">
                        <h6>Create Event</h6>
                        <input
                            type="datetime-local"
                            className="form-control mb-2"
                            value={eventDate}
                            onChange={(e) => setEventDate(e.target.value)}
                        />
                        <input
                            type="text"
                            className="form-control mb-2"
                            placeholder="Location"
                            value={eventLocation}
                            onChange={(e) => setEventLocation(e.target.value)}
                        />
                        <input
                            type="number"
                            className="form-control mb-2"
                            placeholder="Max Participants"
                            value={eventParticipants}
                            onChange={(e) => setEventParticipants(Number(e.target.value))}
                        />
                        <textarea
                            className="form-control mb-2"
                            placeholder="Event Description"
                            value={eventdescription}
                            onChange={(e) => setEventdescription(e.target.value)}
                        />
                        <button className="btn btn-primary btn-sm" onClick={createEvent}>
                            Submit Event
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
};

export default MovieCard;
