import React, { useEffect, useState } from 'react';

import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../utils/queries';
import { useCreatePortfolioMutation } from '../../redux/api/portfolioApi';

import CurrencyDropdown from './fields/CurrencyDropdown';
import TextInput from './fields/TextInput';

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
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <form onSubmit={handleSubmit}>
                <TextInput label='Name' onChange={(val) => setName(val)} placeholder='e.g. US stocks'
                    value={name}
                />
                <CurrencyDropdown
                    currencies={currencies.data!}
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