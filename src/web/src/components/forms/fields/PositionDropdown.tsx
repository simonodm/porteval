import React, { useEffect, useState } from 'react';
import Form from 'react-bootstrap/Form';
import { FormFieldProps, Position } from '../../../types';

type Props = FormFieldProps<number> & {
    /**
     * An array of positions to display in the dropdown.
     */
    positions: Array<Position>;
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

    // adjust internal state on `value` prop change
    useEffect(() => {
        if(value !== undefined) {
            setPositionId(value);
        }
    }, [value]);

    // simulate dropdown value change to first available value if source positions change
    useEffect(() => {
        if(!positionId && positions.length > 0) {
            setPositionId(positions[0].id);
            onChange && onChange(positions[0].id);
        }
    }, [positions]);

    return (
        <Form.Group className={className} controlId="form-position">
            <Form.Label>Position:</Form.Label>
            <Form.Select disabled={disabled}
                onChange={handlePositionChange} value={positionId} aria-label="Position"
            >
                {positions.map(position =>
                    <option key={position.id} value={position.id}>{position.instrument.name}</option>)}
            </Form.Select>
        </Form.Group>
    )
}

export default PositionDropdown;