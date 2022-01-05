import React, { useEffect, useState } from 'react';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';

type Props = {
    name?: string;
    currencyCode?: string;
    note?: string;
    onSubmit: (name: string, currencyCode: string, note: string) => void;
}

export default function PortfolioForm({ name: defaultName, currencyCode: defaultCurrencyCode, note: defaultNote, onSubmit }: Props): JSX.Element {
    const currencies = useGetAllKnownCurrenciesQuery();

    const [name, setName] = useState(defaultName ?? '');
    const [currencyCode, setCurrencyCode ] = useState(defaultCurrencyCode ?? '');
    const [note, setNote ] = useState(defaultNote ?? '');

    useEffect(() => {
        setCurrencyCode(currencies.data?.find(c => c.isDefault)?.code ?? 'USD');
    }, [currencies.isLoading]);

    const isLoaded = checkIsLoaded(currencies);
    const isError = checkIsError(currencies);

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={(e) => { onSubmit(name, currencyCode, note); e.preventDefault() }}>
                <div className="form-group">
                    <label htmlFor="portfolio-name">Name:</label>
                    <input
                        type="text"
                        id="portfolio-name"
                        placeholder="e.g. US stocks"
                        className="form-control"
                        value={name}
                        onChange={(e) => setName(e.target.value)} />
                </div>
                <div className="form-group">
                    <label htmlFor="portfolio-currency">Currency:</label>
                    <select id="portfolio-currency" className="form-control" onChange={(e) => setCurrencyCode(e.target.value)}>
                        {currencies.data?.map(currency => <option selected={currency.isDefault}>{currency.code}</option>)}
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="portfolio-note">Note:</label>
                    <input
                        type="text"
                        id="portfolio-note"
                        placeholder="e.g. NASDAQ stocks"
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