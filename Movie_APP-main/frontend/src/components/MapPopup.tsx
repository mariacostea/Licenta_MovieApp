import React, { useEffect, useRef, useState } from "react";

interface MapPopupProps {
    anchorRef: React.RefObject<HTMLElement>;
    onClose: () => void;
    onLocationSelect: (location: string) => void;
}

const MapPopup: React.FC<MapPopupProps> = ({ anchorRef, onClose, onLocationSelect }) => {
    const popupRef = useRef<HTMLDivElement>(null);
    const [position, setPosition] = useState({ top: 0, left: 0 });

    useEffect(() => {
        if (anchorRef.current) {
            const rect = anchorRef.current.getBoundingClientRect();
            setPosition({
                top: rect.bottom + window.scrollY + 4,
                left: rect.left + window.scrollX,
            });
        }
    }, [anchorRef]);
    
    const selectMockLocation = () => {
        onLocationSelect("Bulevardul Nicolae Bălcescu 35, București");
    };

    return (
        <div
            ref={popupRef}
            className="rounded shadow"
            style={{
                position: "absolute",
                top: position.top,
                left: position.left,
                zIndex: 2000,
                width: 400,
                height: 300,
                backgroundColor: "white",
                padding: "1rem",
                boxShadow: "0 0 10px rgba(0,0,0,0.3)",
            }}
        >
            <div className="d-flex justify-content-between align-items-center mb-2">
                <strong>Select Location</strong>
                <button onClick={onClose} className="btn btn-sm btn-outline-secondary">Close</button>
            </div>
            
            <div className="mb-2" style={{ height: "200px", background: "#eee" }}>
                <iframe
                    title="Map"
                    width="100%"
                    height="100%"
                    frameBorder="0"
                    src={`https://maps.google.com/maps?q=Bucuresti&t=&z=13&ie=UTF8&iwloc=&output=embed`}
                    allowFullScreen
                />
            </div>

            <button className="btn btn-primary w-100" onClick={selectMockLocation}>
                Selectează această locație
            </button>
        </div>
    );
};

export default MapPopup;
