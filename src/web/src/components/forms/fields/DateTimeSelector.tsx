import React, { useEffect, useState } from 'react';
import DatePicker from 'react-datepicker';
import Form from 'react-bootstrap/Form';
import { FormFieldProps } from '../../../types';
import 'react-datepicker/dist/react-datepicker.css';

type Props = FormFieldProps<Date> & {
    /**
     * Date format to display the selected date in.
     */
    dateFormat?: string;

    /**
     * Time format to display the selected time in.
     */
    timeFormat?: string;

    /**
     * Determines whether time can be selected.
     */
    enableTime?: boolean;

    /**
     * Determines discrete time intervals to display in the selector. The value is in minutes.
     */
    timeInterval?: number;
}

/**
 * Renders a date/time selector form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function DateTimeSelector({
    label,
    className,
    dateFormat,
    timeFormat,
    enableTime,
    disabled,
    timeInterval,
    value,
    onChange
}: Props): JSX.Element {
    const [time, setTime] = useState<Date | undefined>(value ?? new Date());

    const handleTimeChange = (dt: Date) => {        
        setTime(dt);
        onChange && onChange(dt);
    }
    
    // adjust internal state on `value` prop change
    useEffect(() => {
        setTime(value);
    }, [value]);

    return (
        <Form.Group className={className}>
            <Form.Label htmlFor="form-date">{label}:</Form.Label>
            <DatePicker
                ariaLabelledBy={label}
                dateFormat={dateFormat && timeFormat ? dateFormat + ' ' + timeFormat : dateFormat}
                disabled={disabled}
                id="form-date"
                onChange={handleTimeChange}
                selected={time}
                showTimeSelect={!!enableTime}
                timeFormat={timeFormat}
                timeIntervals={timeInterval ?? 5}
            />
        </Form.Group>
    )
}

export default DateTimeSelector;