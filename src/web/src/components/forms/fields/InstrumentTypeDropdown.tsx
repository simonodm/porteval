import React, { useEffect, useState } from 'react';
import Form from 'react-bootstrap/Form';
import * as constants from '../../../constants';
import { FormFieldProps, InstrumentType } from '../../../types';

/**
 * Renders an instrument type dropdown form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function InstrumentTypeDropdown(
    { className, value, label, disabled, onChange }: FormFieldProps<InstrumentType>
): JSX.Element {
    const types: Array<InstrumentType> =
        ['stock', 'bond', 'mutualFund', 'commodity', 'cryptoCurrency', 'etf', 'index', 'other']
    const [type, setType] = useState<InstrumentType>(value ?? 'stock');

    const handleTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newType = e.target.value as InstrumentType;
        setType(newType)
        onChange && onChange(newType);
    }

    // adjust internal state on `value` prop change
    useEffect(() => {
        if(value !== undefined) {
            setType(value);
        }
    }, [value]);

    // simulate dropdown value change to default if no `value` is available on first render
    useEffect(() => {
        if(value === undefined) {
            setType('stock');
            onChange && onChange('stock');
        }
    }, [])

    return (
        <Form.Group className={className} controlId="form-instrument-type">
            <Form.Label>{label ?? 'Instrument type'}:</Form.Label>
            <Form.Select
                disabled={disabled}
                onChange={handleTypeChange}
                value={type}
            >
                {types.map(t => <option key={t} value={t}>{constants.INSTRUMENT_TYPE_TO_STRING[t]}</option>)}
            </Form.Select>
        </Form.Group>
    )
}

export default InstrumentTypeDropdown;