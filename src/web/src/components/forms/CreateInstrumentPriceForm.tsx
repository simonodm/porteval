import { DateTime } from 'luxon';
import React, { useState } from 'react';

import { useAddInstrumentPriceMutation } from '../../redux/api/instrumentApi';

import { checkIsLoaded, onSuccessfulResponse } from '../utils/queries';

import LoadingWrapper from '../ui/LoadingWrapper';

import DateTimeSelector from './fields/DateTimeSelector';
import NumberInput from './fields/NumberInput';

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
                <NumberInput allowFloat allowNegativeValues label='Price'
                    onChange={(newPrice) => setPrice(newPrice)} value={price}
                />
                <DateTimeSelector enableTime format='MMM dd, yyyy, HH:mm' label='Date'
                    onChange={(newTime) => setTime(newTime)} timeInterval={1} value={time}
                /> 
                <button className="btn btn-primary" role="button">Save</button>
            </form>
        </LoadingWrapper>        
    )
}