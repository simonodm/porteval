import React, { useState, useEffect } from 'react';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../utils/queries';
import { InstrumentType } from '../../types';
import TextInput from './fields/TextInput';
import InstrumentTypeDropdown from './fields/InstrumentTypeDropdown';
import CurrencyDropdown from './fields/CurrencyDropdown';
import { useCreateInstrumentMutation } from '../../redux/api/instrumentApi';

type Props = {
    onSuccess?: () => void;
}

export default function CreateInstrumentForm({ onSuccess }: Props): JSX.Element {
    const currencies = useGetAllKnownCurrenciesQuery();
    const [createInstrument, mutationStatus] = useCreateInstrumentMutation();

    const [name, setName] = useState('');
    const [symbol, setSymbol] = useState('');
    const [exchange, setExchange] = useState('');
    const [currencyCode, setCurrencyCode] = useState('');
    const [note, setNote] = useState('');
    const [type, setType] = useState<InstrumentType>('stock');

    const isLoaded = checkIsLoaded(currencies, mutationStatus);
    const isError = checkIsError(currencies);

    useEffect(() => {
        if(currencies.data) {
            const defaultCurrency = currencies.data.find(c => c.isDefault);
            if(defaultCurrency !== undefined) {
                setCurrencyCode(defaultCurrency.code);
            }
        }
    }, [currencies.data]);

    const handleSubmit = (e: React.FormEvent) => {
        createInstrument({
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
                <TextInput label='Name' value={name} placeholder='e.g. Apple Inc.' onChange={(val) => setName(val)} />
                <TextInput label='Symbol' value={symbol} placeholder='e.g. AAPL' onChange={(val) => setSymbol(val)} />
                <TextInput label='Exchange' value={exchange} placeholder='e.g. NASDAQ' onChange={(val) => setExchange(val)} />
                <InstrumentTypeDropdown value={type} onChange={(t) => setType(t)} />
                <CurrencyDropdown currencies={currencies.data ?? []} value={currencyCode} onChange={(code) => setCurrencyCode(code)} />
                <TextInput label='Note' value={note} onChange={(val) => setNote(val)} />
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
    )
}