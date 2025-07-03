import React, { useEffect, useState, useCallback } from "react";
import MovieCard, { MovieCardProps } from "./MovieCard";
import Pagination       from "./Pagination";
import NavigationBar    from "./NavigationBar";
import FilterMenu       from "./FilterMenu";

export interface PagedResult {
    page: number;
    pageSize: number;
    totalCount: number;
    data: MovieCardProps[];
}

type ActiveFilter = { year?: string; genre?: string } | null;

const pageSize = 20;

const Movies: React.FC = () => {
    const [movies,        setMovies]        = useState<MovieCardProps[]>([]);
    const [currentPage,   setPage]          = useState(1);
    const [totalPages,    setTotalPages]    = useState(1);
    const [loading,       setLoading]       = useState(false);
    const [watchedIds,    setWatchedIds]    = useState<string[]>([]);
    const [filter,        setFilter]        = useState<ActiveFilter>(null);

    const mergeWithWatched = useCallback(
        (list: MovieCardProps[]) =>
            list
                .filter(m => !watchedIds.includes(m.id))
                .map(m => ({ ...m, isWatched: false })),
        [watchedIds]
    );


    const fetchWatched = async (): Promise<string[]> => {
        const token = localStorage.getItem("token");
        if (!token) return [];

        const res = await fetch("https://licenta-backend-nf1m.onrender.com/api/UserMovie/GetWatchedMovies/watched", {
            headers: { Authorization: `Bearer ${token}` },
        });
        const json = await res.json().catch(() => ({ result: [] }));
        const watchedList = json.result ?? [];
        return watchedList.map((m: any) => m.id);
    };



    const fetchPage = async (page: number, f: ActiveFilter = filter) => {
        const base =
            f && (f.year || f.genre)
                ? "https://licenta-backend-nf1m.onrender.com/api/movie/filter"
                : "https://licenta-backend-nf1m.onrender.com/api/movie/all";

        const qs = new URLSearchParams({
            page: page.toString(),
            pageSize: pageSize.toString(),
        });
        if (f?.year)  qs.append("year",  f.year);
        if (f?.genre) qs.append("genre", f.genre);

        const res  = await fetch(`${base}?${qs.toString()}`);
        const json = await res.json();
        return json.result as PagedResult;
    };
    
    const load = useCallback(
        async (page = 1, f: ActiveFilter = filter) => {
            setLoading(true);
            try {
                const [ids, pageData] = await Promise.all([fetchWatched(), fetchPage(page, f)]);
                setWatchedIds(ids);
                setMovies(mergeWithWatched(pageData.data));
                setPage(pageData.page);
                setTotalPages(Math.max(1, Math.ceil(pageData.totalCount / pageData.pageSize)));
            } catch (err) {
                console.error(err);
                alert("Eroare la încărcare!");
            } finally {
                setLoading(false);
            }
        },
        [filter, mergeWithWatched]
    );
    
    useEffect(() => { load(1); }, []);
    
    const handleMarked = (id: string) => {
        setWatchedIds((prev) => (prev.includes(id) ? prev : [...prev, id]));
    };
    
    const searchMovie = async (title: string) => {
        setLoading(true);
        try {
            const [ids, list] = await Promise.all([
                fetchWatched(),
                fetch(`https://licenta-backend-nf1m.onrender.com/api/movie/search-by-title?title=${encodeURIComponent(title)}`)
                    .then((r) => r.json())
                    .then((j) => j.result as MovieCardProps[]),
            ]);
            if (!list.length) return alert("N‑am găsit nimic!");
            setWatchedIds(ids);
            setFilter(null);
            setMovies(mergeWithWatched(list));
            setPage(1);
            setTotalPages(1);
        } catch (err) {
            console.error(err);
            alert("Eroare la căutare!");
        } finally {
            setLoading(false);
        }
    };
    
    const applyFilter = (f: { year: string; genre: string }) => {
        const active: ActiveFilter =
            f.year || f.genre ? { year: f.year || undefined, genre: f.genre || undefined } : null;
        setFilter(active);
        load(1, active);
    };
    
    const handlePageChange = (p: number) => {
        if (p !== currentPage && p >= 1 && p <= totalPages) load(p);
    };

    return (
        <>
            <div
                className="bg-dark py-2 border-bottom position-sticky top-0 z-3"
                style={{ zIndex: 1040 }}
            >
                <div className="container d-flex justify-content-center gap-3">
                    <a href="/recommendation" className="btn btn-outline-light btn-sm">⭐ Recommendations</a>
                    <a href="/events" className="btn btn-outline-light btn-sm">📅 Events</a>
                    <a href="/feed" className="btn btn-outline-light btn-sm">📰 Feed</a>
                    <a href="/people" className="btn btn-outline-light btn-sm">👥  People</a>
                    <button
                        className="btn btn-outline-danger btn-sm"
                        onClick={() => {
                            localStorage.removeItem('token');
                            localStorage.removeItem('userId');
                            window.location.href = '/login';
                        }}
                    >
                        🚪 Logout
                    </button>
                </div>
            </div>
            <NavigationBar onSearch={searchMovie} />

            <div className="container-fluid py-4 text-white">
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <h1 className="mb-0">All Movies</h1>
                    <FilterMenu onApply={applyFilter} />
                </div>

                {loading && <p className="text-center">Loading…</p>}

                {!loading && (
                    <>
                        <div className="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 g-4">
                            {movies.map((m) => (
                                <div className="col" key={m.id}>
                                    <MovieCard {...m} onMarked={handleMarked} />
                                </div>
                            ))}
                        </div>

                        <div className="d-flex justify-content-center mt-4">
                            <Pagination
                                currentPage={currentPage}
                                totalPages={totalPages}
                                onPageChange={handlePageChange}
                            />
                        </div>
                    </>
                )}
            </div>
        </>
    );
};

export default Movies;

