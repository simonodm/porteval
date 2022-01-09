import React, { useEffect, useState } from 'react';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { InstrumentType } from '../../types';
import * as constants from '../../constants';

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
    const types: Array<InstrumentType> = ['stock', 'bond', 'mutualFund', 'commodity', 'cryptoCurrency', 'etf', 'index', 'other']

    const [ name, setName] = useState(originalName ?? '');
    const [ symbol, setSymbol ] = useState(originalSymbol ?? '');
    const [ exchange, setExchange ] = useState(originalExchange ?? '');
    const [ currencyCode, setCurrencyCode ] = useState(originalCurrencyCode ?? '');
    const [ note, setNote ] = useState(originalNote ?? '');
    const [ type, setType ] = useState<InstrumentType>(originalType ?? 'stock');

    useEffect(() => {
        setCurrencyCode(currencies.data?.find(c => c.isDefault)?.code ?? 'USD');
    }, [currencies.isLoading]);

    const isLoaded = checkIsLoaded(currencies);
    const isError = checkIsError(currencies);

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={(e) => { onSubmit(name, symbol, exchange, type, currencyCode, note); e.preventDefault() }}>
                <div className="form-group">
                    <label htmlFor="instrument-name">Name:</label>
                    <input
                        type="text"
                        id="instrument-name"
                        placeholder="e.g. Apple Inc."
                        className="form-control"
                        value={name}
                        onChange={(e) => setName(e.target.value)} />
                </div>
                <div className="form-group">
                    <label htmlFor="instrument-symbol">Symbol:</label>
                    <input
                        type="text"
                        id="instrument-symbol"
                        placeholder="e.g. AAPL"
                        className="form-control"
                        disabled={originalSymbol !== undefined}
                        value={symbol}
                        onChange={(e) => setSymbol(e.target.value)} />
                </div>
                <div className="form-group">
                    <label htmlFor="instrument-exchange">Exchange:</label>
                    <input
                        type="text"
                        id="instrument-exchange"
                        placeholder="e.g. NASDAQ"
                        className="form-control"
                        disabled={originalExchange !== undefined}
                        value={exchange}
                        onChange={(e) => setExchange(e.target.value)} />
                </div>
                <div className="form-group">
                    <label htmlFor="instrument-type">Type:</label>
                    <select
                        id="instrument-type"
                        className="form-control"
                        defaultValue={type}
                        disabled={originalType !== undefined}
                        onChange={(e) => setType(e.target.value as InstrumentType)}>
                            {types.map(type => <option value={type}>{constants.INSTRUMENT_TYPE_TO_STRING[type]}</option>)}
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="instrument-currency">Currency:</label>
                    <select
                        id="instrument-currency"
                        className="form-control"
                        disabled={originalCurrencyCode !== undefined}
                        onChange={(e) => setCurrencyCode(e.target.value)}>
                            {currencies.data?.map(currency => <option selected={currency.isDefault}>{currency.code}</option>)}
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="instrument-note">Note:</label>
                    <input
                        type="text"
                        id="instrument-note"
                        className="form-control"
                        value={note}
                        onChange={(e) => setNote(e.target.value)} />
                </div>
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
    )
}