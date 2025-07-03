import React, { useEffect, useState } from "react";
import EditPopup from "./EditPopup";

interface Event {
    id: string;
    title: string;
    description: string;
    location: string;
    date: string;
    maxParticipants: number;
    freeSeats: number;
    organizerId: string;
    movieId: string;
    createdAt: string;
    movieTitle?: string;
    moviePosterUrl?: string;
}

interface ApiResponse<T> {
    result: T;
}

type ViewMode = "all" | "my" | "participation";

const EventsPage: React.FC = () => {
    const [events, setEvents] = useState<Event[]>([]);
    const [loading, setLoading] = useState(true);
    const [view, setView] = useState<ViewMode>("all");
    const [editingEvent, setEditingEvent] = useState<Event | null>(null);

    const fetchEvents = async (mode: ViewMode) => {
        setLoading(true);
        let url = "https://licenta-backend-nf1m.onrender.com/api/Event/unattended";

        if (mode === "my") url = "https://licenta-backend-nf1m.onrender.com/api/Event/organizer";
        if (mode === "participation") url = "https://licenta-backend-nf1m.onrender.com/api/Event/participant";

        try {
            const token = localStorage.getItem("token");
            const res = await fetch(url, {
                headers: token ? { Authorization: `Bearer ${token}` } : undefined,
            });
            const data: ApiResponse<Event[]> = await res.json();
            if (!res.ok) throw new Error(data as unknown as string);
            setEvents(data.result);
        } catch (err) {
            console.error(err);
            alert("Error fetching events.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchEvents(view);
    }, [view]);

    const attendEvent = async (eventId: string) => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in!");
        try {
            const res = await fetch(
                `https://licenta-backend-nf1m.onrender.com/api/Event/attend/${eventId}`,
                {
                    method: "POST",
                    headers: { Authorization: `Bearer ${token}` },
                }
            );
            if (!res.ok) throw new Error("Failed to attend.");
            alert("Successfully joined the event!");
            fetchEvents(view);
        } catch {
            alert("Error joining event.");
        }
    };

    const unattendEvent = async (eventId: string) => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in!");
        if (!window.confirm("Are you sure you want to leave this event?")) return;

        try {
            const res = await fetch(
                `https://licenta-backend-nf1m.onrender.com/api/Event/unattend/${eventId}`,
                {
                    method: "POST",
                    headers: { Authorization: `Bearer ${token}` },
                }
            );
            if (!res.ok) throw new Error("Failed to unattend.");
            alert("You have left the event.");
            fetchEvents(view);
        } catch {
            alert("Error leaving event.");
        }
    };

    const deleteEvent = async (eventId: string) => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in!");
        if (!window.confirm("Are you sure you want to delete this event?")) return;
        try {
            const res = await fetch(
                `https://licenta-backend-nf1m.onrender.com/api/Event/delete/${eventId}`,
                {
                    method: "DELETE",
                    headers: { Authorization: `Bearer ${token}` },
                }
            );
            if (!res.ok) throw new Error("Failed to delete event.");
            alert("Event deleted successfully.");
            fetchEvents(view);
        } catch (err) {
            alert("Error deleting event.");
            console.error(err);
        }
    };

    const updateEvent = async (updatedEvent: Event) => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in!");
        try {
            const res = await fetch(
                `https://licenta-backend-nf1m.onrender.com/api/Event/update/${updatedEvent.id}`,
                {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                    body: JSON.stringify(updatedEvent),
                }
            );
            if (!res.ok) throw new Error("Failed to update event.");
            alert("Event updated successfully.");
            fetchEvents(view);
        } catch (err) {
            alert("Error updating event.");
            console.error(err);
        }
    };

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
                        <a href="/movies" className="btn btn-outline-light btn-sm">
                            🎬 Movies
                        </a>
                        <a href="/recommendation" className="btn btn-outline-light btn-sm">
                            ⭐ Recommendations
                        </a>
                        <a href="/feed" className="btn btn-outline-light btn-sm">
                            📰 Feed
                        </a>
                        <a href="/people" className="btn btn-outline-light btn-sm">
                            👥 People
                        </a>
                        <button
                            className="btn btn-outline-danger btn-sm"
                            onClick={() => {
                                localStorage.removeItem("token");
                                localStorage.removeItem("userId");
                                window.location.href = "/login";
                            }}
                        >
                            🚪 Logout
                        </button>
                    </div>
                    <a href="/profile" className="btn btn-secondary btn-sm">
                        Profile
                    </a>
                </div>
            </div>

            <div className="container py-3">
                <h2 className="mb-3">🗓️ Events</h2>

                <div className="btn-group mb-4">
                    <button
                        className={`btn btn-outline-primary ${view === "all" ? "active" : ""}`}
                        onClick={() => setView("all")}
                    >
                        All Events
                    </button>
                    <button
                        className={`btn btn-outline-success ${view === "my" ? "active" : ""}`}
                        onClick={() => setView("my")}
                    >
                        My Events
                    </button>
                    <button
                        className={`btn btn-outline-info ${view === "participation" ? "active" : ""}`}
                        onClick={() => setView("participation")}
                    >
                        Participation Events
                    </button>
                </div>

                {loading ? (
                    <div className="text-center">
                        <div className="spinner-border text-light" />
                    </div>
                ) : events.length === 0 ? (
                    <p className="text-muted">No events found.</p>
                ) : (
                    <div className="row g-4">
                        {events.map((event) => (
                            <div
                                key={event.id}
                                className="col-6 col-sm-4 col-md-3"
                                style={{ flex: "0 0 20%", maxWidth: "20%" }}
                            >
                                <div className="card h-100 bg-dark text-white border-secondary shadow">
                                    <img
                                        src={
                                            event.moviePosterUrl ??
                                            "https://via.placeholder.com/300x450?text=No+Image"
                                        }
                                        alt={event.movieTitle ?? "No title"}
                                        className="card-img-top object-fit-cover"
                                        style={{ height: 300 }}
                                    />
                                    <div className="card-body">
                                        <h5 className="card-title">{event.title}</h5>
                                        <p className="card-text">
                                            <strong>🎮 Movie:</strong> {event.movieTitle ?? "Unknown"}
                                        </p>
                                        <p className="card-text">{event.description}</p>
                                        <p className="card-text">
                                            <strong>📍 Location:</strong> {event.location}
                                        </p>
                                        <p className="card-text">
                                            <strong>🗓️ Date:</strong>{" "}
                                            {new Date(event.date).toLocaleString()}
                                        </p>
                                        <p className="card-text">
                                            <strong>👥 Seats:</strong> {event.freeSeats}/
                                            {event.maxParticipants}
                                        </p>

                                        {view === "all" && (
                                            <button
                                                className="btn btn-sm btn-success mt-2"
                                                onClick={() => attendEvent(event.id)}
                                            >
                                                Attend
                                            </button>
                                        )}

                                        {view === "my" && (
                                            <>
                                                <button
                                                    className="btn btn-sm btn-warning mt-2 me-2"
                                                    onClick={() => setEditingEvent(event)}
                                                >
                                                    Modify
                                                </button>
                                                <button
                                                    className="btn btn-sm btn-danger mt-2"
                                                    onClick={() => deleteEvent(event.id)}
                                                >
                                                    Delete Event
                                                </button>
                                            </>
                                        )}

                                        {view === "participation" && (
                                            <button
                                                className="btn btn-sm btn-danger mt-2"
                                                onClick={() => unattendEvent(event.id)}
                                            >
                                                Unattend
                                            </button>
                                        )}
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                )}

                {editingEvent && (
                    <EditPopup
                        event={editingEvent}
                        onClose={() => setEditingEvent(null)}
                        onSave={(updated) => {
                            updateEvent(updated);
                            setEditingEvent(null);
                        }}
                    />
                )}
            </div>
        </div>
    );
};

export default EventsPage;
