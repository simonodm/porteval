import React, { useEffect, useState } from 'react';
import { DateTime } from 'luxon';
import DatePicker from 'react-datepicker';
import { DEFAULT_DT_SELECTOR_FORMAT } from '../../../constants';

type Props = {
    label: string;
    disabled?: boolean;
    format?: string;
    enableTime?: boolean;
    timeInterval?: number;
    value?: DateTime;
    onChange?: (dt: DateTime) => void;
}

export default function DateTimeSelector({ label, format, enableTime, disabled, timeInterval, value, onChange }: Props): JSX.Element {
    const [time, setTime] = useState(value ?? DateTime.now());

    const handleTimeChange = (dt: Date) => {
        const convertedDT = DateTime.fromJSDate(dt);
        
        setTime(convertedDT);
        onChange && onChange(convertedDT);
    }
    
    useEffect(() => {
        if(value !== undefined) {
            setTime(value);
        }
    }, [value]);

    return (
        <div className="form-group">
            <label htmlFor="date">{label}:</label>
            <DatePicker
                selected={time.toJSDate()}
                onChange={handleTimeChange}
                disabled={disabled}
                showTimeSelect={!!enableTime}
                timeIntervals={timeInterval ?? 5}
                dateFormat={format ?? DEFAULT_DT_SELECTOR_FORMAT}
                id="date" />
        </div>
    )
}