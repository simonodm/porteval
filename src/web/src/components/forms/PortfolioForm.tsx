import React, { useState } from 'react';
import TextInput from './fields/TextInput';
import CurrencyDropdown from './fields/CurrencyDropdown';
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
    const [name, setName] = useState(defaultName ?? '');
    const [currencyCode, setCurrencyCode] = useState(defaultCurrencyCode ?? '');
    const [note, setNote] = useState(defaultNote ?? '');

    const currencies = useGetAllKnownCurrenciesQuery();

    const isLoaded = checkIsLoaded(currencies);
    const isError = checkIsError(currencies);

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={(e) => { onSubmit(name, currencyCode, note); e.preventDefault() }}>
                <TextInput defaultValue={defaultName} label='Name' placeholder='e.g. US stocks' onChange={(val) => setName(val)} />
                <CurrencyDropdown currencies={currencies.data!} defaultCurrency='USD' onChange={(code) => setCurrencyCode(code)} />
                <TextInput defaultValue={defaultNote} label='Note' onChange={(val) => setNote(val)} />
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
        
    )
}