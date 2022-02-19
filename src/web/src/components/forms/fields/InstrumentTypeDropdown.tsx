import React, { useState } from 'react';
import { InstrumentType } from '../../../types';
import * as constants from '../../../constants';

type Props = {
    defaultType?: InstrumentType;
    onChange: (type: InstrumentType) => void;
}

export default function InstrumentTypeDropdown({ defaultType, onChange }: Props): JSX.Element {
    const types: Array<InstrumentType> = ['stock', 'bond', 'mutualFund', 'commodity', 'cryptoCurrency', 'etf', 'index', 'other']
    const [ type, setType ] = useState<InstrumentType>(defaultType ?? 'stock');

    const handleTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const type = e.target.value as InstrumentType;
        setType(type)
        onChange(type);
    }

    return (
        <div className="form-group">
            <label htmlFor="instrument-type">Type:</label>
            <select
                id="instrument-type"
                className="form-control"
                defaultValue={type}
                disabled={defaultType !== undefined}
                onChange={handleTypeChange}>
                    {types.map(type => <option value={type}>{constants.INSTRUMENT_TYPE_TO_STRING[type]}</option>)}
            </select>
        </div>
    )
}