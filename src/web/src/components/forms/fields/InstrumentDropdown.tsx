import React, { useState } from 'react';
import { Instrument } from '../../../types';

type Props = {
    instruments: Array<Instrument>;
    defaultInstrumentId?: number;
    onChange: (instrumentId: number) => void;
}

export default function InstrumentDropdown({ instruments, defaultInstrumentId, onChange }: Props): JSX.Element {
    const [instrumentId, setInstrumentId] = useState(defaultInstrumentId);

    const handleInstrumentIdChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newInstrumentId = parseInt(e.target.value);
        
        if(!isNaN(newInstrumentId)) {
            setInstrumentId(newInstrumentId);
            onChange(newInstrumentId);
        }
    }

    return (
        <div className="form-group">
            <label htmlFor="position-instrument">Instrument:</label>
            <select id="position-instrument" className="form-control" disabled={defaultInstrumentId !== undefined} onChange={handleInstrumentIdChange}>
                {instruments
                        .filter(instrument => instrument.type !== 'index')
                        .map(instrument => <option value={instrument.id} selected={instrument.id === instrumentId}>{instrument.name}</option>)}
            </select>
        </div>
    )
}