import React from 'react';
import { useUpdateInstrumentMutation } from '../../redux/api/instrumentApi';
import { Instrument, InstrumentType, ModalCallbacks } from '../../types';
import InstrumentForm from '../forms/InstrumentForm';
import { onSuccessfulResponse } from '../utils/modal';

type Props = {
    instrument: Instrument;
} & ModalCallbacks;

export default function EditInstrumentModal({ instrument, closeModal }: Props): JSX.Element {
    const [ updateInstrument ] = useUpdateInstrumentMutation();

    const handleSubmit = (name: string, symbol: string, exchange: string, type: InstrumentType, currencyCode: string, note: string) => {
        const updatedInstrument = {
            id: instrument.id,
            name,
            symbol,
            exchange,
            type,
            currencyCode,
            note
        };

        updateInstrument(updatedInstrument).then((val) => {
            onSuccessfulResponse(val, closeModal);
        });
    }

    return (
        <InstrumentForm 
            name={instrument.name}
            symbol={instrument.symbol}
            exchange={instrument.exchange}
            type={instrument.type}
            currencyCode={instrument.currencyCode}
            note={instrument.note}
            onSubmit={handleSubmit} />
    )
}