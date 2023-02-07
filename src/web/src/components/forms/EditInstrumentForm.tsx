import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import TextInput from './fields/TextInput';
import InstrumentTypeDropdown from './fields/InstrumentTypeDropdown';
import CurrencyDropdown from './fields/CurrencyDropdown';
import ExchangeDropdown from './fields/ExchangeDropdown';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../../utils/queries';
import { Instrument, InstrumentType } from '../../types';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import { useUpdateInstrumentMutation } from '../../redux/api/instrumentApi';
import { useGetAllKnownExchangesQuery } from '../../redux/api/exchangeApi';

type Props = {
    /**
     * Instrument to edit.
     */
    instrument: Instrument;

    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess?: () => void;
}

/**
 * Renders an instrument edit form.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function EditInstrumentForm({ instrument, onSuccess }: Props): JSX.Element {
    const currencies = useGetAllKnownCurrenciesQuery();
    const exchanges = useGetAllKnownExchangesQuery();
    const [updateInstrument, mutationStatus] = useUpdateInstrumentMutation();

    const [name, setName] = useState(instrument.name);
    const [symbol, setSymbol] = useState(instrument.symbol);
    const [exchange, setExchange] = useState<string | undefined>(instrument.exchange);
    const [currencyCode, setCurrencyCode] = useState(instrument.currencyCode);
    const [note, setNote] = useState(instrument.note);
    const [type, setType] = useState<InstrumentType>(instrument.type);

    const isLoaded = checkIsLoaded(currencies, exchanges, mutationStatus);
    const isError = checkIsError(currencies, exchanges);

    const handleSubmit = (e: React.FormEvent) => {
        updateInstrument({
            id: instrument.id,
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
            <Form onSubmit={handleSubmit} aria-label="Edit instrument form">
                <TextInput className="mb-3" label='Name' onChange={(val) => setName(val)}
                    value={name}
                />
                <TextInput disabled label='Symbol' onChange={(val) => setSymbol(val)}
                    value={symbol} className="mb-3" 
                />
                <ExchangeDropdown
                    className="mb-3" 
                    exchanges={exchanges.data ?? []}
                    onChange={(e) => setExchange(e.symbol)}
                    value={exchange ? { symbol: exchange } : undefined} 
                />
                <InstrumentTypeDropdown className="mb-3" disabled onChange={(t) => setType(t)}
                    value={type}
                />
                <CurrencyDropdown className="mb-3"  currencies={currencies.data ?? []} disabled
                    onChange={(code) => setCurrencyCode(code)} value={currencyCode}
                />
                <TextInput className="mb-3"  label='Note' onChange={(val) => setNote(val)}
                    value={note}
                />
                <Button 
                    variant="primary"
                    type="submit"
                >Save
                </Button>
            </Form>
        </LoadingWrapper>
    )
}

export default EditInstrumentForm;