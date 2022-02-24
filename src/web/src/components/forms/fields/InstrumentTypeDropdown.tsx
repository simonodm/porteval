import React, { useEffect, useState } from 'react';
import { InstrumentType } from '../../../types';
import * as constants from '../../../constants';

type Props = {
    value?: InstrumentType;
    disabled?: boolean;
    onChange?: (type: InstrumentType) => void;
}

export default function InstrumentTypeDropdown({ value, disabled, onChange }: Props): JSX.Element {
    const types: Array<InstrumentType> = ['stock', 'bond', 'mutualFund', 'commodity', 'cryptoCurrency', 'etf', 'index', 'other']
    const [type, setType] = useState<InstrumentType>(value ?? 'stock');

    const handleTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const type = e.target.value as InstrumentType;
        setType(type)
        onChange && onChange(type);
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
        <div className="form-group">
            <label htmlFor="instrument-type">Type:</label>
            <select
                id="instrument-type"
                className="form-control"
                value={type}
                disabled={disabled}
                onChange={handleTypeChange}>
                    {types.map(t => <option value={t}>{constants.INSTRUMENT_TYPE_TO_STRING[type]}</option>)}
            </select>
        </div>
    )
}