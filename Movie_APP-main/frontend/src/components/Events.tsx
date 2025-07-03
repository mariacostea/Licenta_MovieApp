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
            const res = await fetch(url, { headers: token ? { Authorization: `Bearer ${token}` } : {} });
            const data: ApiResponse<Event[]> = await res.json();
            if (!res.ok) throw new Error(data as unknown as string);
            setEvents(data.result);
        } catch (err) {
            alert("Error fetching events.");
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => { fetchEvents(view); }, [view]);

    const attendEvent = async (eventId: string) => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in!");
        try {
            const res = await fetch(`https://licenta-backend-nf1m.onrender.com/api/Event/attend/${eventId}`, {
                method: "POST", headers: { Authorization: `Bearer ${token}` }
            });
            if (!res.ok) throw new Error();
            alert("Successfully joined the event!");
            fetchEvents(view);
        } catch {
            alert("Error joining event.");
        }
    };

    const deleteEvent = async (eventId: string) => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in!");
        if (!window.confirm("Are you sure you want to delete this event?")) return;

        try {
            const res = await fetch(`https://licenta-backend-nf1m.onrender.com/api/Event/delete/${eventId}`, {
                method: "DELETE", headers: { Authorization: `Bearer ${token}` }
            });
            if (!res.ok) throw new Error();
            alert("Event deleted.");
            fetchEvents(view);
        } catch {
            alert("Error deleting event.");
        }
    };

    const updateEvent = async (updated: Event) => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in!");

        try {
            const res = await fetch(`https://licenta-backend-nf1m.onrender.com/api/Event/update/${updated.id}`, {
                method: "PUT", headers: {
                    "Content-Type": "application/json", Authorization: `Bearer ${token}`
                },
                body: JSON.stringify(updated)
            });
            if (!res.ok) throw new Error();
            alert("Event updated.");
            fetchEvents(view);
        } catch {
            alert("Error updating event.");
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
            if (!res.ok) throw new Error();
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
            <div className="bg-dark py-3 border-bottom shadow" style={{ position: "sticky", top: 0, zIndex: 1050 }}>
                <div className="d-flex justify-content-between align-items-center flex-wrap px-4">
                    <div className="d-flex flex-wrap gap-3">
                        <a href="/movies" className="btn btn-outline-light btn-sm">🎬 Movies</a>
                        <a href="/recommendation" className="btn btn-outline-light btn-sm">⭐ Recommendations</a>
                        <a href="/feed" className="btn btn-outline-light btn-sm">📰 Feed</a>
                        <a href="/people" className="btn btn-outline-light btn-sm">👥 People</a>
                        <button className="btn btn-outline-danger btn-sm" onClick={() => {
                            localStorage.removeItem('token');
                            localStorage.removeItem('userId');
                            window.location.href = '/login';
                        }}>🚪 Logout</button>
                    </div>
                    <a href="/profile" className="btn btn-secondary btn-sm">Profile</a>
                </div>
            </div>

            <div className="container py-4">
                <h2>🗓️ Events</h2>

                <div className="btn-group mb-4">
                    <button className={`btn btn-outline-primary ${view === "all" ? "active" : ""}`} onClick={() => setView("all")}>All Events</button>
                    <button className={`btn btn-outline-success ${view === "my" ? "active" : ""}`} onClick={() => setView("my")}>My Events</button>
                    <button className={`btn btn-outline-info ${view === "participation" ? "active" : ""}`} onClick={() => setView("participation")}>Participation Events</button>
                </div>

                {/* filtre + loading + carduri */}
                {/* păstrezi codul existent aici */}

                <div className="row g-3">
                    {events.map(ev => (
                        <div key={ev.id} className="col-6 col-sm-4 col-md-3" style={{ flex: '0 0 20%', maxWidth: '20%' }}>
                            <div className="card h-100 bg-dark text-white border-secondary shadow">
                                {/* conținutul cardului */}
                            </div>
                        </div>
                    ))}
                </div>

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
