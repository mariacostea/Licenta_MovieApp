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
                                <MovieCardWatched {...m} isRecommended={true} />
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </>
    );
};

export default RecommendedMovies;
