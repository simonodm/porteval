import React, { useState } from 'react'

import useUserSettings from '../../hooks/useUserSettings';

import TextInput from './fields/TextInput';

type Props = {
    onSuccess?: () => void;
}

export default function SettingsForm({ onSuccess }: Props): JSX.Element {
    const [settings, setSettings] = useUserSettings();

    const [dateFormatValue, setDateFormatValue] = useState(settings.dateFormat);
    const [timeFormatValue, setTimeFormatValue] = useState(settings.timeFormat);
    const [decimalSeparatorValue, setDecimalSeparatorValue] = useState(settings.decimalSeparator);

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        setSettings({
            dateFormat: dateFormatValue,
            timeFormat: timeFormatValue,
            decimalSeparator: decimalSeparatorValue
        });
        e.preventDefault();

        onSuccess && onSuccess();
    }

    return (
        <form onSubmit={handleSubmit}>
            <TextInput label="Date format" onChange={setDateFormatValue} value={dateFormatValue} />
            <TextInput label="Time format" onChange={setTimeFormatValue} value={timeFormatValue} />
            <TextInput label="Decimal separator" onChange={setDecimalSeparatorValue} value={decimalSeparatorValue} />
            <button 
                className="btn btn-primary"
                role="button"
            >Save
            </button>
        </form>
    )
}