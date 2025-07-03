import React, { useRef, useState } from "react";
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

    return (
        <div
            className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center bg-dark bg-opacity-75"
            style={{ zIndex: 1050 }}
        >
            <div
                className="bg-white text-dark p-3 rounded position-relative"
                style={{ width: "fit-content", maxWidth: 400, overflow: "visible" }}
            >
                <h5 className="mb-3">Edit Event</h5>

                <input
                    className="form-control mb-2"
                    value={title}
                    onChange={e => setTitle(e.target.value)}
                    placeholder="Title"
                />

                <textarea
                    className="form-control mb-2"
                    value={description}
                    onChange={e => setDescription(e.target.value)}
                    placeholder="Description"
                />

                <p className="mb-2"><strong>🎮 Movie:</strong> {event.movieTitle ?? "Unknown"}</p>

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
                        <div style={{ position: "absolute", top: "100%", left: 0, zIndex: 1060 }}>
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

                <DatePicker
                    selected={date}
                    onChange={(date: Date | null) => date && setDate(date)}
                    showTimeSelect
                    timeFormat="HH:mm"
                    timeIntervals={15}
                    dateFormat="Pp"
                    minDate={minDate}
                    className="form-control mb-2"
                    popperContainer={({ children }) => (
                        <div style={{ zIndex: 9999, position: "absolute" }}>{children}</div>
                    )}
                />

                <input
                    type="number"
                    className="form-control mb-3"
                    value={maxParticipants}
                    onChange={e => setMaxParticipants(Math.max(1, Number(e.target.value)))}
                    min={1}
                />

                <div className="d-flex justify-content-between">
                    <button className="btn btn-secondary" onClick={onClose}>Cancel</button>
                    <button
                        className="btn btn-primary"
                        onClick={() =>
                            onSave({
                                ...event,
                                title,
                                description,
                                location,
                                date: date.toISOString(),
                                maxParticipants,
                            })
                        }
                    >
                        Save
                    </button>
                </div>
            </div>
        </div>
    );
};

export default EditPopup;
