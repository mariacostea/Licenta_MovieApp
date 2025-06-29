import React from "react";

const TopNav: React.FC = () => {
    return (
        <div
            className="bg-dark py-2 border-bottom position-sticky top-0 z-3"
            style={{ zIndex: 1050 }}
        >
            <div className="container d-flex justify-content-end">
                <a href="/profile" className="btn btn-secondary btn-sm">Profile</a>
            </div>
        </div>
    );
};

export default TopNav;
