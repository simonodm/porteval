import React, { useEffect, useState } from 'react';

import { DateTime } from 'luxon';

import { useCreateInstrumentMutation, useGetAllInstrumentsQuery } from '../../redux/api/instrumentApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../utils/queries';

import useInstrumentPriceAutoFetchingState from '../../hooks/useInstrumentPriceAutoFetchingState';

import { Instrument, InstrumentType } from '../../types';

import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';

import { useAddPositionMutation } from '../../redux/api/positionApi';

import InstrumentDropdown from './fields/InstrumentDropdown';

import TextInput from './fields/TextInput';
import InstrumentTypeDropdown from './fields/InstrumentTypeDropdown';
import NumberInput from './fields/NumberInput';
import DateTimeSelector from './fields/DateTimeSelector';
import CurrencyDropdown from './fields/CurrencyDropdown';


type Props = {
    portfolioId: number;
    onSuccess: () => void;
}

export default function OpenPositionForm({ portfolioId, onSuccess }: Props): JSX.Element {
    const instruments = useGetAllInstrumentsQuery();
    const currencies = useGetAllKnownCurrenciesQuery();
    
    const [createInstrument, instrumentMutationStatus] = useCreateInstrumentMutation();
    const [createPosition, positionMutationStatus] = useAddPositionMutation();

    const [instrumentId, setInstrumentId] = useState<number | undefined>(undefined);

    const [isNewInstrument, setIsNewInstrument] = useState(false);
    const [instrumentSymbol, setInstrumentSymbol] = useState('');
    const [instrumentName, setInstrumentName] = useState('');
    const [instrumentExchange, setInstrumentExchange] = useState('');
    const [instrumentType, setInstrumentType] = useState<InstrumentType>('stock');
    const [instrumentCurrency, setInstrumentCurrency] = useState('');

    const [amount, setAmount] = useState(1);
    const [time, setTime] = useState(DateTime.now());
    const [
        price,
        setPriceFetchInstrument,
        setPriceFetchTime,
        setPrice
    ] = useInstrumentPriceAutoFetchingState(instrumentId, DateTime.now());

    const [positionNote, setPositionNote] = useState('');

    const isLoaded = checkIsLoaded(instruments, currencies, instrumentMutationStatus, positionMutationStatus);
    const isError = checkIsError(instruments, currencies);

    useEffect(() => {
        if(currencies.data) {
            const defaultCurrency = currencies.data.find(c => c.isDefault);
            if(defaultCurrency !== undefined) {
                setInstrumentCurrency(defaultCurrency.code);
            }
        }
    }, [currencies.data]);

    const handleInstrumentChange = (id: number) => {
        setInstrumentId(id);
        setPriceFetchInstrument(id);
    }

    const handleTimeChange = (time: DateTime) => {
        setTime(time);
        setPriceFetchTime(time);
    }

    const handleInstrumentCreationStart = () => {
        setIsNewInstrument(true);
        setInstrumentId(undefined);
    }

    const handleInstrumentCreationCancel = () => {
        setIsNewInstrument(false);
    }

    const handleSubmit = (e: React.FormEvent) => {
        let positionInstrumentId: number | undefined = instrumentId;

        const handleNewInstrumentCreated = (newInstrument: Instrument) => {
            setInstrumentId(newInstrument.id);
            setIsNewInstrument(false);
            console.log(newInstrument);
            positionInstrumentId = newInstrument.id;
        }

        const handleCreatePosition = () => {
            if(portfolioId && positionInstrumentId) {
                createPosition({
                    portfolioId,
                    instrumentId: positionInstrumentId,
                    note: positionNote,
                    initialTransaction: {
                        amount,
                        price,
                        time: time.toISO(),
                        note: ''
                    }
                }).then(res => onSuccessfulResponse(res, onSuccess));
            }
        }

        if(isNewInstrument) {
            createInstrument({
                symbol: instrumentSymbol,
                name: instrumentName,
                exchange: instrumentExchange,
                currencyCode: instrumentCurrency,
                type: instrumentType,
                note: ''
            })
            .then(res => onSuccessfulResponse(res, handleNewInstrumentCreated))
            .then(handleCreatePosition);
        } else {
            handleCreatePosition();
        }        
        
        e.preventDefault();
    }

    return (
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <form onSubmit={handleSubmit}>
                <InstrumentDropdown
                    creatable
                    instruments={instruments.data ?? []}
                    onCancelCreate={handleInstrumentCreationCancel}
                    onChange={handleInstrumentChange}
                    onCreate={handleInstrumentCreationStart}
                    value={instrumentId}
                />
                { 
                    isNewInstrument &&
                    <>
                        <TextInput
                            label='Instrument name'
                            onChange={setInstrumentName}
                            placeholder='e.g. Apple Inc.'
                            value={instrumentName}
                        />
                        <TextInput
                            label='Instrument symbol'
                            onChange={setInstrumentSymbol}
                            placeholder='e.g. AAPL'
                            value={instrumentSymbol}
                        />
                        <TextInput
                            label='Instrument exchange'
                            onChange={setInstrumentExchange}
                            placeholder='e.g. NASDAQ'
                            value={instrumentExchange}
                        />
                        <CurrencyDropdown
                            currencies={currencies.data ?? []}
                            onChange={setInstrumentCurrency}
                            value={instrumentCurrency}
                        />
                        <InstrumentTypeDropdown onChange={setInstrumentType} value={instrumentType} />
                    </>
                }
                <NumberInput allowFloat label='Amount' onChange={setAmount}
                    value={amount}
                />
                <NumberInput allowFloat label='Price' onChange={setPrice}
                    value={price}
                />
                <DateTimeSelector enableTime format='MMM dd, yyyy, HH:mm' label='Date'
                    onChange={handleTimeChange} timeInterval={1} value={time}
                />
                <TextInput label='Note' onChange={setPositionNote} value={positionNote} />
                <button 
                    className="btn btn-primary"
                    role="button"
                >Save
                </button>
            </form>
        </LoadingWrapper>
    )
}