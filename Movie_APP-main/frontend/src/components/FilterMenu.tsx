// Bootstrap Offcanvas panel (end/right)
import React, { useRef } from "react";
import { Offcanvas } from "bootstrap";

interface Props {
    onApply: (filters: { year: string; genre: string }) => void;
}

const FilterMenu: React.FC<Props> = ({ onApply }) => {
    const panel = useRef<HTMLDivElement>(null);

    /** helper – creăm instanţa Bootstrap Offcanvas la nevoie */
    const bs = () => panel.current && Offcanvas.getOrCreateInstance(panel.current);

    /** trimite filtrele + închide panoul */
    const handleApply = (e: React.FormEvent) => {
        e.preventDefault();
        const fd = new FormData(e.currentTarget as HTMLFormElement);

        onApply({
            year:   (fd.get("year")   as string) ?? "",
            genre:  (fd.get("genre")  as string) ?? ""
        });

        bs()?.hide();
    };

    return (
        <>
            {/* buton care deschide panoul */}
            <button className="btn btn-primary" onClick={() => bs()?.show()}>
                Filter
            </button>

            {/*  off-canvas  */}
            <div className="offcanvas offcanvas-end text-bg-light" ref={panel} id="filterPanel">
                <div className="offcanvas-header">
                    <h5 className="offcanvas-title">Filters</h5>
                    <button type="button" className="btn-close" data-bs-dismiss="offcanvas" />
                </div>

                <div className="offcanvas-body">
                    <form onSubmit={handleApply} className="d-grid gap-3">
                        {/* Year */}
                        <div>
                            <label className="form-label">Year</label>
                            <input
                                className="form-control"
                                type="number"
                                name="year"
                                placeholder="e.g. 2015"
                            />
                        </div>

                        {/* Genre */}
                        <div>
                            <label className="form-label">Genre</label>
                            <select name="genre" className="form-select default-select">
                                <option value="">(all)</option>
                                {[
                                    "Action","Animation","Comedy","Crime","Documentary","Drama",
                                    "Family","Fantasy","History","Horror","Music","Mystery",
                                    "Romance","Science Fiction","Thriller","TV Movie","War","Western"
                                ].map(g => <option key={g}>{g}</option>)}
                            </select>
                        </div>

                        <button className="btn btn-success w-100">Apply</button>
                    </form>
                </div>
            </div>
        </>
    );
};

export default FilterMenu;
