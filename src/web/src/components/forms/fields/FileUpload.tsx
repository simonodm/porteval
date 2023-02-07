import React, { useRef } from 'react';

import Form from 'react-bootstrap/Form';

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
        <Form.Group className={className} controlId="form-file-upload">
            <Form.Label htmlFor="file">{label}:</Form.Label>
            <Form.Control
                aria-label={label}
                ref={inputRef}
                accept="text/csv"
                className="form-control-file"
                onChange={handleFileUpload}
                type="file"
            />
        </Form.Group>
    )
}

export default FileUpload;