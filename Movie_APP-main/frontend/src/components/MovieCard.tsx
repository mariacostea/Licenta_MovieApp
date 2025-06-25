import React, { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

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
                                                 id, title, year, averageRating, genres, posterUrl,
                                                 isWatched: initialIsWatched, onMarked
                                             }) => {
    const navigate = useNavigate();
    const [isWatched, setIsWatched] = useState(initialIsWatched ?? false);
    const [showEventForm, setShowEventForm] = useState(false);
    const [eventLocation, setEventLocation] = useState("");
    const [eventDate, setEventDate] = useState<Date | null>(null);
    const [eventParticipants, setEventParticipants] = useState(1);
    const [eventDescription, setEventDescription] = useState("");
    const mapRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        setIsWatched(initialIsWatched ?? false);
    }, [initialIsWatched]);

    const markAsWatched = async () => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in.");

        try {
            const res = await fetch("https://licenta-backend-nf1m.onrender.com/api/UserMovie/MarkAsWatched", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ title, year }),
            });

            if (!res.ok) throw new Error("Error marking movie as watched.");
            setIsWatched(true);
            onMarked?.(id);
        } catch (err) {
            alert((err as Error).message);
        }
    };

    const createEvent = async () => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in.");

        if (!eventDate || !eventLocation.trim()) {
            return alert("Complete all fields, including a valid location and future date.");
        }

        try {
            const res = await fetch("https://licenta-backend-nf1m.onrender.com/api/Event/create", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({
                    title,
                    description: eventDescription,
                    location: eventLocation,
                    date: eventDate.toISOString(),
                    maxParticipants: Math.max(eventParticipants, 1),
                    movieId: id,
                }),
            });

            if (!res.ok) throw new Error("Failed to create event.");
            alert("Event created successfully!");
            setShowEventForm(false);
        } catch (err) {
            alert((err as Error).message);
        }
    };

    const minDate = new Date();
    minDate.setDate(minDate.getDate() + 2);

    useEffect(() => {
        if (!showEventForm || !mapRef.current || !window.google) return;

        const defaultCoords = { lat: 44.4328, lng: 26.1043 };

        const map = new google.maps.Map(mapRef.current, {
            center: defaultCoords,
            zoom: 12,
        });

        const marker = new google.maps.Marker({
            position: defaultCoords,
            map,
        });

        map.addListener("click", async (e: google.maps.MapMouseEvent) => {
            const latLng = e.latLng;
            if (!latLng) return;

            marker.setPosition(latLng);

            const geocoder = new google.maps.Geocoder();
            geocoder.geocode({ location: latLng }, (results, status) => {
                if (status === "OK" && results && results[0]) {
                    setEventLocation(results[0].formatted_address);
                } else {
                    setEventLocation(`Lat: ${latLng.lat()}, Lng: ${latLng.lng()}`);
                }
            });
        });
    }, [showEventForm]);

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
                        className={`btn btn-sm ${isWatched ? "btn-success" : "btn-outline-success"}`}
                        onClick={markAsWatched}
                        disabled={isWatched}
                    >
                        {isWatched ? "Watched" : "Mark as Watched"}
                    </button>
                    <button className="btn btn-outline-warning btn-sm" onClick={() => setShowEventForm(!showEventForm)}>
                        Create Event
                    </button>
                </div>

                {showEventForm && (
                    <div className="mt-3 border-top pt-3">
                        <h6>Create Event</h6>

                        <label className="form-label">Date & Time</label>
                        <DatePicker
                            selected={eventDate}
                            onChange={(date) => setEventDate(date)}
                            showTimeSelect
                            dateFormat="MMMM d, yyyy h:mm aa"
                            timeIntervals={15}
                            timeCaption="Time"
                            minDate={minDate}
                            className="form-control mb-2"
                            placeholderText="Click to select date and time"
                        />

                        <label className="form-label">Location</label>
                        <input
                            type="text"
                            className="form-control mb-2"
                            value={eventLocation}
                            readOnly
                            placeholder="Click on the map to select location"
                        />

                        <div ref={mapRef} style={{ width: "100%", height: 300, borderRadius: 8, marginBottom: 10 }} />

                        <label className="form-label">Max Participants</label>
                        <input
                            type="number"
                            className="form-control mb-2"
                            value={eventParticipants}
                            min={1}
                            onChange={(e) => setEventParticipants(Math.max(1, Number(e.target.value)))}
                        />

                        <label className="form-label">Description</label>
                        <textarea
                            className="form-control mb-2"
                            value={eventDescription}
                            onChange={(e) => setEventDescription(e.target.value)}
                        />

                        <div className="d-flex justify-content-between">
                            <button className="btn btn-secondary" onClick={() => setShowEventForm(false)}>Cancel</button>
                            <button className="btn btn-primary" onClick={createEvent}>Submit Event</button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
};

export default MovieCard;
