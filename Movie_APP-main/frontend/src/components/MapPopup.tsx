import React, { useEffect, useRef, useState } from "react";
import { MapContainer, TileLayer, Marker, useMapEvents } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";

interface MapPopupProps {
    anchorRef: React.RefObject<HTMLElement>;
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

const MapPopup: React.FC<MapPopupProps> = ({ anchorRef, onClose, onLocationSelect }) => {
    const popupRef = useRef<HTMLDivElement>(null);
    const [position, setPosition] = useState({ top: 0, left: 0 });
    const [selectedCoords, setSelectedCoords] = useState<L.LatLng | null>(null);

    useEffect(() => {
        if (anchorRef.current) {
            const rect = anchorRef.current.getBoundingClientRect();
            setPosition({
                top: rect.bottom + window.scrollY + 6,
                left: rect.left + window.scrollX,
            });
        }
    }, [anchorRef]);

    const handleSelect = async (coords: L.LatLng) => {
        setSelectedCoords(coords);
        try {
            const res = await fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${coords.lat}&lon=${coords.lng}`);
            const data = await res.json();
            onLocationSelect(data.display_name || `Lat: ${coords.lat}, Lng: ${coords.lng}`);
            onClose();
        } catch {
            onLocationSelect(`Lat: ${coords.lat}, Lng: ${coords.lng}`);
            onClose();
        }
    };

    return (
        <div
            ref={popupRef}
            style={{
                position: "absolute",
                top: position.top,
                left: position.left,
                zIndex: 2000,
                width: "400px",
                height: "300px",
                backgroundColor: "white",
                boxShadow: "0 0 10px rgba(0,0,0,0.25)",
                borderRadius: "8px",
                padding: "0.5rem",
            }}
        >
            <div className="d-flex justify-content-between align-items-center mb-2">
                <strong>Select Location</strong>
                <button className="btn btn-sm btn-outline-secondary" onClick={onClose}>Close</button>
            </div>
            <MapContainer
                center={[44.4328, 26.1043]}
                zoom={13}
                style={{ width: "100%", height: "230px" }}
            >
                <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
                <LocationSelector onSelect={handleSelect} />
                {selectedCoords && <Marker position={selectedCoords} />}
            </MapContainer>
        </div>
    );
};

export default MapPopup;
