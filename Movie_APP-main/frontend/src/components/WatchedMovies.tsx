import React, { useEffect, useState } from "react";
import NavigationBar from "./NavigationBar";
import MovieCardWatched, { MovieCardProps } from "./MovieCardWatched";

const WatchedMovies: React.FC = () => {
    const [movies, setMovies] = useState<MovieCardProps[]>([]);
    const [loading, setLoading] = useState(true);

    const loadWatchedMovies = async () => {
        setLoading(true);
        const token = localStorage.getItem("token");

        try {
            const res = await fetch("https://licenta-backend-nf1m.onrender.com/api/UserMovie/GetWatchedMovies/watched", {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (!res.ok) throw new Error("Failed to load watched movies");

            const json = await res.json();
            const movieData = json.result as MovieCardProps[];

            setMovies(movieData);
        } catch (err) {
            console.error("Error loading watched movies:", err);
            alert("Error loading watched movies!");
        } finally {
            setLoading(false);
        }
    };

    const handleUnwatch = (id: string) => {
        setMovies(prev => prev.filter(m => m.id !== id));
    };

    const handleRecommended = (id: string) => {
        setMovies(prev =>
            prev.map(m => m.id === id ? { ...m, isRecommended: true } : m)
        );
    };

    const handleUnrecommended = (id: string) => {
        setMovies(prev =>
            prev.map(m => m.id === id ? { ...m, isRecommended: false } : m)
        );
    };

    useEffect(() => {
        loadWatchedMovies();
    }, []);

    return (
        <>
            <NavigationBar onSearch={() => {}} />

            <div className="container py-4 text-white">
                <h1>Watched Movies</h1>

                {loading && <p>Loading...</p>}
                {!loading && movies.length === 0 && <p>No watched movies found.</p>}

                {!loading && (
                    <div className="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 g-4">
                        {movies.map((m) => (
                            <div className="col" key={m.id}>
                                <MovieCardWatched
                                    {...m}
                                    onRecommended={handleRecommended}
                                    onUnrecommended={handleUnrecommended}
                                    onUnwatch={handleUnwatch}
                                    showUnwatchButton={true}
                                />
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </>
    );
};

export default WatchedMovies;
