import React, { useEffect, useState } from 'react';

import { Position } from '../../../types';

type Props = {
    positions: Array<Position>;
    disabled?: boolean;
    value?: number;
    onChange?: (positionId: number) => void;
}

export default function PositionDropdown({ positions, disabled, value, onChange }: Props): JSX.Element {
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
        <div className="form-group">
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