import React, { useEffect, useState } from 'react';

type Props = {
    className?: string;
    label?: string;
    value?: number;
    disabled?: boolean;
    allowNegativeValues?: boolean;
    allowFloat?: boolean;
    validator?: (num: number) => boolean;
    onChange?: (num: number) => void;
}

export default function NumberInput(
    { className, label, value, disabled, allowNegativeValues, allowFloat, validator, onChange }: Props
): JSX.Element {
    const [numberText, setNumberText] = useState(value?.toString() ?? '');

    const updateNumber = (value: string) => {
        const newNumber = allowFloat ? parseFloat(value) : parseInt(value);

        if(!isNaN(newNumber)) {
            if(validator === undefined || validator(newNumber)) {
                onChange && onChange(newNumber);
            }
        }
    }

    const handleNumberChange = (e: React.ChangeEvent<HTMLInputElement>) => { 
        let inputRegexPattern = '\\d*';
        let numberRegexPattern = '\\d+';
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
            // Since JS differentiates between +0 and -0, this special check is enough
            // to convert -0 to '-0' string (which naturally gets converted to '0' by .toString())
            if(Object.is(value, -0)) {
                setNumberText('-0');
            } else {
                setNumberText(value.toString());
            }
        }
    }, [value]);

    return (
        <div className={`form-group ${className ?? ''}`}>
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