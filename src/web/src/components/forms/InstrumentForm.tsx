import React, { useEffect, useState } from 'react';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { InstrumentType } from '../../types';
import * as constants from '../../constants';

type Props = {
    onSubmit: (name: string, symbol: string, exchange: string, type: InstrumentType, currencyCode: string) => void;
}

export default function InstrumentForm({ onSubmit }: Props): JSX.Element {
    const currencies = useGetAllKnownCurrenciesQuery();
    const types: Array<InstrumentType> = ['stock', 'bond', 'mutualFund', 'commodity', 'cryptoCurrency', 'etf', 'index', 'other']

    const [ name, setName] = useState('');
    const [ symbol, setSymbol ] = useState('');
    const [ exchange, setExchange ] = useState('');
    const [ currencyCode, setCurrencyCode ] = useState('');
    const [ type, setType ] = useState<InstrumentType>('stock');

    useEffect(() => {
        setCurrencyCode(currencies.data?.find(c => c.isDefault)?.code ?? 'USD');
    }, [currencies.isLoading]);

    const isLoaded = checkIsLoaded(currencies);
    const isError = checkIsError(currencies);

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={(e) => { onSubmit(name, symbol, exchange, type, currencyCode); e.preventDefault() }}>
                <div className="form-group">
                    <label htmlFor="instrument-name">Name:</label>
                    <input type="text" id="instrument-name" placeholder="e.g. Apple Inc." className="form-control" onChange={(e) => setName(e.target.value)}></input>
                </div>
                <div className="form-group">
                    <label htmlFor="instrument-symbol">Symbol:</label>
                    <input type="text" id="instrument-symbol" placeholder="e.g. AAPL" className="form-control" onChange={(e) => setSymbol(e.target.value)}></input>
                </div>
                <div className="form-group">
                    <label htmlFor="instrument-exchange">Exchange:</label>
                    <input type="text" id="instrument-exchange" placeholder="e.g. NASDAQ" className="form-control" onChange={(e) => setExchange(e.target.value)}></input>
                </div>
                <div className="form-group">
                    <label htmlFor="instrument-type">Type:</label>
                    <select id="instrument-type" className="form-control" onChange={(e) => setType(e.target.value as InstrumentType)}>
                        {types.map(type => <option value={type}>{constants.INSTRUMENT_TYPE_TO_STRING[type]}</option>)}
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="instrument-currency">Currency:</label>
                    <select id="instrument-currency" className="form-control" onChange={(e) => setCurrencyCode(e.target.value)}>
                        {currencies.data?.map(currency => <option selected={currency.isDefault}>{currency.code}</option>)}
                    </select>
                </div>
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
    )
}