import React from 'react';
import { DateTime } from 'luxon';
import { useAddInstrumentPriceMutation } from '../../redux/api/instrumentApi';
import { Instrument } from '../../types';
import InstrumentPriceForm from '../forms/InstrumentPriceForm';
import { onSuccessfulResponse } from '../utils/modal';

type Props = {
    instrument: Instrument;
    closeModal: () => void;
}

export default function CreateInstrumentPriceModal({ instrument, closeModal }: Props): JSX.Element {
    const [addPriceMutation] = useAddInstrumentPriceMutation();

    const handleSubmit = (instrumentId: number, price: number, time: DateTime) => {
        addPriceMutation({
            instrumentId: instrument.id,
            price: price,
            time: time.toISO()
        }).then((val) => {
            onSuccessfulResponse(val, closeModal);
        });
    }
    
    return (
        <InstrumentPriceForm onSubmit={handleSubmit} instrumentId={instrument.id} />
    )
}