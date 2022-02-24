import React, { useState, useEffect } from 'react';

import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../../utils/queries';
import { InstrumentType } from '../../types';

import { useCreateInstrumentMutation } from '../../redux/api/instrumentApi';

import TextInput from './fields/TextInput';
import InstrumentTypeDropdown from './fields/InstrumentTypeDropdown';
import CurrencyDropdown from './fields/CurrencyDropdown';

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
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <form onSubmit={handleSubmit}>
                <TextInput label='Name' onChange={(val) => setName(val)} placeholder='e.g. Apple Inc.'
                    value={name}
                />
                <TextInput label='Symbol' onChange={(val) => setSymbol(val)} placeholder='e.g. AAPL'
                    value={symbol}
                />
                <TextInput label='Exchange' onChange={(val) => setExchange(val)} placeholder='e.g. NASDAQ'
                    value={exchange}
                />
                <InstrumentTypeDropdown onChange={(t) => setType(t)} value={type} />
                <CurrencyDropdown
                    currencies={currencies.data ?? []}
                    onChange={(code) => setCurrencyCode(code)}
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