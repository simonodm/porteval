import React, { useEffect, useState } from 'react';
import { DateTime } from 'luxon';
import DatePicker from 'react-datepicker';

type Props = {
    label: string;
    className?: string;
    dateFormat?: string;
    timeFormat?: string;
    disabled?: boolean;
    enableTime?: boolean;
    timeInterval?: number;
    value?: DateTime;
    onChange?: (dt: DateTime) => void;
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
    const [time, setTime] = useState<DateTime | undefined>(value ?? DateTime.now());

    const handleTimeChange = (dt: Date) => {
        const convertedDT = DateTime.fromJSDate(dt);
        
        setTime(convertedDT);
        onChange && onChange(convertedDT);
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
                selected={time?.toJSDate()}
                showTimeSelect={!!enableTime}
                timeFormat={timeFormat}
                timeIntervals={timeInterval ?? 5}
            />
        </div>
    )
}