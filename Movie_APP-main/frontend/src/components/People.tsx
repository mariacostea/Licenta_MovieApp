import React, { useEffect, useState } from "react";
import "bootstrap/dist/css/bootstrap.min.css";

interface User {
    id: string;
    username: string;
}

const People: React.FC = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [sentRequests, setSentRequests] = useState<string[]>([]);
    const [receivedRequests, setReceivedRequests] = useState<string[]>([]);
    const [friends, setFriends] = useState<string[]>([]);
    const [activeTab, setActiveTab] = useState("all");

    useEffect(() => {
        const token = localStorage.getItem("token");
        if (!token) return;

        const headers = { Authorization: `Bearer ${token}` };

        fetch("https://licenta-backend-nf1m.onrender.com/api/User/all", { headers })
            .then(res => res.json())
            .then(setUsers)
            .catch(console.error);

        fetch("https://licenta-backend-nf1m.onrender.com/api/Friends/sent", { headers })
            .then(res => res.json())
            .then(setSentRequests)
            .catch(console.error);

        fetch("https://licenta-backend-nf1m.onrender.com/api/Friends/received", { headers })
            .then(res => res.json())
            .then(setReceivedRequests)
            .catch(console.error);

        fetch("https://licenta-backend-nf1m.onrender.com/api/Friends/list", { headers })
            .then(res => res.json())
            .then(setFriends)
            .catch(console.error);
    }, []);

    const filteredUsers = () => {
        switch (activeTab) {
            case "sent": return users.filter(u => sentRequests.includes(u.id));
            case "received": return users.filter(u => receivedRequests.includes(u.id));
            case "friends": return users.filter(u => friends.includes(u.id));
            default: return users;
        }
    };

    return (
        <div className="container py-4 text-white">
            <div className="bg-dark py-2 border-bottom position-sticky top-0 z-3">
                <div className="container d-flex justify-content-center gap-3">
                    <a href="/recommendation" className="btn btn-outline-light btn-sm">⭐ Recommendations</a>
                    <a href="/events" className="btn btn-outline-light btn-sm">📅 Events</a>
                    <a href="/movies" className="btn btn-outline-light btn-sm">🎬 Movies</a>
                    <a href="/feed" className="btn btn-outline-light btn-sm">📰 Feed</a>
                    <button
                        className="btn btn-outline-danger btn-sm"
                        onClick={() => {
                            localStorage.removeItem('token');
                            localStorage.removeItem('userId');
                            window.location.href = '/login';
                        }}
                    >
                        🚪 Logout
                    </button>
                </div>
            </div>

            <h2 className="mb-4 mt-3">👥 People</h2>

            <div className="btn-group mb-3">
                <button className={`btn btn-outline-primary ${activeTab === "all" ? "active" : ""}`} onClick={() => setActiveTab("all")}>All Users</button>
                <button className={`btn btn-outline-warning ${activeTab === "sent" ? "active" : ""}`} onClick={() => setActiveTab("sent")}>Sent Requests</button>
                <button className={`btn btn-outline-info ${activeTab === "received" ? "active" : ""}`} onClick={() => setActiveTab("received")}>Received Requests</button>
                <button className={`btn btn-outline-success ${activeTab === "friends" ? "active" : ""}`} onClick={() => setActiveTab("friends")}>Friends</button>
            </div>

            <div className="row g-3">
                {filteredUsers().map(user => (
                    <div className="col-md-4" key={user.id}>
                        <div className="card bg-dark text-white border-secondary h-100">
                            <div className="card-body">
                                <h5 className="card-title">👤 {user.username}</h5>
                                <button className="btn btn-sm btn-outline-primary mt-2">Add Friend</button>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default People;
