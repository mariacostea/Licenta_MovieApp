import React, { useState } from "react";
import { MapContainer, TileLayer, Marker, useMapEvents } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";

interface MapPopupProps {
    onClose: () => void;
    onLocationSelect: (location: string) => void;
}

const LocationSelector: React.FC<{ onSelect: (coords: L.LatLng) => void }> = ({ onSelect }) => {
    useMapEvents({
        click(e) {
            onSelect(e.latlng);
        },
    });
    return null;
};

const MapPopup: React.FC<MapPopupProps> = ({ onClose, onLocationSelect }) => {
    const [selectedPosition, setSelectedPosition] = useState<L.LatLng | null>(null);

    const handleSelect = async (coords: L.LatLng) => {
        setSelectedPosition(coords);
        try {
            const res = await fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${coords.lat}&lon=${coords.lng}`);
            const data = await res.json();
            onLocationSelect(data.display_name || `Lat: ${coords.lat}, Lng: ${coords.lng}`);
        } catch {
            onLocationSelect(`Lat: ${coords.lat}, Lng: ${coords.lng}`);
        }
    };

    return (
        <div
            style={{
                position: "fixed",
                top: 0,
                left: 0,
                width: "100vw",
                height: "100vh",
                backgroundColor: "rgba(0,0,0,0.6)",
                zIndex: 2000,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
            }}
        >
            <div
                style={{
                    background: "white",
                    padding: "1rem",
                    borderRadius: "10px",
                    width: "90%",
                    maxWidth: "600px",
                }}
            >
                <div className="d-flex justify-content-between align-items-center mb-2">
                    <strong>Select Location</strong>
                    <button onClick={onClose} className="btn btn-sm btn-outline-secondary">Close</button>
                </div>

                <MapContainer
                    center={[44.4328, 26.1043]}
                    zoom={13}
                    style={{ height: "300px", width: "100%" }}
                >
                    <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
                    <LocationSelector onSelect={handleSelect} />
                    {selectedPosition && <Marker position={selectedPosition} />}
                </MapContainer>
            </div>
        </div>
    );
};

export default MapPopup;
