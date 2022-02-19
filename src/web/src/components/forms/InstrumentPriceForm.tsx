import { DateTime } from 'luxon';
import React, { useState } from 'react';
import DateTimeSelector from './fields/DateTimeSelector';
import NumberInput from './fields/NumberInput';

type Props = {
    instrumentId: number;
    onSubmit: (instrumentId: number, price: number, time: DateTime) => void;
}

export default function InstrumentPriceForm({ instrumentId, onSubmit }: Props): JSX.Element {
    const [price, setPrice] = useState(0);
    const [time, setTime] = useState(DateTime.now());

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        onSubmit(instrumentId, price, time);
        e.preventDefault();
    }
    
    return (
        <form onSubmit={handleSubmit}>
            <NumberInput label='Price' defaultValue={price} allowFloat allowNegativeValues onChange={(newPrice) => setPrice(newPrice)} />
            <DateTimeSelector label='Date' defaultTime={time} enableTime timeInterval={1} format='MMM dd, yyyy, HH:mm' onChange={(newTime) => setTime(newTime)} /> 
            <button role="button" className="btn btn-primary">Save</button>
        </form>
    )
}