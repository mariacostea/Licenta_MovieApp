﻿import React, { useRef, useState } from "react";
import MapPopup from "./MapPopup";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

interface EditPopupProps {
    event: any;
    onClose: () => void;
    onSave: (updated: any) => void;
}

const EditPopup: React.FC<EditPopupProps> = ({ event, onClose, onSave }) => {
    const [title, setTitle] = useState(event.title);
    const [description, setDescription] = useState(event.description);
    const [location, setLocation] = useState(event.location);
    const [date, setDate] = useState(new Date(event.date));
    const [maxParticipants, setMaxParticipants] = useState(event.maxParticipants);
    const [showMap, setShowMap] = useState(false);

    const locationRef = useRef<HTMLInputElement>(null);
    const minDate = new Date();
    minDate.setDate(minDate.getDate() + 2);

    const handleSave = () => {
        const participantsEnrolled = event.maxParticipants - event.freeSeats;

        if (maxParticipants < participantsEnrolled) {
            alert(
                `Cannot set max participants to ${maxParticipants} because there are already ${participantsEnrolled} participants enrolled.`
            );
            return;
        }

        const now = new Date();
        const minFutureDate = new Date(now);
        minFutureDate.setDate(now.getDate() + 2);

        if (date < minFutureDate) {
            alert(`The event date must be at least 2 days in the future.`);
            return;
        }

        onSave({
            ...event,
            title,
            description,
            location,
            date: date.toISOString(),
            maxParticipants,
        });
    };

    return (
        <div
            className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center bg-dark bg-opacity-75"
            style={{ zIndex: 1050 }}
        >
            <div
                className="bg-white text-dark p-3 rounded position-relative"
                style={{
                    width: "fit-content",
                    maxWidth: 400,
                    overflow: "visible",
                }}
            >
                <h5 className="mb-3">Edit Event</h5>
                <input
                    className="form-control mb-2"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    placeholder="Title"
                />
                <textarea
                    className="form-control mb-2"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder="Description"
                />
                <p className="mb-2">
                    <strong>🎮 Movie:</strong> {event.movieTitle ?? "Unknown"}
                </p>

                <div style={{ position: "relative" }}>
                    <input
                        ref={locationRef}
                        className="form-control mb-2"
                        value={location}
                        placeholder="Click to choose location"
                        onClick={() => setShowMap(true)}
                        readOnly
                    />

                    {showMap && (
                        <div
                            style={{
                                position: "fixed",
                                top: "50%",
                                left: "50%",
                                transform: "translate(-50%, -50%)",
                                zIndex: 2000,
                                background: "white",
                                padding: "1rem",
                                borderRadius: "0.5rem",
                                boxShadow: "0 0 10px rgba(0,0,0,0.5)",
                            }}
                        >
                            <MapPopup
                                anchorRef={locationRef}
                                onClose={() => setShowMap(false)}
                                onLocationSelect={(loc) => {
                                    setLocation(loc);
                                    setShowMap(false);
                                }}
                            />
                        </div>
                    )}
                </div>

                <div style={{ zIndex: 2000 }}>
                    <DatePicker
                        selected={date}
                        onChange={(date: Date | null) => date && setDate(date)}
                        showTimeSelect
                        timeFormat="HH:mm"
                        timeIntervals={15}
                        dateFormat="Pp"
                        minDate={minDate}
                        className="form-control mb-2"
                        calendarClassName="custom-datepicker"
                        withPortal
                    />
                </div>

                <input
                    type="number"
                    className="form-control mb-3"
                    value={maxParticipants}
                    onChange={(e) => setMaxParticipants(Math.max(1, Number(e.target.value)))}
                    min={1}
                />

                <div className="d-flex justify-content-between">
                    <button className="btn btn-secondary" onClick={onClose}>
                        Cancel
                    </button>
                    <button className="btn btn-primary" onClick={handleSave}>
                        Save
                    </button>
                </div>
            </div>
        </div>
    );
};

export default EditPopup;
