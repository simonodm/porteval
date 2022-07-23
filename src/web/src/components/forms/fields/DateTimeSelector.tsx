import React, { useEffect, useState } from 'react';
import DatePicker from 'react-datepicker';

type Props = {
    label: string;
    className?: string;
    dateFormat?: string;
    timeFormat?: string;
    disabled?: boolean;
    enableTime?: boolean;
    timeInterval?: number;
    value?: Date;
    onChange?: (dt: Date) => void;
}

export default function DateTimeSelector({
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