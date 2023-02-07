import React, { useState } from 'react';
import NumberInput from './NumberInput';

import Form from 'react-bootstrap/Form';
import OverlayTrigger from 'react-bootstrap/OverlayTrigger';
import ButtonGroup from 'react-bootstrap/ButtonGroup';
import Button from 'react-bootstrap/Button';
import Popover from 'react-bootstrap/Popover';

import { ChartToDateRange, FormFieldProps } from '../../../types';

/**
 * Renders a chart to-date range selector form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function ToDateRangeSelector(
    { disabled, label, className, value, onChange }: FormFieldProps<ChartToDateRange>
): JSX.Element {
    const [isCustomToDateRange, setIsCustomToDateRange] = useState(false);
    const toDateRanges: ChartToDateRange[] = [
        {unit: 'day', value: 1},
        {unit: 'day', value: 5},
        {unit: 'month', value: 1},
        {unit: 'month', value: 3},
        {unit: 'month', value: 6},
        {unit: 'year', value: 1}
    ];

    const handleRangeChange = (toDateRange: ChartToDateRange) => {
        if(!toDateRanges.reduce((prev, curr) => prev || toDateRange === curr, false)) {
            setIsCustomToDateRange(true);
        } else {
            setIsCustomToDateRange(false);
        }

        onChange && onChange(toDateRange);
    }

    return (
        <Form.Group className={className} controlId="form-to-date-range">
            <Form.Label>{label}:</Form.Label>
            <ButtonGroup>
                {
                    toDateRanges.map(range =>
                        <Button
                            disabled={disabled}
                            variant={
                                !isCustomToDateRange
                                    && value
                                    && value.unit === range.unit
                                    && value.value === range.value
                                ? 'dark'
                                : 'light'
                            }
                            size="sm"
                            key={range.value + range.unit}
                            onClick={() => handleRangeChange(range)}
                        >
                            {range.value + range.unit[0].toUpperCase()}
                        </Button>
                    )
                }
                <OverlayTrigger
                    trigger="click"
                    rootClose
                    placement="bottom"
                    overlay={
                        <Popover>
                            <Popover.Body>
                                <NumberInput
                                    disabled={disabled}
                                    label="Custom range (in days)"
                                    onChange={(days) =>
                                        handleRangeChange({ unit: 'day', value: days })}
                                />
                            </Popover.Body>
                        </Popover>
                    }
                >
                    <Button
                        disabled={disabled}
                        variant={isCustomToDateRange ? 'dark' : 'light'}
                        size="sm"
                    >
                        Custom
                    </Button>
                </OverlayTrigger>
            </ButtonGroup>
        </Form.Group>
    )
}

export default ToDateRangeSelector;