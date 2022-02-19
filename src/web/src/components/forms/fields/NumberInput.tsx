import React, { useState } from 'react';

type Props = {
    label?: string;
    defaultValue?: number;
    allowNegativeValues?: boolean;
    allowFloat?: boolean;
    validator?: (num: number) => boolean;
    onChange: (num: number) => void;
}

export default function NumberInput({ label, defaultValue, allowNegativeValues, allowFloat, validator, onChange }: Props): JSX.Element {
    const [numberText, setNumberText] = useState(defaultValue?.toString() ?? '0');
    const [number, setNumber] = useState(defaultValue ?? 0);

    const handleNumberChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        let resultTextValue;
        if(!allowNegativeValues) {
            resultTextValue = e.target.value.replaceAll('-', '');
        }
        else {
            resultTextValue = e.target.value;
        }
        
        setNumberText(resultTextValue);

        let newNumber;
        if(allowFloat) {
            newNumber = parseFloat(resultTextValue);
        }
        else {
            newNumber = parseInt(resultTextValue);
        }

        if(!isNaN(newNumber)) {
            if(validator === undefined || validator(number)) {
                setNumber(newNumber);
                onChange(newNumber);
            }
        }
    }

    return (
        <div className="form-group">
            <label htmlFor={label?.toLowerCase().replaceAll(' ', '-')}>{label}:</label>
            <input
                type="number"
                id={label?.toLowerCase().replaceAll(' ', '-')}
                className="form-control"
                value={numberText}
                disabled={defaultValue !== undefined}
                onChange={handleNumberChange} />
        </div>
    )    
}