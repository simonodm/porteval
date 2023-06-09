import React, { useState, useEffect } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import TextInput from './fields/TextInput';
import InstrumentTypeDropdown from './fields/InstrumentTypeDropdown';
import CurrencyDropdown from './fields/CurrencyDropdown';
import ExchangeDropdown from './fields/ExchangeDropdown';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../../utils/queries';
import { InstrumentType } from '../../types';
import { useCreateInstrumentMutation } from '../../redux/api/instrumentApi';
import { useGetAllKnownExchangesQuery } from '../../redux/api/exchangeApi';

type Props = {
    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess?: () => void;
}

/**
 * Renders an instrument creation form.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function CreateInstrumentForm({ onSuccess }: Props): JSX.Element {
    const currencies = useGetAllKnownCurrenciesQuery();
    const exchanges = useGetAllKnownExchangesQuery();
    const [createInstrument, mutationStatus] = useCreateInstrumentMutation();

    const [name, setName] = useState('');
    const [symbol, setSymbol] = useState('');
    const [exchange, setExchange] = useState('');
    const [currencyCode, setCurrencyCode] = useState('');
    const [note, setNote] = useState('');
    const [type, setType] = useState<InstrumentType>('stock');

    const isLoaded = checkIsLoaded(currencies, exchanges, mutationStatus);
    const isError = checkIsError(currencies);

    // set currency to default when currencies are loaded
    useEffect(() => {
        if(currencies.data) {
            const defaultCurrency = currencies.data.find(c => c.isDefault);
            if(defaultCurrency !== undefined) {
                setCurrencyCode(defaultCurrency.code);
            }
        }
    }, [currencies.data]);

    const handleSubmit = (e: React.FormEvent) => {
        createInstrument({
            name,
            symbol,
            exchange,
            currencyCode,
            note,
            type
        }).then(res => onSuccessfulResponse(res, onSuccess));

        e.preventDefault();
    }

    return (
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <Form onSubmit={handleSubmit} aria-label="Create instrument form">
                <TextInput className="mb-3" label='Name' onChange={(val) => setName(val)}
                    placeholder='e.g. Apple Inc.' value={name}
                />
                <TextInput className="mb-3"  label='Symbol' onChange={(val) => setSymbol(val)}
                    placeholder='e.g. AAPL' value={symbol}
                />
                <ExchangeDropdown className="mb-3" exchanges={exchanges.data ?? []}
                    onChange={(e) => setExchange(e.symbol)}
                />
                <InstrumentTypeDropdown className="mb-3" onChange={(t) => setType(t)} value={type} />
                <CurrencyDropdown
                    className="mb-3" 
                    currencies={currencies.data ?? []}
                    onChange={(code) => setCurrencyCode(code)}
                    value={currencyCode}
                />
                <TextInput className="mb-3"  label='Note' onChange={(val) => setNote(val)}
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

export default CreateInstrumentForm;