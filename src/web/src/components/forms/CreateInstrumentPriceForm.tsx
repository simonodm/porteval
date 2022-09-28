import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import useUserSettings from '../../hooks/useUserSettings';
import DateTimeSelector from './fields/DateTimeSelector';
import NumberInput from './fields/NumberInput';

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
            <form onSubmit={handleSubmit}>
                <NumberInput allowFloat label='Price'
                    onChange={(newPrice) => setPrice(newPrice)} value={price}
                />
                <DateTimeSelector
                    dateFormat={userSettings.dateFormat}
                    enableTime
                    label='Date'
                    onChange={(newTime) => setTime(newTime)}
                    timeFormat={userSettings.timeFormat}
                    timeInterval={1}
                    value={time}
                /> 
                <button className="btn btn-primary" role="button">Save</button>
            </form>
        </LoadingWrapper>        
    )
}

export default CreateInstrumentPriceForm;