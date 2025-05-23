import React from "react";

interface PaginationProps {
    currentPage: number;
    totalPages: number;
    onPageChange: (page: number) => void;
    windowSize?: number;         
}

const Pagination: React.FC<PaginationProps> = ({
                                                   currentPage,
                                                   totalPages,
                                                   onPageChange,
                                                   windowSize = 10,
                                               }) => {
    const half = Math.floor(windowSize / 2);

    let start = Math.max(1, currentPage - half);
    let end   = start + windowSize - 1;
    
    if (end > totalPages) {
        end   = totalPages;
        start = Math.max(1, end - windowSize + 1);
    }

    const pages = Array.from({ length: end - start + 1 }, (_, i) => start + i);

    const gotoFirstWindow = () => onPageChange(Math.max(1, start - windowSize));
    const gotoLastWindow  = () => onPageChange(Math.min(totalPages, end + 1));

    return (
        <nav aria-label="Movies pagination">
            <ul className="pagination justify-content-center flex-wrap">

                {/* « PREV pagină */}
                <li className={`page-item ${currentPage === 1 && "disabled"}`}>
                    <button className="page-link" onClick={() => onPageChange(currentPage - 1)}>
                        &laquo;
                    </button>
                </li>
                
                {start > 1 && (
                    <li className="page-item">
                        <button className="page-link" onClick={gotoFirstWindow}>…</button>
                    </li>
                )}
                
                {pages.map((page) => (
                    <li key={page} className={`page-item ${page === currentPage && "active"}`}>
                        <button className="page-link" onClick={() => onPageChange(page)}>
                            {page}
                        </button>
                    </li>
                ))}
                
                {end < totalPages && (
                    <li className="page-item">
                        <button className="page-link" onClick={gotoLastWindow}>…</button>
                    </li>
                )}
                
                <li className={`page-item ${currentPage === totalPages && "disabled"}`}>
                    <button className="page-link" onClick={() => onPageChange(currentPage + 1)}>
                        &raquo;
                    </button>
                </li>
            </ul>
        </nav>
    );
};

export default Pagination;
