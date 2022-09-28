import React, { useEffect, useState } from 'react';
import { Position } from '../../../types';

type Props = {
    /**
     * An array of positions to display in the dropdown.
     */
    positions: Array<Position>;

    /**
     * Custom class name to use for the form field.
     */
    className?: string;

    /**
     * Determines whether the form field is disabled.
     */
    disabled?: boolean;

    /**
     * Binding property for the dropdown's current position ID.
     */
    value?: number;

    /**
     * A callback which is invoked whenever dropdown's selection changes.
     */
    onChange?: (positionId: number) => void;
}

/**
 * Renders a position dropdown form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component 
 */
function PositionDropdown({ positions, className, disabled, value, onChange }: Props): JSX.Element {
    const [positionId, setPositionId] = useState(value ?? (positions.length > 0 ? positions[0].id : undefined));

    const handlePositionChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newPositionId = parseInt(e.target.value);
        if(!isNaN(newPositionId)) {
            setPositionId(newPositionId);
            onChange && onChange(newPositionId);
        }
    }

    useEffect(() => {
        if(value !== undefined) {
            setPositionId(value);
        }
    }, [value]);

    useEffect(() => {
        if(!positionId && positions.length > 0) {
            setPositionId(positions[0].id);
            onChange && onChange(positions[0].id);
        }
    }, [positions]);

    return (
        <div className={`form-group ${className ?? ''}`}>
            <label htmlFor="position">Instrument:</label>
            <select className="form-control" disabled={disabled} id="portfolio-position"
                onChange={handlePositionChange} value={positionId}
            >
                {positions.map(position =>
                    <option key={position.id} value={position.id}>{position.instrument.name}</option>)}
            </select>
        </div>
    )
}

export default PositionDropdown;