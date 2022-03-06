import React, { useEffect, useState } from 'react';

type Props = {
    label: string;
    className?: string;
    placeholder?: string;
    disabled?: boolean;
    value?: string;
    validator?: (value: string) => boolean;
    onChange?: (value: string) => void;
}

export default function TextInput(
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