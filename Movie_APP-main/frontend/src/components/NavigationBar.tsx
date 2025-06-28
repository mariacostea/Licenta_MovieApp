import React from "react";
import { Link } from "react-router-dom";

export interface NavigationBarProps {
    onSearch: (title: string) => void;
}

const NavigationBar: React.FC<NavigationBarProps> = ({ onSearch }) => {
    const handleSearch = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const form = e.target as HTMLFormElement;
        const input = form.elements.namedItem("search") as HTMLInputElement;
        onSearch(input.value);
    };

    return (
        <nav className="navbar navbar-dark bg-dark p-3">
            <Link to="/movies" className="navbar-brand">MovieApp</Link>
            <form onSubmit={handleSearch} className="d-flex">
                <input name="search" type="text" placeholder="Search title..." className="form-control me-2" />
                <button type="submit" className="btn btn-success">Search</button>
            </form>
            <div className="d-flex gap-2 ms-3">
                <Link to="/watched" className="btn btn-warning">Watched Movies</Link>
                <Link to="/recommended" className="btn btn-info">Recommended Movies</Link>
                <Link to="/profile" className="btn btn-secondary">Profile</Link>
            </div>
        </nav>
    );
};

export default NavigationBar;
