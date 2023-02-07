import React, { useEffect, useState } from 'react';

import Form from 'react-bootstrap/Form';

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
     * Custom label to use for the form field.
     */
    label?: string;

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
    { instruments, className, label, value, disabled, creatable, onCreate, onCancelCreate, onChange }: Props
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

    // simulate dropdown value change to first available value if source instruments change
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
        <Form.Group className={className} controlId="form-instrument">
            <Form.Label>{label ?? 'Instrument'}:</Form.Label>
            <Form.Select disabled={disabled || creatingNew}
                onChange={handleInstrumentIdChange} value={instrumentId} aria-label='Instrument'
            >
                {instruments
                        .filter(instrument => instrument.type !== 'index')
                        .map(instrument =>
                            <option key={instrument.id} value={instrument.id}>{instrument.name}</option>)}
            </Form.Select>
            {
                creatable &&
                <Form.Group controlId="form-create-instrument">
                    <Form.Check
                        type="checkbox"
                        checked={creatingNew}
                        onChange={handleCreateNewInstrumentCheck}
                        label="Create new instrument"
                    />
                </Form.Group>
            }
            
        </Form.Group>
    )
}

export default InstrumentDropdown;