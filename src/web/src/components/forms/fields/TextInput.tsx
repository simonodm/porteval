import React, { useEffect, useState } from 'react';

type Props = {
    /**
     * Custom label to use for the form field.
     */
    label: string;

    /**
     * Custom class name to use for the form field.
     */
    className?: string;

    /**
     * Placeholder text to display in the text input.
     */
    placeholder?: string;

    /**
     * Determines whether the form field is disabled.
     */
    disabled?: boolean;

    /**
     * Binding property for the text input's current value.
     */
    value?: string;

    /**
     * Validator function. The return value of this determines whether {@link onChange} is invoked.
     */
    validator?: (value: string) => boolean;

    /**
     * A callback which is invoked whenever input's value changes and passes the validator.
     */
    onChange?: (value: string) => void;
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

    useEffect(() => {
        if(value !== undefined) {
            setText(value);
        }
    }, [value]);

    return (
        <div className={`form-group ${className ?? ''}`}>
            <label htmlFor="transaction-note">{label}:</label>
            <input 
                className="form-control"
                disabled={disabled}
                id="position-note"
                onChange={handleTextChange}
                placeholder={placeholder}
                type="text"
                value={text}
            />
        </div>
    )
}

export default TextInput;