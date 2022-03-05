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

export default function NumberInput(
    { label, value, disabled, allowNegativeValues, allowFloat, validator, onChange }: Props
): JSX.Element {
    const [numberText, setNumberText] = useState(value?.toString() ?? '0');
    const [number, setNumber] = useState(value ?? 0);

    const updateNumber = (value: string) => {
        const newNumber = allowFloat ? parseFloat(value) : parseInt(value);

        if(!isNaN(newNumber)) {
            if(validator === undefined || validator(number)) {
                setNumber(newNumber);
                onChange && onChange(newNumber);
            }
        }
    }

    const handleNumberChange = (e: React.ChangeEvent<HTMLInputElement>) => { 
        let inputRegexPattern = '\\d*';
        let numberRegexPattern = '\\d*';
        if(allowFloat) {
            inputRegexPattern += '(\\.\\d*)?';
            numberRegexPattern += '(\\.\\d+)?';
        }
        if(allowNegativeValues) {
            inputRegexPattern = '-?' + inputRegexPattern;
            numberRegexPattern = '-?' + numberRegexPattern;
        }

        const inputRegex = new RegExp(`^(${inputRegexPattern})?$`, 'g');
        const numberRegex = new RegExp(`^${numberRegexPattern}$`, 'g');

        if(inputRegex.test(e.target.value)) {
            setNumberText(e.target.value);
        }
        if(numberRegex.test(e.target.value)) {
            updateNumber(e.target.value);
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
                className="form-control"
                disabled={disabled}
                id={label?.toLowerCase().replaceAll(' ', '-')}
                onChange={handleNumberChange}
                value={numberText}
            />
        </div>
    )    
}