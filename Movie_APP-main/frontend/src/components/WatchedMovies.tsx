﻿import React, { useEffect, useState } from "react";
import NavigationBar from "./NavigationBar";
import MovieCard, { MovieCardProps } from "./MovieCard";

const WatchedMovies: React.FC = () => {
    const [movies, setMovies] = useState<MovieCardProps[]>([]);
    const [loading, setLoading] = useState(true);

    const loadWatchedMovies = async () => {
        setLoading(true);
        const token = localStorage.getItem("token");

        try {
            const resIds = await fetch("http://localhost:5000/api/UserMovie/GetWatchedMovies/watched", {
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

            const jsonIds = await resIds.json();
            const ids = jsonIds.result as string[];
            
            const moviePromises = ids.map(id =>
                fetch(`http://localhost:5000/api/movie/${id}`)
                    .then(res => res.json())
                    .then(data => data.result as MovieCardProps)
            );

            const movieData = await Promise.all(moviePromises);
            
            setMovies(movieData);
        } catch (err) {
            console.error("Error loading watched movies:", err);
            alert("Error loading watched movies!");
        } finally {
            setLoading(false);
        }
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
                                <MovieCard {...m} isWatched={true} />
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </>
    );
};

export default WatchedMovies;
