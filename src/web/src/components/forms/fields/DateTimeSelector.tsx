import React, { useState } from 'react';
import { DateTime } from 'luxon';
import DatePicker from 'react-datepicker';
import { DEFAULT_DT_SELECTOR_FORMAT } from '../../../constants';

type Props = {
    label: string;
    format?: string;
    enableTime?: boolean;
    timeInterval?: number;
    defaultTime?: DateTime;
    onChange: (dt: DateTime) => void;
}

export default function DateTimeSelector({ label, format, enableTime, timeInterval, defaultTime, onChange }: Props): JSX.Element {
    const [time, setTime] = useState(defaultTime ?? DateTime.now());

    const handleTimeChange = (dt: Date) => {
        const convertedDT = DateTime.fromJSDate(dt);
        
        setTime(convertedDT);
        onChange(convertedDT);
    }
    
    return (
        <div className="form-group">
                <label htmlFor="date">{label}:</label>
            <DatePicker
                selected={time.toJSDate()}
                onChange={handleTimeChange}
                showTimeSelect={!!enableTime}
                timeIntervals={timeInterval ?? 5}
                dateFormat={format ?? DEFAULT_DT_SELECTOR_FORMAT}
                id="date" />
        </div>
    )
}