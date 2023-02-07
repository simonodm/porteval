import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import useUserSettings from '../../hooks/useUserSettings';
import DateTimeSelector from './fields/DateTimeSelector';
import NumberInput from './fields/NumberInput';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { useAddInstrumentPriceMutation } from '../../redux/api/instrumentApi';
import { checkIsLoaded, onSuccessfulResponse } from '../../utils/queries';

type Props = {
    /**
     * ID of the instrument to create price for.
     */
    instrumentId: number;

    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess?: () => void;
}

/**
 * Renders an instrument price creation form.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function CreateInstrumentPriceForm({ instrumentId, onSuccess }: Props): JSX.Element {
    const [addPrice, mutationStatus] = useAddInstrumentPriceMutation();

    const [price, setPrice] = useState(0);
    const [time, setTime] = useState(new Date());

    const [userSettings] = useUserSettings();

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        addPrice({
            instrumentId,
            price,
            time: time.toISOString()
        }).then(res => onSuccessfulResponse(res, onSuccess));

        e.preventDefault();
    }
    
    const isLoaded = checkIsLoaded(mutationStatus);

    return (
        <LoadingWrapper isLoaded={isLoaded}>
            <Form onSubmit={handleSubmit} aria-label="Create instrument price form">
                <NumberInput className="mb-3" allowFloat label='Price'
                    onChange={(newPrice) => setPrice(newPrice)} value={price}
                />
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
                <Button type="submit" variant="primary">Save</Button>
            </Form>
        </LoadingWrapper>        
    )
}

export default CreateInstrumentPriceForm;