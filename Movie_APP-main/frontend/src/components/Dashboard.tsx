import React from 'react';
import { useNavigate } from 'react-router-dom';

const Dashboard: React.FC = () => {
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('userId');
        navigate('/login');
    };

    const buttons = [
        { label: '🎬 Movies', path: '/movies' },
        { label: '📅 Events', path: '/events' },
        { label: '📰 Feed', path: '/feed' },
        { label: '👥 People', path: '/people' },
        { label: '⭐ Recommendation', path: '/recommendation' },
        { label: '📝 Feedback', path: '/feedback' },
    ];

    return (
        <div
            style={{
                backgroundImage: 'linear-gradient(rgba(0,0,0,0.8), rgba(0,0,0,0.8)), url(/movibackgroung.jpg)',
                backgroundSize: 'cover',
                backgroundPosition: 'center',
                height: '100vh',
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                fontFamily: 'Arial, sans-serif',
            }}
        >
            <div
                style={{
                    backgroundColor: 'rgba(0, 0, 0, 0.9)',
                    padding: '3rem',
                    borderRadius: '16px',
                    boxShadow: '0 8px 24px rgba(0,0,0,0.3)',
                    textAlign: 'center',
                    width: '90%',
                    maxWidth: '600px',
                }}
            >
                <h1 style={{ fontSize: '2.5rem', marginBottom: '1.5rem', color: '#eee' }}>
                    Welcome to <span style={{ color: '#c11c1c' }}>MovieApp 🎬</span>
                </h1>

                <div style={{ display: 'flex', flexWrap: 'wrap', gap: '1rem', justifyContent: 'center' }}>
                    {buttons.map((btn) => (
                        <button
                            key={btn.path}
                            onClick={() => navigate(btn.path)}
                            style={{
                                padding: '0.75rem 1.5rem',
                                fontSize: '1.1rem',
                                borderRadius: '10px',
                                border: 'none',
                                backgroundColor: '#c11c1c',
                                color: 'white',
                                cursor: 'pointer',
                                transition: '0.3s ease',
                                fontWeight: 'bold',
                                minWidth: '160px',
                            }}
                        >
                            {btn.label}
                        </button>
                    ))}
                </div>

                <button
                    onClick={handleLogout}
                    style={{
                        marginTop: '2rem',
                        padding: '0.6rem 1.2rem',
                        fontSize: '1rem',
                        borderRadius: '8px',
                        backgroundColor: '#444',
                        color: 'white',
                        border: 'none',
                        cursor: 'pointer',
                        fontWeight: 'bold'
                    }}
                >
                    🔓 Logout
                </button>
            </div>
        </div>
    );
};

export default Dashboard;
