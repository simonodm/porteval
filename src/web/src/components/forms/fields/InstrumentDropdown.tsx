import React, { useEffect, useState } from 'react';
import { Instrument } from '../../../types';

type Props = {
    instruments: Array<Instrument>;
    value?: number;
    creatable?: boolean;
    disabled?: boolean;
    onCreate?: () => void;
    onCancelCreate?: () => void;
    onChange?: (instrumentId: number) => void;
}

export default function InstrumentDropdown({ instruments, value, disabled, creatable, onCreate, onCancelCreate, onChange }: Props): JSX.Element {
    const [instrumentId, setInstrumentId] = useState(value);
    const [creatingNew, setCreatingNew] = useState(false);

    const handleInstrumentIdChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newInstrumentId = parseInt(e.target.value);
        
        if(!isNaN(newInstrumentId)) {
            setInstrumentId(newInstrumentId);
            onChange && onChange(newInstrumentId);
        }
    }

    const handleCreateNewInstrumentCheck = () => {
        if(!creatingNew && onCreate) {
            onCreate();
        }
        else if(creatingNew && onCancelCreate) {
            onCancelCreate();
        }

        setCreatingNew(!creatingNew);
    }

    useEffect(() => {
        if(!instrumentId && instruments.length > 0) {
            setInstrumentId(instruments[0].id);
            onChange && onChange(instruments[0].id);
        }
    }, [instruments]);

    useEffect(() => {
        if(value !== undefined) {
            setInstrumentId(value);
        }
    }, [value]);

    return (
        <div className="form-group">
            <label htmlFor="instrument">Instrument:</label>
            <select id="instrument" className="form-control" value={instrumentId} disabled={disabled || creatingNew} onChange={handleInstrumentIdChange}>
                {instruments
                        .filter(instrument => instrument.type !== 'index')
                        .map(instrument => <option value={instrument.id}>{instrument.name}</option>)}
            </select>
            {
                creatable &&
                <>
                    <input id="instrument-create-new" type="checkbox" checked={creatingNew} onChange={handleCreateNewInstrumentCheck} />
                    <label htmlFor="instrument-create-new">Create new instrument</label>
                </>
            }
            
        </div>
    )
}