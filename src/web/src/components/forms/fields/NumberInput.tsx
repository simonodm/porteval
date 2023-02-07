import React, { useEffect, useState } from 'react';
import Form from 'react-bootstrap/Form';
import { FormFieldProps } from '../../../types';

type Props = FormFieldProps<number> & {
    /**
     * Determines whether negative numeric values are allowed.
     */
    allowNegativeValues?: boolean;

    /**
     * Determines whether decimal values are allowed.
     */
    allowFloat?: boolean;

    /**
     * Validator function. The return value of this determines whether {@link onChange} is invoked.
     */
    validator?: (num: number) => boolean;
}

/**
 * Renders a number input form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function NumberInput(
    { className, label, value, disabled, allowNegativeValues, allowFloat, validator, onChange }: Props
): JSX.Element {
    
    // separate state bound to <input /> `value` attribute, needed to allow incomplete values such as "1." in the input
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
        // We do two separate input validations - one to test whether the text is valid (only contains valid symbols),
        // and another to check if the text is a valid number.
        // The reason for that is to allow users to type in values like "1." or "-"
        // without breaking the input or parent state.
        // The number change callback only gets called if the text is a valid number, otherwise old number persists.
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
            // Since JS differentiates between +0 and -0, this check is enough
            // to convert -0 to '-0' string (which normally gets converted to '0' by .toString())
            if(Object.is(value, -0)) {
                setNumberText('-0');
            } else {
                setNumberText(value.toString());
            }
        } else {
            setNumberText('');
        }
    }, [value]);

    return (
        <Form.Group className={className} controlId={`form-number-${label?.replaceAll(' ', '-').toLowerCase()}`}>
            <Form.Label>{label}:</Form.Label>
            <Form.Control
                type="text"
                aria-label={label}
                disabled={disabled}
                onChange={handleNumberChange}
                value={numberText}
            />
        </Form.Group>
    )    
}

export default NumberInput;