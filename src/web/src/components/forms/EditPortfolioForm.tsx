import React, { useState } from 'react';

import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../utils/queries';
import { useUpdatePortfolioMutation } from '../../redux/api/portfolioApi';
import { Portfolio } from '../../types';

import CurrencyDropdown from './fields/CurrencyDropdown';
import TextInput from './fields/TextInput';

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
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <form onSubmit={handleSubmit}>
                <TextInput label='Name' onChange={(val) => setName(val)} value={name} />
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