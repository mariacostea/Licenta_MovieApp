import React, { useEffect, useState } from "react";
import TopNav    from "./TopNav";
interface User {
    id: string;
    name: string;
    email?: string;
    role?: string;
    profilePictureUrl?: string;
}

interface Friendship {
    id: string;
    requesterId: string;
    requesterName: string;
    requesterProfilePictureUrl?: string;
}

type Tab = "all" | "pendingSent" | "pendingReceived" | "friends";
const API = "https://licenta-backend-nf1m.onrender.com/api";

export default function People() {
    const token = localStorage.getItem("token");
    const userId = localStorage.getItem("userId");

    if (!token || !userId) {
        alert("Not logged-in");
        return null;
    }

    const auth = { Authorization: `Bearer ${token}` };

    const [tab, setTab] = useState<Tab>("all");
    const [users, setUsers] = useState<User[]>([]);
    const [pendingSent, setPendingSent] = useState<User[]>([]);
    const [friends, setFriends] = useState<User[]>([]);
    const [pendingReceived, setPendingReceived] = useState<Friendship[]>([]);

    const fetchUsers = async (url: string, setter: Function, label: string) => {
        try {
            const res = await fetch(url, { headers: auth });
            const json = await res.json();
            const list = Array.isArray(json) ? json : json.response ?? json.result ?? [];
            setter(list);
        } catch (err) {
            console.error(`Error loading ${label}`, err);
        }
    };

    const loadAvailable = () =>
        fetchUsers(`${API}/User/GetAvailableUsers/available/${userId}`, setUsers, "users");

    const loadFriends = () =>
        fetchUsers(`${API}/Friendship/users/friends/${userId}`, setFriends, "friends");

    const loadSent = () =>
        fetchUsers(`${API}/Friendship/users/pending/sent/${userId}`, setPendingSent, "pendingSent");

    const loadReceived = () =>
        fetchUsers(`${API}/Friendship/list/received/${userId}`, setPendingReceived, "pendingReceived");

    useEffect(() => {
        if (tab === "all") loadAvailable();
        if (tab === "pendingSent") loadSent();
        if (tab === "pendingReceived") loadReceived();
        if (tab === "friends") loadFriends();
    }, [tab]);

    const sendRequest = async (toUserId: string) => {
        await fetch(`${API}/Friendship/request`, {
            method: "POST",
            headers: { ...auth, "Content-Type": "application/json" },
            body: JSON.stringify({ toUserId }),
        });

        loadAvailable();
        loadSent();
    };

    const acceptRequest = async (friendshipId: string) => {
        await fetch(`${API}/Friendship/accept/${friendshipId}`, {
            method: "POST",
            headers: auth,
        });
        loadReceived();
        loadFriends();
    };

    const rejectRequest = async (friendshipId: string) => {
        await fetch(`${API}/Friendship/reject/${friendshipId}`, {
            method: "POST",
            headers: auth,
        });
        loadReceived();
    };

    const isKnown = (id: string) =>
        friends.some((u) => u.id === id) ||
        pendingSent.some((u) => u.id === id) ||
        pendingReceived.some((f) => f.requesterId === id);

    const renderUser = (
        user: { id: string; name: string; profilePictureUrl?: string },
        extra?: React.ReactNode
    ) => (
        <li key={user.id} className="mb-3 d-flex align-items-center gap-3">
            {user.profilePictureUrl ? (
                <img
                    src={user.profilePictureUrl}
                    alt="profile"
                    style={{ width: 60, height: 60, borderRadius: "50%", objectFit: "cover" }}
                />
            ) : (
                <div
                    style={{
                        width: 60,
                        height: 60,
                        borderRadius: "50%",
                        backgroundColor: "#444",
                        display: "inline-block"
                    }}
                />
            )}
            <span>{user.name}</span>
            {extra && <div className="ms-auto">{extra}</div>}
        </li>
    );

    return (
        <div style={{ minHeight: "100vh", backgroundColor: "#111", color: "white" }}>
            <TopNav />
            <div
                className="bg-dark py-3 border-bottom shadow"
                style={{ position: "sticky", top: 0, width: "100%", zIndex: 1050 }}
            >
                <div className="d-flex justify-content-center flex-wrap gap-3 px-4">
                    <a href="/recommendation" className="btn btn-outline-light btn-sm">⭐ Recommendations</a>
                    <a href="/events" className="btn btn-outline-light btn-sm">📅 Events</a>
                    <a href="/feed" className="btn btn-outline-light btn-sm">📰 Feed</a>
                    <a href="/movies" className="btn btn-outline-light btn-sm">🎬 Movies</a>
                    <button
                        className="btn btn-outline-danger btn-sm"
                        onClick={() => {
                            localStorage.removeItem("token");
                            localStorage.removeItem("userId");
                            window.location.href = "/login";
                        }}
                    >
                        🚪 Logout
                    </button>
                </div>
            </div>

            <div className="container py-4">
                <h2 className="mb-4">👥 People</h2>

                <div className="btn-group mb-4">
                    <button className={`btn btn-outline-primary ${tab === "all" && "active"}`} onClick={() => setTab("all")}>All Users</button>
                    <button className={`btn btn-outline-warning ${tab === "pendingSent" && "active"}`} onClick={() => setTab("pendingSent")}>Sent Requests</button>
                    <button className={`btn btn-outline-info ${tab === "pendingReceived" && "active"}`} onClick={() => setTab("pendingReceived")}>Received Requests</button>
                    <button className={`btn btn-outline-success ${tab === "friends" && "active"}`} onClick={() => setTab("friends")}>Friends</button>
                </div>

                <ul className="list-unstyled">
                    {tab === "all" &&
                        users.filter((u) => !isKnown(u.id)).map((u) =>
                            renderUser(u, (
                                <button className="btn btn-sm btn-primary" onClick={() => sendRequest(u.id)}>
                                    Add Friend
                                </button>
                            ))
                        )}

                    {tab === "pendingSent" &&
                        (pendingSent.length > 0 ? (
                            pendingSent.map((u) =>
                                renderUser(u, <span className="text-muted">(Pending)</span>)
                            )
                        ) : (
                            <li>No pending sent requests</li>
                        ))}

                    {tab === "pendingReceived" &&
                        (pendingReceived.length > 0 ? (
                            pendingReceived.map((f) =>
                                renderUser(
                                    {
                                        id: f.requesterId,
                                        name: f.requesterName,
                                        profilePictureUrl: f.requesterProfilePictureUrl
                                    },
                                    <>
                                        <button className="btn btn-sm btn-success me-1" onClick={() => acceptRequest(f.id)}>
                                            Accept
                                        </button>
                                        <button className="btn btn-sm btn-danger" onClick={() => rejectRequest(f.id)}>
                                            Reject
                                        </button>
                                    </>
                                )
                            )
                        ) : (
                            <li>No received friend requests</li>
                        ))}

                    {tab === "friends" &&
                        (friends.length > 0 ? (
                            friends.map((u) => renderUser(u))
                        ) : (
                            <li>No friends</li>
                        ))}
                </ul>
            </div>
        </div>
    );
}
