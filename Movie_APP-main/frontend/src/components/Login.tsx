import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const Login: React.FC = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const response = await fetch('/api/authorization/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email, password })
            });

            const data = await response.json();
            console.log('Login response:', data);

            const token = data.response?.token;

            if (token) {
                localStorage.setItem('token', token);
                navigate('/dashboard');
            } else {
                alert(data.errorMessage || 'Login failed. Please check your credentials.');
            }
        } catch (error) {
            console.error('Login error:', error);
            alert('Login failed. Please try again later.');
        }
    };

    return (
        <div className="login-container">
            <div className="login-card">
                <h1>Welcome Back</h1>

                <form onSubmit={handleSubmit}>
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
                    <button type="submit">Login</button>
                </form>

                <p style={{ marginTop: "1.5rem" }}>
                    Don't have an account?{' '}
                    <span className="login-link" onClick={() => navigate('/register')}>
                        Register
                    </span>
                </p>
            </div>
            
            <style>{`
                .login-container {
                    min-height: 100vh;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    background-image: url("/movibackgroung.jpg");
                    background-size: cover;
                    background-position: center;
                    background-repeat: no-repeat;
                }

                .login-card {
                    background: rgba(0, 0, 0, 0.85);
                    padding: 2rem;
                    border-radius: 12px;
                    width: 100%;
                    max-width: 400px;
                    box-shadow: 0 8px 24px rgba(0,0,0,0.8);
                    text-align: center;
                    color: #eee;
                }

                .login-card h1 {
                    margin-bottom: 1.5rem;
                    font-size: 2rem;
                    color: #ffa500;
                }

                .login-card input {
                    width: 100%;
                    padding: 0.75rem;
                    margin-bottom: 1rem;
                    border: 1px solid #444;
                    border-radius: 8px;
                    background-color: #1b1b1b;
                    color: #fff;
                }

                .login-card button {
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

                .login-card button:hover {
                    background: #e65c00;
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

export default Login;
