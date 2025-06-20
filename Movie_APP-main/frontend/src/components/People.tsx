﻿import React, { useEffect, useState } from "react";

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
}

type Tab = "all" | "pendingSent" | "pendingReceived" | "friends";
const API = "http://localhost:5000/api";

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
    const [pendingReceived, setPendingReceived] = useState<Friendship[]>([]); // Important

    const fetchUsers = async (url: string, setter: Function, label: string) => {
        try {
            const res = await fetch(url, { headers: auth });
            const json = await res.json();
            const list = Array.isArray(json) ? json : json.response ?? json.result ?? [];
            console.log(`Loaded ${label}:`, list);
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

    return (
        <div style={{ padding: 24 }}>
            <h2>👥 People</h2>
            <div className="btn-group mb-3">
                <button className={`btn btn-outline-primary ${tab === "all" && "active"}`} onClick={() => setTab("all")}>
                    All Users
                </button>
                <button className={`btn btn-outline-warning ${tab === "pendingSent" && "active"}`} onClick={() => setTab("pendingSent")}>
                    Sent Requests
                </button>
                <button className={`btn btn-outline-info ${tab === "pendingReceived" && "active"}`} onClick={() => setTab("pendingReceived")}>
                    Received Requests
                </button>
                <button className={`btn btn-outline-success ${tab === "friends" && "active"}`} onClick={() => setTab("friends")}>
                    Friends
                </button>
            </div>

            <ul>
                {tab === "all" &&
                    users
                        .filter((u) => !isKnown(u.id))
                        .map((u) => (
                            <li key={u.id}>
                                {u.name}{" "}
                                <button className="btn btn-sm btn-primary" onClick={() => sendRequest(u.id)}>
                                    Add Friend
                                </button>
                            </li>
                        ))}

                {tab === "pendingSent" &&
                    (pendingSent.length > 0 ? (
                        pendingSent.map((u) => <li key={u.id}>{u.name} (Pending)</li>)
                    ) : (
                        <li>No pending sent requests</li>
                    ))}

                {tab === "pendingReceived" &&
                    (pendingReceived.length > 0 ? (
                        pendingReceived.map((f) => (
                            <li key={f.id}>
                                {f.requesterName}{" "}
                                <button className="btn btn-sm btn-success me-1" onClick={() => acceptRequest(f.id)}>
                                    Accept
                                </button>
                                <button className="btn btn-sm btn-danger" onClick={() => rejectRequest(f.id)}>
                                    Reject
                                </button>
                            </li>
                        ))
                    ) : (
                        <li>No received friend requests</li>
                    ))}

                {tab === "friends" &&
                    (friends.length > 0 ? (
                        friends.map((u) => <li key={u.id}>{u.name}</li>)
                    ) : (
                        <li>No friends</li>
                    ))}
            </ul>
        </div>
    );
}
