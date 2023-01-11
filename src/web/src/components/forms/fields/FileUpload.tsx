import React, { useRef } from 'react';

type Props = {
    /**
     * Custom label to use for the form field.
     */
    label?: string;

    /**
     * Custom class name to use for the form field.
     */
    className?: string;

    /**
     * A callback which is invoked every time a file is selected.
     */
    onFileSelected?: (file: File) => void;
}

/**
 * Renders a file upload form feild.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function FileUpload({label, className, onFileSelected}: Props): JSX.Element {
    const inputRef = useRef<HTMLInputElement | null>(null);

    const handleFileUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
        if(!e.target.files?.length) {
            return;
        }

        onFileSelected && onFileSelected(e.target.files[0]);
    }
    
    return (
        <div className={`form-group ${className ?? ''}`}>
            <label htmlFor="file">{label}:</label>
            <input
                ref={inputRef}
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

export default FileUpload;