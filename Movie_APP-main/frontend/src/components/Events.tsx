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
type FilterMode = "none" | "location" | "day" | "month" | "movie";

const EventsPage: React.FC = () => {
    const [events, setEvents] = useState<Event[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [view, setView] = useState<ViewMode>("all");
    const [filterMode, setFilterMode] = useState<FilterMode>("none");
    const [filterValue, setFilterValue] = useState<string>("");
    const [editingEvent, setEditingEvent] = useState<Event | null>(null);

    const fetchEvents = async (mode: ViewMode) => {
        setLoading(true);
        let url = "https://licenta-backend-nf1m.onrender.com/api/Event/unattended";

        if (mode === "my") url = "https://licenta-backend-nf1m.onrender.com/api/Event/organizer";
        if (mode === "participation") url = "https://licenta-backend-nf1m.onrender.com/api/Event/participant";

        try {
            const token = localStorage.getItem("token");
            const res = await fetch(url, {
                headers: token ? { Authorization: `Bearer ${token}` } : {},
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
            const res = await fetch(`https://licenta-backend-nf1m.onrender.com/api/Event/attend/${eventId}`, {
                method: "POST",
                headers: { Authorization: `Bearer ${token}` },
            });

            if (!res.ok) throw new Error("Failed to attend.");
            alert("Successfully joined the event!");
            fetchEvents(view);
        } catch {
            alert("Error joining event.");
        }
    };

    const deleteEvent = async (eventId: string) => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in!");

        const confirmDelete = window.confirm("Are you sure you want to delete this event?");
        if (!confirmDelete) return;

        try {
            const res = await fetch(`https://licenta-backend-nf1m.onrender.com/api/Event/delete/${eventId}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${token}` },
            });

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
            const res = await fetch(`https://licenta-backend-nf1m.onrender.com/api/Event/update/${updatedEvent.id}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify(updatedEvent),
            });

            if (!res.ok) throw new Error("Failed to update event.");
            alert("Event updated successfully.");
            fetchEvents(view);
        } catch (err) {
            alert("Error updating event.");
            console.error(err);
        }
    };

    const applyFilter = async () => {
        if (!filterValue.trim()) return;
        setLoading(true);
        let url = "";

        switch (filterMode) {
            case "location":
                url = `https://licenta-backend-nf1m.onrender.com/api/Event/by-location?location=${encodeURIComponent(filterValue)}`;
                break;
            case "day":
                url = `https://licenta-backend-nf1m.onrender.com/api/Event/by-day?date=${filterValue}`;
                break;
            case "month": {
                const [year, month] = filterValue.split("-");
                if (!year || !month) {
                    alert("Please enter month as YYYY-MM");
                    setLoading(false);
                    return;
                }
                url = `https://licenta-backend-nf1m.onrender.com/api/Event/by-month?year=${year}&month=${month}`;
                break;
            }
            case "movie":
                url = `https://licenta-backend-nf1m.onrender.com/api/Event/by-movie-title?title=${encodeURIComponent(filterValue)}`;
                break;
            default:
                setLoading(false);
                return;
        }

        try {
            const res = await fetch(url);
            const data: ApiResponse<Event[]> = await res.json();
            if (!res.ok) throw new Error(data as unknown as string);
            setEvents(data.result);
        } catch {
            alert("Error applying filter.");
        } finally {
            setLoading(false);
        }
    };

    const resetFilter = () => {
        setFilterMode("none");
        setFilterValue("");
        fetchEvents(view);
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
                    <div className="d-flex justify-content-center flex-wrap gap-3 px-4">
                    <a href="/recommendation" className="btn btn-outline-light btn-sm">⭐ Recommendations</a>
                    <a href="/movies" className="btn btn-outline-light btn-sm">🎬 Movies</a>
                    <a href="/feed" className="btn btn-outline-light btn-sm">📰 Feed</a>
                    <a href="/people" className="btn btn-outline-light btn-sm">👥  People</a>
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
            </div>
            
            <h2>🗓️Events</h2>

            <div className="btn-group mb-4">
                <button className={`btn btn-outline-primary ${view === "all" ? "active" : ""}`} onClick={() => setView("all")}>All Events</button>
                <button className={`btn btn-outline-success ${view === "my" ? "active" : ""}`} onClick={() => setView("my")}>My Events</button>
                <button className={`btn btn-outline-info ${view === "participation" ? "active" : ""}`} onClick={() => setView("participation")}>Participation Events</button>
            </div>

            <div className="mb-4 d-flex align-items-center gap-2">
                <select className="form-select w-auto" value={filterMode} onChange={(e) => setFilterMode(e.target.value as FilterMode)}>
                    <option value="none">No filter</option>
                    <option value="location">Location</option>
                    <option value="day">Date (yyyy-mm-dd)</option>
                    <option value="month">Month (yyyy-mm)</option>
                    <option value="movie">Movie Title</option>
                </select>

                <input
                    type="text"
                    className="form-control w-auto"
                    placeholder="Enter filter value"
                    value={filterValue}
                    onChange={(e) => setFilterValue(e.target.value)}
                    disabled={filterMode === "none"}
                />

                <button className="btn btn-secondary" onClick={applyFilter} disabled={filterMode === "none"}>Apply Filter</button>
                <button className="btn btn-outline-danger" onClick={resetFilter}>Clear Filter</button>
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
                                    <p className="card-text"><strong>🎮 Movie:</strong> {event.movieTitle ?? "Unknown"}</p>
                                    <p className="card-text">{event.description}</p>
                                    <p className="card-text"><strong>📍 Location:</strong> {event.location}</p>
                                    <p className="card-text"><strong>🗓️ Date:</strong> {new Date(event.date).toLocaleString()}</p>
                                    <p className="card-text"><strong>👥 Seats:</strong> {event.freeSeats}/{event.maxParticipants}</p>

                                    {view === "all" && (
                                        <button className="btn btn-sm btn-success mt-2" onClick={() => attendEvent(event.id)}>Attend</button>
                                    )}

                                    {view === "my" && (
                                        <>
                                            <button className="btn btn-sm btn-warning mt-2 me-2" onClick={() => setEditingEvent(event)}>Modify</button>
                                            <button className="btn btn-sm btn-danger mt-2" onClick={() => deleteEvent(event.id)}>Delete Event</button>
                                        </>
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
    );
};

export default EventsPage;
