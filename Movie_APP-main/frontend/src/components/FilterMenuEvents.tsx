import React, { forwardRef } from "react";

interface Props {
    onApply: (filters: { [key: string]: string }) => void;
}

const FilterMenuEvents = forwardRef<HTMLDivElement, Props>(({ onApply }, ref) => {
    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        const fd = new FormData(e.currentTarget as HTMLFormElement);

        onApply({
            location: fd.get("location") as string,
            day: fd.get("day") as string,
            fullDate: fd.get("fullDate") as string,
            month: fd.get("month") as string,
            movie: fd.get("movie") as string,
        });
    };

    return (
        <div className="offcanvas offcanvas-end text-bg-light" ref={ref}>
            <div className="offcanvas-header">
                <h5 className="offcanvas-title">Filter Events</h5>
                <button type="button" className="btn-close" data-bs-dismiss="offcanvas"></button>
            </div>
            <div className="offcanvas-body">
                <form onSubmit={handleSubmit} className="d-grid gap-3">
                    <input name="location" className="form-control" placeholder="Location" />
                    <input name="day" className="form-control" placeholder="Day (YYYY-MM-DD)" />
                    <input name="fullDate" className="form-control" placeholder="Full DateTime" />
                    <input name="month" className="form-control" placeholder="Month (YYYY-MM)" />
                    <input name="movie" className="form-control" placeholder="Movie Title" />

                    <button className="btn btn-success w-100">Apply</button>
                </form>
            </div>
        </div>
    );
});

export default FilterMenuEvents;
