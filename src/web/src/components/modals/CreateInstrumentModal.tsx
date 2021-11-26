import React from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useCreateInstrumentMutation } from '../../redux/api/instrumentApi';
import { InstrumentType, ModalCallbacks } from '../../types';
import InstrumentForm from '../forms/InstrumentForm';

export default function CreateInstrumentModal({ closeModal }: ModalCallbacks): JSX.Element {
    const [ createInstrument, mutationStatus] = useCreateInstrumentMutation();

    const isLoaded = checkIsLoaded(mutationStatus);
    const isError = checkIsError(mutationStatus);

    const handleSubmit = (name: string, symbol: string, exchange: string, type: InstrumentType, currencyCode: string) => {
        const instrument = {
            name,
            symbol,
            exchange,
            type,
            currencyCode
        };

        createInstrument(instrument).then(() => closeModal());
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <InstrumentForm onSubmit={handleSubmit} />
        </LoadingWrapper>
    )
}