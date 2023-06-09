import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import useUserSettings from '../../hooks/useUserSettings';
import DateTimeSelector from './fields/DateTimeSelector';
import NumberInput from './fields/NumberInput';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { useCreateInstrumentSplitMutation } from '../../redux/api/instrumentApi';
import { checkIsLoaded, onSuccessfulResponse } from '../../utils/queries';

type Props = {
    /**
     * ID of the instrument to create a split for.
     */
    instrumentId: number;

    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess?: () => void;
}

/**
 * Renders an instrument split creation form.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function CreateInstrumentSplitForm({ instrumentId, onSuccess }: Props): JSX.Element {
    const [createSplit, mutationStatus] = useCreateInstrumentSplitMutation();

    const [time, setTime] = useState(new Date());
    const [numerator, setNumerator] = useState(2);
    const [denominator, setDenominator] = useState(1);

    const [userSettings] = useUserSettings();

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        createSplit({
            instrumentId,
            time: time.toISOString(),
            splitRatioDenominator: denominator,
            splitRatioNumerator: numerator
        }).then(res => onSuccessfulResponse(res, onSuccess));

        e.preventDefault();
    }
    
    const isLoaded = checkIsLoaded(mutationStatus);

    return (
        <LoadingWrapper isLoaded={isLoaded}>
            <Form aria-label='Create instrument split form' onSubmit={handleSubmit}>
                <DateTimeSelector
                    className="mb-3" 
                    dateFormat={userSettings.dateFormat}
                    enableTime
                    label='Date'
                    onChange={(newTime) => setTime(newTime)}
                    timeFormat={userSettings.timeFormat}
                    timeInterval={1}
                    value={time}
                /> 
                <NumberInput
                    className="mb-3" 
                    label='Split factor numerator'
                    onChange={(n) => setNumerator(n)}
                    value={numerator}
                />
                <NumberInput
                    className="mb-3" 
                    label='Split factor denominator'
                    onChange={(n) => setDenominator(n)}
                    value={denominator}
                />
                <Button variant="primary" type="submit">Save</Button>
            </Form>
        </LoadingWrapper>        
    )
}

export default CreateInstrumentSplitForm;