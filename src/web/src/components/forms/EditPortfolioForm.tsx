import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import CurrencyDropdown from './fields/CurrencyDropdown';
import TextInput from './fields/TextInput';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../../utils/queries';
import { useUpdatePortfolioMutation } from '../../redux/api/portfolioApi';
import { Portfolio } from '../../types';

type Props = {
    /**
     * Portfolio to edit.
     */
    portfolio: Portfolio;

    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess?: () => void;
}

/**
 * Renders a portfolio edit form.
 *  
 * @category Forms
 * @subcategory Forms
 * @component 
 */
function EditPortfolioForm({ portfolio, onSuccess }: Props): JSX.Element {
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
            <Form onSubmit={handleSubmit} aria-label="Edit portfolio form">
                <TextInput className="mb-3"  label='Name' onChange={(val) => setName(val)}
                    value={name}
                />
                <CurrencyDropdown
                    className="mb-3" 
                    currencies={currencies.data!}
                    onChange={(code) => setCurrencyCode(code)}
                    value={currencyCode}
                />
                <TextInput className="mb-3" label='Note' onChange={(val) => setNote(val)}
                    value={note}
                />
                <Button 
                    variant="primary"
                    type="submit"
                >
                    Save
                </Button>
            </Form>
        </LoadingWrapper>
        
    )
}

export default EditPortfolioForm;