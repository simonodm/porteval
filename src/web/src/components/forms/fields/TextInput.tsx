import React, { useState } from 'react';

type Props = {
    label: string;
    placeholder?: string;
    defaultValue?: string;
    validator?: (value: string) => boolean;
    onChange: (value: string) => void;
}

export default function TextInput({ label, placeholder, validator, defaultValue, onChange }: Props): JSX.Element {
    const [text, setText] = useState(defaultValue ?? '');

    const handleTextChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if(validator === undefined || validator(e.target.value)) {
            setText(e.target.value);
            onChange(e.target.value);
        }
    }

    return (
        <div className="form-group">
            <label htmlFor="transaction-note">{label}:</label>
            <input 
                type="text"
                id="position-note"
                placeholder={placeholder}
                className="form-control"
                value={text}
                onChange={handleTextChange} />
        </div>
    )
}