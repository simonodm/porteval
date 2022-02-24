import React, { useState } from 'react';
import TextInput from './fields/TextInput';
import CurrencyDropdown from './fields/CurrencyDropdown';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../utils/queries';
import { useUpdatePortfolioMutation } from '../../redux/api/portfolioApi';
import { Portfolio } from '../../types';

type Props = {
    portfolio: Portfolio;
    onSuccess?: () => void;
}

export default function EditPortfolioForm({ portfolio, onSuccess }: Props): JSX.Element {
    const [updatePortfolio, mutationStatus] = useUpdatePortfolioMutation();

    const [name, setName] = useState(portfolio.name);
    const [currencyCode, setCurrencyCode] = useState(portfolio.currencyCode);
    const [note, setNote] = useState(portfolio.note);

    const currencies = useGetAllKnownCurrenciesQuery();

    const isLoaded = checkIsLoaded(currencies, mutationStatus);
    const isError = checkIsError(currencies);

    const handleSubmit = (e: React.FormEvent) => {
        updatePortfolio({
            id: portfolio.id,
            name,
            currencyCode,
            note
        }).then(res => onSuccessfulResponse(res, onSuccess));

        e.preventDefault();
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={handleSubmit}>
                <TextInput value={name} label='Name' onChange={(val) => setName(val)} />
                <CurrencyDropdown currencies={currencies.data!} value={currencyCode} onChange={(code) => setCurrencyCode(code)} />
                <TextInput value={note} label='Note' onChange={(val) => setNote(val)} />
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
        
    )
}