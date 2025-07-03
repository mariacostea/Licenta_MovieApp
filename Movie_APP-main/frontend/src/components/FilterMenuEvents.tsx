import React, { useState } from "react";

interface FilterMenuProps {
    onApply: (filter: { type: string; value: string }) => void;
}

const FilterMenuEvents: React.FC<FilterMenuProps> = ({ onApply }) => {
    const [filterType, setFilterType] = useState("location");
    const [filterValue, setFilterValue] = useState("");

    const applyFilter = () => {
        if (!filterValue.trim()) {
            alert("Please enter a value");
            return;
        }
        onApply({ type: filterType, value: filterValue });
    };

    return (
        <div className="d-flex gap-2 flex-wrap">
            <select
                className="form-select w-auto"
                value={filterType}
                onChange={(e) => setFilterType(e.target.value)}
            >
                <option value="location">📍 Location</option>
                <option value="day">📅 Day (YYYY-MM-DD)</option>
                <option value="full-date">⏱️ Full DateTime</option>
                <option value="month">🗓️ Month (YYYY-MM)</option>
                <option value="movie">🎬 Movie Title</option>
            </select>

            <input
                type="text"
                className="form-control w-auto"
                placeholder="Enter value"
                value={filterValue}
                onChange={(e) => setFilterValue(e.target.value)}
            />

            <button className="btn btn-secondary" onClick={applyFilter}>
                Apply Filter
            </button>
        </div>
    );
};

export default FilterMenuEvents;
