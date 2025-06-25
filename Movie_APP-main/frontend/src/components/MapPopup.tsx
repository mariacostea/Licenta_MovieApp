import React, { useEffect, useRef } from "react";

interface MapPopupProps {
    anchorRef: React.RefObject<HTMLElement>;
    onClose: () => void;
    onLocationSelect: (location: string) => void;
}

const MapPopup: React.FC<MapPopupProps> = ({ anchorRef, onClose, onLocationSelect }) => {
    const popupRef = useRef<HTMLDivElement>(null);
    const mapRef = useRef<HTMLDivElement>(null);
    const positionRef = useRef({ top: 0, left: 0 });

    useEffect(() => {
        if (anchorRef.current) {
            const rect = anchorRef.current.getBoundingClientRect();
            positionRef.current = {
                top: rect.bottom + window.scrollY + 4,
                left: rect.left + window.scrollX,
            };
        }
    }, [anchorRef]);

    useEffect(() => {
        if (!mapRef.current || !window.google) return;

        const defaultCoords = { lat: 44.4328, lng: 26.1043 };
        const map = new google.maps.Map(mapRef.current, {
            center: defaultCoords,
            zoom: 12,
        });

        const marker = new google.maps.Marker({
            position: defaultCoords,
            map,
        });

        map.addListener("click", (e: google.maps.MapMouseEvent) => {
            const latLng = e.latLng;
            if (!latLng) return;

            marker.setPosition(latLng);

            const geocoder = new google.maps.Geocoder();
            geocoder.geocode({ location: latLng }, (results, status) => {
                if (status === "OK" && results && results[0]) {
                    onLocationSelect(results[0].formatted_address);
                } else {
                    onLocationSelect(`Lat: ${latLng.lat()}, Lng: ${latLng.lng()}`);
                }
            });
        });
    }, []);

    return (
        <div
            ref={popupRef}
            className="rounded shadow"
            style={{
                position: "absolute",
                top: positionRef.current.top,
                left: positionRef.current.left,
                zIndex: 2000,
                width: 400,
                height: 320,
                backgroundColor: "white",
                padding: "1rem",
                boxShadow: "0 0 10px rgba(0,0,0,0.3)",
            }}
        >
            <div className="d-flex justify-content-between align-items-center mb-2">
                <strong>Select Location</strong>
                <button onClick={onClose} className="btn btn-sm btn-outline-secondary">Close</button>
            </div>

            <div ref={mapRef} style={{ height: 240, width: "100%", borderRadius: 8 }} />
        </div>
    );
};

export default MapPopup;
