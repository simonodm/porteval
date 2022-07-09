import React, { useEffect, useState } from 'react';

import { InstrumentType } from '../../../types';
import * as constants from '../../../constants';

type Props = {
    className?: string;
    value?: InstrumentType;
    disabled?: boolean;
    onChange?: (type: InstrumentType) => void;
}

export default function InstrumentTypeDropdown({ className, value, disabled, onChange }: Props): JSX.Element {
    const types: Array<InstrumentType> =
        ['stock', 'bond', 'mutualFund', 'commodity', 'cryptoCurrency', 'etf', 'index', 'other']
    const [type, setType] = useState<InstrumentType>(value ?? 'stock');

    const handleTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newType = e.target.value as InstrumentType;
        setType(newType)
        onChange && onChange(newType);
    }

    useEffect(() => {
        if(value !== undefined) {
            setType(value);
        }
    }, [value]);

    useEffect(() => {
        if(value === undefined) {
            setType('stock');
            onChange && onChange('stock');
        }
    }, [])

    return (
        <div className={`form-group ${className ?? ''}`}>
            <label htmlFor="instrument-type">Type:</label>
            <select
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