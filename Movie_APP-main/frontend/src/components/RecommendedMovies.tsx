import React, { useEffect, useState } from "react";
import NavigationBar from "./NavigationBar";
import MovieCardWatched, { MovieCardProps } from "./MovieCardWatched";

const RecommendedMovies: React.FC = () => {
    const [movies, setMovies] = useState<MovieCardProps[]>([]);
    const [loading, setLoading] = useState(true);

    const loadRecommendedMovies = async () => {
        setLoading(true);
        const token = localStorage.getItem("token");

        try {
            const resIds = await fetch("https://licenta-backend-nf1m.onrender.com/api/UserMovie/GetRecommendedMovies/recommended", {
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

            const jsonIds = await resIds.json();
            const ids = jsonIds.result as string[];

            const moviePromises = ids.map(id =>
                fetch(`https://licenta-backend-nf1m.onrender.com/api/movie/${id}`)
                    .then(res => res.json())
                    .then(data => data.result as MovieCardProps)
            );

            const movieData = await Promise.all(moviePromises);

            setMovies(movieData);
        } catch (err) {
            console.error("Error loading recommended movies:", err);
            alert("Error loading recommended movies!");
        } finally {
            setLoading(false);
        }
    };

    const handleUnrecommend = async (movie: MovieCardProps) => {
        const token = localStorage.getItem("token");
        if (!token) return alert("You must be logged in.");

        try {
            const res = await fetch("https://licenta-backend-nf1m.onrender.com/api/UserMovie/unrecommend", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ title: movie.title, year: movie.year }),
            });

            if (!res.ok) {
                const data = await res.json();
                throw new Error(data?.error?.message || "Failed to unrecommend movie.");
            }

            setMovies(prev => prev.filter(m => m.id !== movie.id));
            alert("Movie was removed from recommendations.");
        } catch (err) {
            alert((err as Error).message);
        }
    };

    useEffect(() => {
        loadRecommendedMovies();
    }, []);

    return (
        <>
            <NavigationBar onSearch={() => {}} />

            <div className="container py-4 text-white">
                <h1>Recommended Movies</h1>

                {loading && <p>Loading...</p>}

                {!loading && movies.length === 0 && <p>No recommended movies yet.</p>}

                {!loading && (
                    <div className="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 g-4">
                        {movies.map((m) => (
                            <div className="col" key={m.id}>
                                <MovieCardWatched
                                    {...m}
                                    isRecommended={true}
                                    onUnrecommended={() => handleUnrecommend(m)}
                                />
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </>
    );
};

export default RecommendedMovies;
