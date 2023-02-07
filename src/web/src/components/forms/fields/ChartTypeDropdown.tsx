import React from 'react';
import Form from 'react-bootstrap/Form';
import { ChartType, FormFieldProps } from '../../../types';
import { camelToProperCase } from '../../../utils/string';

/**
 * Renders a chart type dropdown form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function ChartTypeDropdown(
    { label, disabled, className, value, onChange }: FormFieldProps<ChartType>
): JSX.Element {
    const types: ChartType[] = ['price', 'profit', 'performance', 'aggregatedProfit', 'aggregatedPerformance'];

    const handleTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newType = e.target.value as ChartType;

        onChange && onChange(newType);
    }

    return (
        <Form.Group className={className} controlId="form-chart-type">
            <Form.Label>{label}:</Form.Label>
            <Form.Select disabled={disabled} aria-label='Chart type'
                onChange={handleTypeChange}
            >
                {types.map(type =>
                    <option
                        key={type}
                        selected={value === type}
                        value={type}
                    >{camelToProperCase(type)}
                    </option>)}
            </Form.Select>
        </Form.Group>
    )
}

export default ChartTypeDropdown;