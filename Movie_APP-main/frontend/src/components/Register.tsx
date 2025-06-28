import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const Register: React.FC = () => {
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [profilePicture, setProfilePicture] = useState<File | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) setProfilePicture(file);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            let profilePictureUrl = "";
            
            if (profilePicture) {
                const formData = new FormData();
                formData.append("file", profilePicture);

                const uploadRes = await fetch("/api/User/upload-profile-picture", {
                    method: "POST",
                    body: formData,
                });

                if (!uploadRes.ok) {
                    throw new Error("Failed to upload profile picture.");
                }

                const uploadJson = await uploadRes.json();
                profilePictureUrl = uploadJson.url;
            }
            
            const registerRes = await fetch("/api/authorization/register", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    username,
                    email,
                    password,
                    profilePictureUrl,
                }),
            });

            if (!registerRes.ok) {
                const msg = await registerRes.text();
                throw new Error(msg || "Registration failed.");
            }

            alert("Account created! Please check your email.");
            setUsername("");
            setEmail("");
            setPassword("");
            setProfilePicture(null);
            navigate("/login");

        } catch (err: any) {
            setError(err.message || "An unexpected error occurred.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="register-container">
            <div className="register-card">
                <h1>Create Account 🎬</h1>

                <form onSubmit={handleSubmit}>
                    <input
                        type="text"
                        placeholder="Username"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        required
                    />
                    <input
                        type="email"
                        placeholder="Email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                    <input
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                    <input
                        type="file"
                        accept="image/*"
                        onChange={handleFileChange}
                    />

                    <button type="submit" disabled={loading}>
                        {loading ? "Registering..." : "Register"}
                    </button>

                    {error && <p className="error">{error}</p>}
                </form>

                <p>
                    Already have an account?{" "}
                    <span className="login-link" onClick={() => navigate("/login")}>
                        Login
                    </span>
                </p>
            </div>

            <style>{`
                .register-container {
                    min-height: 100vh;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    background-image: url("/movibackgroung.jpg");
                    background-size: cover;
                    background-position: center;
                }

                .register-card {
                    background: rgba(0, 0, 0, 0.85);
                    padding: 2rem;
                    border-radius: 12px;
                    width: 100%;
                    max-width: 400px;
                    box-shadow: 0 8px 24px rgba(0,0,0,0.8);
                    text-align: center;
                    color: #eee;
                }

                .register-card h1 {
                    margin-bottom: 1.5rem;
                    font-size: 2rem;
                    color: #ffa500;
                }

                .register-card input {
                    width: 100%;
                    padding: 0.75rem;
                    margin-bottom: 1rem;
                    border: 1px solid #444;
                    border-radius: 8px;
                    background-color: #1b1b1b;
                    color: #fff;
                }

                .register-card input[type="file"] {
                    color: #aaa;
                    background-color: transparent;
                    border: none;
                }

                .register-card button {
                    width: 100%;
                    padding: 0.75rem;
                    background: #ffa500;
                    color: white;
                    border: none;
                    border-radius: 8px;
                    font-weight: bold;
                    cursor: pointer;
                    transition: background 0.3s ease;
                }

                .register-card button:hover {
                    background: #e65c00;
                }

                .register-card .error {
                    color: #ff4c4c;
                    margin-top: 1rem;
                }

                .login-link {
                    color: #c11c1c;
                    cursor: pointer;
                    font-weight: bold;
                }

                .login-link:hover {
                    text-decoration: underline;
                    color: #ff66ff;
                }
            `}</style>
        </div>
    );
};

export default Register;
