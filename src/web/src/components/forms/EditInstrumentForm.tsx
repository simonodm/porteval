import React, { useState } from 'react';

import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../utils/queries';
import { Instrument, InstrumentType } from '../../types';

import { useUpdateInstrumentMutation } from '../../redux/api/instrumentApi';

import TextInput from './fields/TextInput';
import InstrumentTypeDropdown from './fields/InstrumentTypeDropdown';
import CurrencyDropdown from './fields/CurrencyDropdown';

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
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <form onSubmit={handleSubmit}>
                <TextInput label='Name' onChange={(val) => setName(val)} value={name} />
                <TextInput disabled label='Symbol' onChange={(val) => setSymbol(val)}
                    value={symbol}
                />
                <TextInput label='Exchange' onChange={(val) => setExchange(val)} value={exchange} />
                <InstrumentTypeDropdown disabled onChange={(t) => setType(t)} value={type} />
                <CurrencyDropdown currencies={currencies.data ?? []} disabled onChange={(code) => setCurrencyCode(code)}
                    value={currencyCode}
                />
                <TextInput label='Note' onChange={(val) => setNote(val)} value={note} />
                <button 
                    className="btn btn-primary"
                    role="button"
                >Save
                </button>
            </form>
        </LoadingWrapper>
    )
}