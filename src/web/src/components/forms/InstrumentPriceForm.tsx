import { DateTime } from 'luxon';
import React, { useState } from 'react';
import DatePicker from 'react-datepicker';

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
            <div className="form-group">
                <label htmlFor="price">Price:</label>
                <input type="number" className="form-control" value={price} onChange={(e) => setPrice(parseFloat(e.target.value))} />
            </div>
            <div className="form-group">
                <label htmlFor="date">Date:</label>
                <DatePicker
                    selected={time.toJSDate()}
                    onChange={(date: Date) => setTime(DateTime.fromJSDate(date))}
                    showTimeSelect
                    timeIntervals={1}
                    dateFormat="MMM dd, yyyy, HH:mm"
                    id="date" />
            </div>
            <button role="button" className="btn btn-primary">Save</button>
        </form>
    )
}