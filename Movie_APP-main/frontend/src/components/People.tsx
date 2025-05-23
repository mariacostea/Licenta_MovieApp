import React, { useEffect, useState } from "react";

interface User {
    id: string;
    name: string;
    profilePictureUrl?: string;
}

interface Friendship {
    id: string;
    requesterId: string;
    requesterName: string;
    addresseeId: string;
    addresseeName: string;
    status: "Pending" | "Accepted" | "Rejected";
}

const People: React.FC = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [friendships, setFriendships] = useState<Friendship[]>([]);
    const [tab, setTab] = useState<"all" | "pendingSent" | "pendingReceived" | "friends">("all");

    const currentUserId = localStorage.getItem("userId")?.toLowerCase().trim() ?? "";

    const loadUsers = async () => {
        const token = localStorage.getItem("token");
        const res = await fetch("http://localhost:5000/api/User/GetAll", {
            headers: { Authorization: `Bearer ${token}` },
        });
        const json = await res.json();
        const allUsers: User[] = json.response ?? [];
        setUsers(allUsers.filter(u => u.id.toLowerCase().trim() !== currentUserId));
    };

    const loadFriendships = async () => {
        const token = localStorage.getItem("token");
        const res = await fetch(`http://localhost:5000/api/Friendship/list/${currentUserId}`, {
            headers: { Authorization: `Bearer ${token}` },
        });
        const json = await res.json();
        setFriendships(json.result ?? []);
    };

    const reload = async () => {
        await Promise.all([loadUsers(), loadFriendships()]);
    };

    useEffect(() => {
        reload();
    }, []);

    const sendRequest = async (toUserId: string) => {
        const token = localStorage.getItem("token");
        try {
            const res = await fetch("http://localhost:5000/api/Friendship/request", {
                method: "POST",
                headers: {
                    Authorization: `Bearer ${token}`,
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ toUserId }),
            });

            if (!res.ok) {
                const data = await res.json().catch(() => null);
                const message = data?.error?.message || data?.title || "Unknown error.";
                alert("Error: " + message);
            } else {
                await reload();
            }
        } catch (err) {
            alert("Network error. Check backend or internet connection.");
        }
    };

    const acceptRequest = async (friendshipId: string) => {
        const token = localStorage.getItem("token");
        await fetch(`http://localhost:5000/api/Friendship/accept/${friendshipId}`, {
            method: "POST",
            headers: { Authorization: `Bearer ${token}` },
        });
        await reload();
    };

    const rejectRequest = async (friendshipId: string) => {
        const token = localStorage.getItem("token");
        await fetch(`http://localhost:5000/api/Friendship/reject/${friendshipId}`, {
            method: "POST",
            headers: { Authorization: `Bearer ${token}` },
        });
        await reload();
    };

    const getFriendship = (userId: string): Friendship | undefined =>
        friendships.find(f =>
            (f.requesterId.toLowerCase() === currentUserId && f.addresseeId.toLowerCase() === userId.toLowerCase()) ||
            (f.addresseeId.toLowerCase() === currentUserId && f.requesterId.toLowerCase() === userId.toLowerCase())
        );

    const renderUsers = () =>
        users
            .filter(u => !getFriendship(u.id))
            .map(u => (
                <li key={u.id}>
                    {u.name} <button onClick={() => sendRequest(u.id)}>Add Friend</button>
                </li>
            ));

    const renderPendingSent = () =>
        friendships
            .filter(f => f.status === "Pending" && f.requesterId.toLowerCase() === currentUserId)
            .map(f => <li key={f.id}>{f.addresseeName} (Pending)</li>);

    const renderPendingReceived = () =>
        friendships
            .filter(f => f.status === "Pending" && f.addresseeId.toLowerCase() === currentUserId)
            .map(f => (
                <li key={f.id}>
                    {f.requesterName}
                    <button onClick={() => acceptRequest(f.id)}>Accept</button>
                    <button onClick={() => rejectRequest(f.id)}>Reject</button>
                </li>
            ));

    const renderFriends = () =>
        friendships
            .filter(f => f.status === "Accepted")
            .map(f => (
                <li key={f.id}>
                    {f.requesterId.toLowerCase() === currentUserId
                        ? f.addresseeName
                        : f.requesterName}
                </li>
            ));

    return (
        <div style={{ padding: "20px" }}>
            <h2>👥 People</h2>
            <div className="btn-group mb-4">
                <button className={`btn btn-outline-primary ${tab === "all" ? "active" : ""}`} onClick={() => setTab("all")}>All Users</button>
                <button className={`btn btn-outline-warning ${tab === "pendingSent" ? "active" : ""}`} onClick={() => setTab("pendingSent")}>Sent Requests</button>
                <button className={`btn btn-outline-info ${tab === "pendingReceived" ? "active" : ""}`} onClick={() => setTab("pendingReceived")}>Received Requests</button>
                <button className={`btn btn-outline-success ${tab === "friends" ? "active" : ""}`} onClick={() => setTab("friends")}>Friends</button>
            </div>

            <ul>
                {tab === "all" && renderUsers()}
                {tab === "pendingSent" && renderPendingSent()}
                {tab === "pendingReceived" && renderPendingReceived()}
                {tab === "friends" && renderFriends()}
            </ul>
        </div>
    );
};

export default People;
