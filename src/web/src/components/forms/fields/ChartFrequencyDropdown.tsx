import React from 'react';
import Form from 'react-bootstrap/Form';
import { AggregationFrequency, FormFieldProps } from '../../../types';
import { camelToProperCase } from '../../../utils/string';

/**
 * Renders a chart frequency dropdown form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function ChartFrequencyDropdown(
    { label, disabled, className, value, onChange }: FormFieldProps<AggregationFrequency>
): JSX.Element {
    const frequencies: AggregationFrequency[] = ['day', 'week', 'month', 'year'];

    const handleFrequencyChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newFrequency = e.target.value as AggregationFrequency;
        onChange && onChange(newFrequency);
    }

    return (
        <Form.Group className={className} controlId="form-chart-frequency">
            <Form.Label>{label}:</Form.Label>
            <Form.Select disabled={disabled} onChange={handleFrequencyChange}>
                {frequencies.map(frequency =>
                    <option
                        key={frequency}
                        selected={
                            value === frequency
                        }
                        value={frequency}
                    >{camelToProperCase(frequency)}
                    </option>)}
            </Form.Select>
        </Form.Group>
    )
}

export default ChartFrequencyDropdown;