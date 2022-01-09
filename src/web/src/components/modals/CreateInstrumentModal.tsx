import React from 'react';
import { useCreateInstrumentMutation } from '../../redux/api/instrumentApi';
import { InstrumentType, ModalCallbacks } from '../../types';
import InstrumentForm from '../forms/InstrumentForm';
import { onSuccessfulResponse } from '../utils/modal';

export default function CreateInstrumentModal({ closeModal }: ModalCallbacks): JSX.Element {
    const [ createInstrument ] = useCreateInstrumentMutation();

    const handleSubmit = (name: string, symbol: string, exchange: string, type: InstrumentType, currencyCode: string, note: string) => {
        const instrument = {
            name,
            symbol,
            exchange,
            type,
            currencyCode,
            note
        };

        createInstrument(instrument).then((val) => {
            onSuccessfulResponse(val, closeModal);
        });
    }

    return (
        <InstrumentForm onSubmit={handleSubmit} />
    )
}