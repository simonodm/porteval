import React, { useEffect, useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import CurrencyDropdown from './fields/CurrencyDropdown';
import TextInput from './fields/TextInput';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../../utils/queries';
import { useCreatePortfolioMutation } from '../../redux/api/portfolioApi';

type Props = {
    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess?: () => void;
}

/**
 * Renders a portfolio creation form.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function CreatePortfolioForm({ onSuccess }: Props): JSX.Element {
    const [createPortfolio, mutationStatus] = useCreatePortfolioMutation();

    const [name, setName] = useState('');
    const [currencyCode, setCurrencyCode] = useState('');
    const [note, setNote] = useState('');

    const currencies = useGetAllKnownCurrenciesQuery();

    const isLoaded = checkIsLoaded(currencies, mutationStatus);
    const isError = checkIsError(currencies);

    // set portfolio currency to default after currencies are loaded
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
            <Form onSubmit={handleSubmit} aria-label="Create portfolio form">
                <TextInput className="mb-3" label='Name' onChange={(val) => setName(val)}
                    placeholder='e.g. US stocks' value={name}
                />
                <CurrencyDropdown
                    className="mb-3" 
                    currencies={currencies.data!}
                    onChange={(code) => setCurrencyCode(code)}
                    value={currencyCode}
                />
                <TextInput className="mb-3"  label='Note'
                    onChange={(val) => setNote(val)} value={note}
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

export default CreatePortfolioForm;