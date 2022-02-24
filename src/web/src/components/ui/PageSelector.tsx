import React from 'react';

type Props = {
    page: number,
    totalPages: number,
    onPageChange: (page: number) => void;
    prefetch: (page: number) => void;
}

export default function PageSelector({ page, totalPages, onPageChange, prefetch }: Props): JSX.Element {
    if(page > 1) {
        prefetch(page - 1);
    }
    if(page < totalPages) {
        prefetch(page + 1);
    }

    return (
        <div className="page-selector">
            { page > 1 &&
                <button
                    className="btn btn-primary btn-sm mr-1"
                    onClick={() => onPageChange(page - 1)}
                    role="button"
                >
                    Previous page
                </button>
            }
            { page < totalPages &&
                <button
                    className="btn btn-primary btn-sm"
                    onClick={() => onPageChange(page + 1)}
                    role="button"
                >
                    Next page
                </button>
            }
        </div>
    )
}