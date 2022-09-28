import React, { useEffect, useState } from 'react';
import { Instrument } from '../../../types';

type Props = {
    /**
     * An array of instruments to display in the dropdown.
     */
    instruments: Array<Instrument>;

    /**
     * A custom class name to use for the form field.
     */
    className?: string;

    /**
     * Binding property for the dropdown's current instrument ID.
     */
    value?: number;

    /**
     * Determines whether user can specify to create a new instrument.
     */
    creatable?: boolean;

    /**
     * Determines whether the form field is disabled.
     */
    disabled?: boolean;

    /**
     * A callback which is invoked whenever a new instrument is requested to be created.
     */
    onCreate?: () => void;

    /**
     * A callback which is invoked whenever new instrument creation is cancelled.
     */
    onCancelCreate?: () => void;

    /**
     * A callback which is invoked whenever the dropdown's value changes.
     */
    onChange?: (instrumentId: number) => void;
}

/**
 * Renders an instrument dropdown form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function InstrumentDropdown(
    { instruments, className, value, disabled, creatable, onCreate, onCancelCreate, onChange }: Props
): JSX.Element {
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
        } else if(creatingNew && onCancelCreate) {
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
        <div className={`form-group ${className ?? ''}`}>
            <label htmlFor="instrument">Instrument:</label>
            <select className="form-control" disabled={disabled || creatingNew} id="instrument"
                onChange={handleInstrumentIdChange} value={instrumentId}
            >
                {instruments
                        .filter(instrument => instrument.type !== 'index')
                        .map(instrument =>
                            <option key={instrument.id} value={instrument.id}>{instrument.name}</option>)}
            </select>
            {
                creatable &&
                <>
                    <input checked={creatingNew} id="instrument-create-new" onChange={handleCreateNewInstrumentCheck}
                        type="checkbox"
                    />
                    <label htmlFor="instrument-create-new">Create new instrument</label>
                </>
            }
            
        </div>
    )
}

export default InstrumentDropdown;