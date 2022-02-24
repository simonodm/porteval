import React, { useEffect, useState } from 'react';

type Props = {
    label: string;
    placeholder?: string;
    disabled?: boolean;
    value?: string;
    validator?: (value: string) => boolean;
    onChange?: (value: string) => void;
}

export default function TextInput({ label, placeholder, disabled, value, validator, onChange }: Props): JSX.Element {
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
        <div className="form-group">
            <label htmlFor="transaction-note">{label}:</label>
            <input 
                type="text"
                id="position-note"
                disabled={disabled}
                placeholder={placeholder}
                className="form-control"
                value={text}
                onChange={handleTextChange} />
        </div>
    )
}