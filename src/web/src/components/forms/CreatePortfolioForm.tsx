import React, { useEffect, useState } from 'react';
import TextInput from './fields/TextInput';
import CurrencyDropdown from './fields/CurrencyDropdown';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../utils/queries';
import { useCreatePortfolioMutation } from '../../redux/api/portfolioApi';

type Props = {
    onSuccess?: () => void;
}

export default function CreatePortfolioForm({ onSuccess }: Props): JSX.Element {
    const [createPortfolio, mutationStatus] = useCreatePortfolioMutation();

    const [name, setName] = useState('');
    const [currencyCode, setCurrencyCode] = useState('');
    const [note, setNote] = useState('');

    const currencies = useGetAllKnownCurrenciesQuery();

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
        createPortfolio({
            name,
            currencyCode,
            note
        }).then(res => onSuccessfulResponse(res, onSuccess));

        e.preventDefault();
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={handleSubmit}>
                <TextInput label='Name' placeholder='e.g. US stocks' value={name} onChange={(val) => setName(val)} />
                <CurrencyDropdown currencies={currencies.data!} value={currencyCode} onChange={(code) => setCurrencyCode(code)} />
                <TextInput label='Note' value={note} onChange={(val) => setNote(val)} />
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
        
    )
}