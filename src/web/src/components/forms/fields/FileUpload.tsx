import React from 'react';

type Props = {
    label?: string;
    className?: string;
    onUpload?: (file: File) => void;
}

export default function FileUpload({label, className, onUpload}: Props): JSX.Element {
    const handleFileUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
        if(!e.target.files?.length) {
            return;
        }

        onUpload && onUpload(e.target.files[0]);
    }
    
    return (
        <div className={`form-group ${className ?? ''}`}>
            <label htmlFor="file">{label}:</label>
            <input
                accept="text/csv"
                className="form-control-file"
                id="file"
                name="file"
                onChange={handleFileUpload}
                type="file"
            />
        </div>
    )
}