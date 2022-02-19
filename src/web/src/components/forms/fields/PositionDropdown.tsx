import React, { useState } from 'react';
import { Position } from '../../../types';

type Props = {
    positions: Array<Position>;
    defaultPositionId?: number;
    onChange: (positionId: number) => void;
}

export default function PositionDropdown({ positions, defaultPositionId, onChange }: Props): JSX.Element {
    const [positionId, setPositionId] = useState(defaultPositionId);

    const handlePositionChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newPositionId = parseInt(e.target.value);
        if(!isNaN(newPositionId)) {
            setPositionId(newPositionId);
            onChange(newPositionId);
        }
    }

    return (
        <div className="form-group">
            <label htmlFor="position">Instrument:</label>
            <select disabled={defaultPositionId !== undefined} id="portfolio-position" className="form-control" onChange={handlePositionChange}>
                {positions.map(position => <option value={position.id} selected={position.id === positionId}>{position.instrument.name}</option>)}
            </select>
        </div>
    )
}