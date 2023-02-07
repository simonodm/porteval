import React, { useState } from 'react'
import useUserSettings from '../../hooks/useUserSettings';
import TextInput from './fields/TextInput';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { isValidDateTimeFormat } from '../../utils/string';

type Props = {
    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess?: () => void;

    /**
     * A callback which is invoked whenever the form fails to submit.
     */
    onFailure?: (error: string) => void;
}

/**
 * Renders a user settings edit form.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function SettingsForm({ onSuccess, onFailure }: Props): JSX.Element {
    const [settings, setSettings] = useUserSettings();

    const [dateFormatValue, setDateFormatValue] = useState(settings.dateFormat);
    const [timeFormatValue, setTimeFormatValue] = useState(settings.timeFormat);
    const [decimalSeparatorValue, setDecimalSeparatorValue] = useState(settings.decimalSeparator);
    const [thousandsSeparatorValue, setThousandsSeparatorValue] = useState(settings.thousandsSeparator);

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if(!isValidDateTimeFormat(dateFormatValue) || !isValidDateTimeFormat(timeFormatValue)) {
            onFailure && onFailure('Invalid date or time format.');
            return;
        }

        setSettings({
            dateFormat: dateFormatValue,
            timeFormat: timeFormatValue,
            decimalSeparator: decimalSeparatorValue,
            thousandsSeparator: thousandsSeparatorValue
        });

        onSuccess && onSuccess();
    }

    return (
        <Form onSubmit={handleSubmit} aria-label="Settings form">
            <TextInput className="mb-3" label="Date format" onChange={setDateFormatValue}
                value={dateFormatValue}
            />
            <TextInput className="mb-3" label="Time format" onChange={setTimeFormatValue}
                value={timeFormatValue}
            />
            <TextInput className="mb-3" label="Decimal separator" onChange={setDecimalSeparatorValue}
                value={decimalSeparatorValue}
            />
            <TextInput className="mb-3" label="Thousands separator" onChange={setThousandsSeparatorValue}
                value={thousandsSeparatorValue}
            />
            <Button 
                variant="primary"
                type="submit"
            >Save
            </Button>
        </Form>
    )
}

export default SettingsForm;