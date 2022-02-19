import React, { useState } from 'react';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { InstrumentType } from '../../types';
import TextInput from './fields/TextInput';
import InstrumentTypeDropdown from './fields/InstrumentTypeDropdown';
import CurrencyDropdown from './fields/CurrencyDropdown';

type Props = {
    name?: string;
    symbol?: string;
    exchange?: string;
    currencyCode?: string;
    type?: InstrumentType;
    note?: string;
    onSubmit: (name: string, symbol: string, exchange: string, type: InstrumentType, currencyCode: string, note: string) => void;
}

export default function InstrumentForm({ name: originalName,
    symbol: originalSymbol,
    exchange: originalExchange,
    currencyCode: originalCurrencyCode,
    type: originalType,
    note: originalNote,
    onSubmit }: Props): JSX.Element {
    const currencies = useGetAllKnownCurrenciesQuery();

    const [ name, setName] = useState(originalName ?? '');
    const [ symbol, setSymbol ] = useState(originalSymbol ?? '');
    const [ exchange, setExchange ] = useState(originalExchange ?? '');
    const [ currencyCode, setCurrencyCode ] = useState(originalCurrencyCode ?? '');
    const [ note, setNote ] = useState(originalNote ?? '');
    const [ type, setType ] = useState<InstrumentType>(originalType ?? 'stock');

    const isLoaded = checkIsLoaded(currencies);
    const isError = checkIsError(currencies);

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={(e) => { onSubmit(name, symbol, exchange, type, currencyCode, note); e.preventDefault() }}>
                <TextInput label='Name' placeholder='e.g. Apple Inc.' defaultValue={originalName} onChange={(val) => setName(val)} />
                <TextInput label='Symbol' placeholder='e.g. AAPL' defaultValue={originalSymbol} onChange={(val) => setSymbol(val)} />
                <TextInput label='Exchange' placeholder='e.g. NASDAQ' defaultValue={originalExchange} onChange={(val) => setExchange(val)} />
                <InstrumentTypeDropdown defaultType={originalType} onChange={(t) => setType(t)} />
                <CurrencyDropdown currencies={currencies.data ?? []} defaultCurrency={originalCurrencyCode} onChange={(code) => setCurrencyCode(code)} />
                <TextInput label='Note' defaultValue={originalNote} onChange={(val) => setNote(val)} />
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
    )
}