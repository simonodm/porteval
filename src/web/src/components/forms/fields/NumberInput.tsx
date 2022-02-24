import React, { useEffect, useState } from 'react';

type Props = {
    label?: string;
    value?: number;
    disabled?: boolean;
    allowNegativeValues?: boolean;
    allowFloat?: boolean;
    validator?: (num: number) => boolean;
    onChange?: (num: number) => void;
}

export default function NumberInput({ label, value, disabled, allowNegativeValues, allowFloat, validator, onChange }: Props): JSX.Element {
    const [numberText, setNumberText] = useState(value?.toString() ?? '0');
    const [number, setNumber] = useState(value ?? 0);

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
                onChange && onChange(newNumber);
            }
        }
    }

    useEffect(() => {
        if(value !== undefined) {
            setNumberText(value.toString());
            setNumber(value);
        }
    }, [value]);

    return (
        <div className="form-group">
            <label htmlFor={label?.toLowerCase().replaceAll(' ', '-')}>{label}:</label>
            <input
                type="number"
                id={label?.toLowerCase().replaceAll(' ', '-')}
                className="form-control"
                value={numberText}
                disabled={disabled}
                onChange={handleNumberChange} />
        </div>
    )    
}