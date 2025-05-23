import React, { useState } from 'react';
import axios from 'axios';

const Feedback = () => {
    const [feedbackType, setFeedbackType] = useState('');
    const [rating, setRating] = useState<number | null>(null);
    const [agreeToTerms, setAgreeToTerms] = useState(false);
    const [comment, setComment] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await axios.post('http://localhost:5000/api/Feedback/submit', {
                feedbackType,
                rating,
                agreeToTerms,
                comment,
            }, {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`,
                },
            });
            alert('Feedback trimis!');
        } catch (error) {
            alert('Eroare la trimitere.');
        }
    };

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
            <form
                onSubmit={handleSubmit}
                className="h-full w-full max-w-4xl bg-zinc-900 p-12 rounded-xl shadow-lg flex flex-col justify-between items-center space-y-10"
            >
                <h1 className="text-5xl font-bold text-center">Trimite Feedback</h1>

                <div className="flex flex-col w-full text-xl">
                    <label className="mb-2">Tip feedback</label>
                    <select
                        value={feedbackType}
                        onChange={(e) => setFeedbackType(e.target.value)}
                        required
                        className="bg-black text-white border border-white px-4 py-3 rounded text-lg"
                    >
                        <option value="">Alege...</option>
                        <option value="Sugestie">Sugestie</option>
                        <option value="Eroare">Eroare</option>
                        <option value="Altceva">Altceva</option>
                    </select>
                </div>

                <div className="flex flex-col w-full text-xl">
                    <label className="mb-2">Rating</label>
                    <div className="flex justify-around text-3xl">
                        {[1, 2, 3, 4, 5].map((val) => (
                            <label key={val} className="flex items-center gap-2">
                                <input
                                    type="radio"
                                    value={val}
                                    checked={rating === val}
                                    onChange={() => setRating(val)}
                                    className="scale-150 accent-white"
                                />
                                {val}
                            </label>
                        ))}
                    </div>
                </div>

                <div className="flex items-center w-full gap-4 text-lg">
                    <input
                        type="checkbox"
                        checked={agreeToTerms}
                        onChange={(e) => setAgreeToTerms(e.target.checked)}
                        className="scale-150 accent-white"
                        id="terms"
                    />
                    <label htmlFor="terms">Sunt de acord cu termenii și condițiile</label>
                </div>

                <div className="flex flex-col w-full text-xl">
                    <label className="mb-2">Comentariu</label>
                    <textarea
                        value={comment}
                        onChange={(e) => setComment(e.target.value)}
                        rows={6}
                        className="bg-black text-white border border-white px-4 py-3 rounded resize-none text-lg"
                        placeholder="Scrie un comentariu..."
                    />
                </div>

                <button
                    type="submit"
                    className="w-full bg-white text-black text-2xl font-semibold py-4 rounded hover:bg-gray-300 transition"
                >
                    Trimite
                </button>
            </form>
        </div>

    );
};

export default Feedback;
