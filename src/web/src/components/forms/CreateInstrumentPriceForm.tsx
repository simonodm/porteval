import { DateTime } from 'luxon';
import React, { useState } from 'react';
import { useAddInstrumentPriceMutation } from '../../redux/api/instrumentApi';
import DateTimeSelector from './fields/DateTimeSelector';
import NumberInput from './fields/NumberInput';
import { checkIsLoaded, onSuccessfulResponse } from '../utils/queries';
import LoadingWrapper from '../ui/LoadingWrapper';

type Props = {
    instrumentId: number;
    onSuccess?: () => void;
}

export default function CreateInstrumentPriceForm({ instrumentId, onSuccess }: Props): JSX.Element {
    const [addPrice, mutationStatus] = useAddInstrumentPriceMutation();

    const [price, setPrice] = useState(0);
    const [time, setTime] = useState(DateTime.now());

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        addPrice({
            instrumentId,
            price,
            time: time.toISO()
        }).then(res => onSuccessfulResponse(res, onSuccess));

        e.preventDefault();
    }
    
    const isLoaded = checkIsLoaded(mutationStatus);

    return (
        <LoadingWrapper isLoaded={isLoaded}>
            <form onSubmit={handleSubmit}>
                <NumberInput label='Price' value={price} allowFloat allowNegativeValues onChange={(newPrice) => setPrice(newPrice)} />
                <DateTimeSelector label='Date' value={time} enableTime timeInterval={1} format='MMM dd, yyyy, HH:mm' onChange={(newTime) => setTime(newTime)} /> 
                <button role="button" className="btn btn-primary">Save</button>
            </form>
        </LoadingWrapper>        
    )
}