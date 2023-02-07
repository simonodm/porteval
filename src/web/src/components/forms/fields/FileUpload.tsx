import React, { useRef } from 'react';
import Form from 'react-bootstrap/Form';
import { FormFieldProps } from '../../../types';

type Props = Omit<FormFieldProps<File>, 'value'>;

/**
 * Renders a file upload form feild.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function FileUpload({label, disabled, className, onChange}: Props): JSX.Element {
    const inputRef = useRef<HTMLInputElement | null>(null);

    const handleFileUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
        if(!e.target.files?.length) {
            return;
        }

        onChange && onChange(e.target.files[0]);
    }
    
    return (
        <Form.Group className={className} controlId="form-file-upload">
            <Form.Label htmlFor="file">{label}:</Form.Label>
            <Form.Control
                disabled={disabled}
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