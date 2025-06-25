import React, { useEffect, useRef, useState } from "react";
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
    const [selectedCoords, setSelectedCoords] = useState<{ lat: number; lng: number } | null>(null);
    const mapRef = useRef<HTMLDivElement>(null);

    const minDate = new Date();
    minDate.setDate(minDate.getDate() + 2);

    useEffect(() => {
        if (!window.google || !mapRef.current) return;

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
            setSelectedCoords({ lat: latLng.lat(), lng: latLng.lng() });

            const geocoder = new google.maps.Geocoder();
            geocoder.geocode({ location: latLng }, (results, status) => {
                if (status === "OK" && results && results[0]) {
                    setLocation(results[0].formatted_address);
                } else {
                    setLocation(`Lat: ${latLng.lat()}, Lng: ${latLng.lng()}`);
                }
            });
        });
    }, []);

    return (
        <div className="position-fixed top-0 start-0 w-100 h-100 bg-dark bg-opacity-75 d-flex align-items-center justify-content-center" style={{ zIndex: 1050 }}>
            <div className="bg-white text-dark p-4 rounded d-flex gap-4 flex-wrap justify-content-center" style={{ width: "95vw", maxHeight: "95vh", overflowY: "auto" }}>
                {/* FORM */}
                <div style={{ minWidth: 300, maxWidth: 400 }}>
                    <h5 className="mb-3">Edit Event</h5>
                    <input className="form-control mb-2" value={title} onChange={e => setTitle(e.target.value)} placeholder="Title" />
                    <textarea className="form-control mb-2" value={description} onChange={e => setDescription(e.target.value)} placeholder="Description" />
                    <p className="mb-2"><strong>🎮 Movie:</strong> {event.movieTitle ?? "Unknown"}</p>

                    <input
                        className="form-control mb-2"
                        value={location}
                        placeholder="Click on the map to choose location"
                        readOnly
                    />

                    <DatePicker
                        selected={date}
                        onChange={(date: Date | null) => date && setDate(date)}
                        showTimeSelect
                        timeFormat="HH:mm"
                        timeIntervals={15}
                        dateFormat="Pp"
                        minDate={minDate}
                        className="form-control mb-2"
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

                {/* MAP */}
                <div style={{ flex: 1, minWidth: 300 }}>
                    <div><strong>Click on map to select location</strong></div>
                    <div ref={mapRef} style={{ width: "100%", height: 400, borderRadius: 8, boxShadow: "0 0 10px rgba(0,0,0,0.2)" }} />
                </div>
            </div>
        </div>
    );
};

export default EditPopup;
