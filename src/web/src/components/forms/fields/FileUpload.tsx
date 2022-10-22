import React from 'react';

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
     * A callback which is invoked every time a file is uploaded.
     */
    onUpload?: (file: File) => void;
}

/**
 * Renders a file upload form feild.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function FileUpload({label, className, onUpload}: Props): JSX.Element {
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

export default FileUpload;