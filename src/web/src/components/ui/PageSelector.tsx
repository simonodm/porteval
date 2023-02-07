import React, { useEffect } from 'react';

type Props = {
    /**
     * Current page number.
     */
    page: number,

    /**
     * Total page count.
     */
    totalPages: number,

    /**
     * Callback which is invoked whenever the page is changed.
     */
    onPageChange?: (page: number) => void;

    /**
     * Callback which is invoked whenever a page prefetch is requested.
     */
    prefetch?: (page: number) => void;
}

/**
 * Renders pagination controls.
 * 
 * @category UI
 * @component
 */
function PageSelector({ page, totalPages, onPageChange, prefetch }: Props): JSX.Element {
    const handlePageChange = (newPage: number): void => {
        onPageChange && onPageChange(newPage);
    }

    useEffect(() => {
        if(page > 1) {
            prefetch && prefetch(page - 1);
        }
        if(page < totalPages) {
            prefetch && prefetch(page + 1);
        }
    }, [page, totalPages]);

    return (
        <span className="page-selector d-inline" aria-label="Pagination controls">
            { page > 1 &&
                <button
                    className="btn btn-primary btn-sm"
                    onClick={() => handlePageChange(page - 1)}
                    role="button"
                >
                    Previous page
                </button>
            }
            { page < totalPages &&
                <button
                    className="btn btn-primary btn-sm"
                    onClick={() => handlePageChange(page + 1)}
                    role="button"
                >
                    Next page
                </button>
            }
        </span>
    )
}

export default PageSelector;