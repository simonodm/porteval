import React, { useEffect, useState } from 'react';
import Form from 'react-bootstrap/Form';
import { FormFieldProps } from '../../../types';

type Props = FormFieldProps<string> & {
    /**
     * Placeholder text to display in the text input.
     */
    placeholder?: string;

    /**
     * Validator function. The return value of this determines whether {@link onChange} is invoked.
     */
    validator?: (value: string) => boolean;
}

/**
 * Renders a text input form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function TextInput(
    { label, className, placeholder, disabled, value, validator, onChange }: Props
): JSX.Element {
    const [text, setText] = useState(value ?? '');

    const handleTextChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if(validator === undefined || validator(e.target.value)) {
            setText(e.target.value);
            onChange && onChange(e.target.value);
        }
    }

    // adjust internal state on `value` prop change
    useEffect(() => {
        if(value !== undefined) {
            setText(value);
        }
    }, [value]);

    return (
        <Form.Group className={className} controlId={`form-text-${label?.replaceAll(' ', '-').toLowerCase()}`}>
            <Form.Label>{label}:</Form.Label>
            <Form.Control 
                aria-label={label}
                disabled={disabled}
                onChange={handleTextChange}
                placeholder={placeholder}
                type="text"
                value={text}
            />
        </Form.Group>
    )
}

export default TextInput;