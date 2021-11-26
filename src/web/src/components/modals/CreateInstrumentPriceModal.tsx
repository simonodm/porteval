import React from 'react';
import { DateTime } from 'luxon';
import { useAddInstrumentPriceMutation } from '../../redux/api/instrumentApi';
import { Instrument } from '../../types';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import InstrumentPriceForm from '../forms/InstrumentPriceForm';

type Props = {
    instrument: Instrument;
    closeModal: () => void;
}

export default function CreateInstrumentPriceModal({ instrument, closeModal }: Props): JSX.Element {
    const [addPriceMutation, mutationStatus] = useAddInstrumentPriceMutation();

    const handleSubmit = (instrumentId: number, price: number, time: DateTime) => {
        addPriceMutation({
            instrumentId: instrument.id,
            price: price,
            time: time.toISO()
        }).then(closeModal);
    }

    const isLoaded = checkIsLoaded(mutationStatus);
    const isError = checkIsError(mutationStatus);
    
    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <InstrumentPriceForm onSubmit={handleSubmit} instrumentId={instrument.id} />
        </LoadingWrapper>
    )
}