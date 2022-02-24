import React, { useState } from 'react';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../utils/queries';
import { Instrument, InstrumentType } from '../../types';
import TextInput from './fields/TextInput';
import InstrumentTypeDropdown from './fields/InstrumentTypeDropdown';
import CurrencyDropdown from './fields/CurrencyDropdown';
import { useUpdateInstrumentMutation } from '../../redux/api/instrumentApi';

type Props = {
    instrument: Instrument;
    onSuccess?: () => void;
}

export default function EditInstrumentForm({ instrument, onSuccess }: Props): JSX.Element {
    const currencies = useGetAllKnownCurrenciesQuery();
    const [updateInstrument, mutationStatus] = useUpdateInstrumentMutation();

    const [name, setName] = useState(instrument.name);
    const [symbol, setSymbol] = useState(instrument.symbol);
    const [exchange, setExchange] = useState(instrument.exchange);
    const [currencyCode, setCurrencyCode] = useState(instrument.currencyCode);
    const [note, setNote] = useState(instrument.note);
    const [type, setType] = useState<InstrumentType>(instrument.type);

    const isLoaded = checkIsLoaded(currencies, mutationStatus);
    const isError = checkIsError(currencies);

    const handleSubmit = (e: React.FormEvent) => {
        updateInstrument({
            id: instrument.id,
            name,
            symbol,
            exchange,
            currencyCode,
            note,
            type
        }).then(res => onSuccessfulResponse(res, onSuccess));

        e.preventDefault();
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={handleSubmit}>
                <TextInput label='Name' value={name} onChange={(val) => setName(val)} />
                <TextInput label='Symbol' disabled value={symbol} onChange={(val) => setSymbol(val)} />
                <TextInput label='Exchange' value={exchange} onChange={(val) => setExchange(val)} />
                <InstrumentTypeDropdown value={type} disabled onChange={(t) => setType(t)} />
                <CurrencyDropdown currencies={currencies.data ?? []} disabled value={currencyCode} onChange={(code) => setCurrencyCode(code)} />
                <TextInput label='Note' value={note} onChange={(val) => setNote(val)} />
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
    )
}