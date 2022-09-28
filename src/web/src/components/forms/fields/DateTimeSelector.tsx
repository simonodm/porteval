import React, { useEffect, useState } from 'react';
import DatePicker from 'react-datepicker';

type Props = {
    /**
     * Custom label to use for the form field.
     */
    label: string;

    /**
     * Custom class name to use for the form field.
     */
    className?: string;

    /**
     * Date format to display the selected date in.
     */
    dateFormat?: string;

    /**
     * Time format to display the selected time in.
     */
    timeFormat?: string;

    /**
     * Determines whether the field is disabled.
     */
    disabled?: boolean;

    /**
     * Determines whether time can be selected.
     */
    enableTime?: boolean;

    /**
     * Determines discrete time intervals to display in the selector. The value is in minutes.
     */
    timeInterval?: number;

    /**
     * Binding property for the selector's current value.
     */
    value?: Date;

    /**
     * A callback which is invoked every time the selected date/time changes.
     */
    onChange?: (dt: Date) => void;
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
    
    useEffect(() => {
        setTime(value);
    }, [value]);

    return (
        <div className={`form-group ${className ?? ''}`}>
            <label htmlFor="date">{label}:</label>
            <DatePicker
                dateFormat={dateFormat && timeFormat ? dateFormat + ' ' + timeFormat : dateFormat}
                disabled={disabled}
                id="date"
                onChange={handleTimeChange}
                selected={time}
                showTimeSelect={!!enableTime}
                timeFormat={timeFormat}
                timeIntervals={timeInterval ?? 5}
            />
        </div>
    )
}

export default DateTimeSelector;