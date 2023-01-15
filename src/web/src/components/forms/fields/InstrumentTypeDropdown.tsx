import React, { useEffect, useState } from 'react';
import { InstrumentType } from '../../../types';
import * as constants from '../../../constants';

type Props = {
    /**
     * Custom class name to use for the form field.
     */
    className?: string;

    /**
     * Binding property for the dropdown's current value.
     */
    value?: InstrumentType;

    /**
     * Determines whether the form field is disabled.
     */
    disabled?: boolean;

    /**
     * A callback which is invoked whenever the dropdown's selection changes.
     */
    onChange?: (type: InstrumentType) => void;
}

/**
 * Renders an instrument type dropdown form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function InstrumentTypeDropdown({ className, value, disabled, onChange }: Props): JSX.Element {
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
        <div className={`form-group ${className ?? ''}`}>
            <label htmlFor="instrument-type">Instrument type:</label>
            <select
                aria-label="Instrument type"
                className="form-control"
                disabled={disabled}
                id="instrument-type"
                onChange={handleTypeChange}
                value={type}
            >
                {types.map(t => <option key={t} value={t}>{constants.INSTRUMENT_TYPE_TO_STRING[t]}</option>)}
            </select>
        </div>
    )
}

export default InstrumentTypeDropdown;