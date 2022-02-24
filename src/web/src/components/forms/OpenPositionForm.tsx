import React, { useEffect, useState } from 'react';
import { useCreateInstrumentMutation, useGetAllInstrumentsQuery } from '../../redux/api/instrumentApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../utils/queries';
import InstrumentDropdown from './fields/InstrumentDropdown';
import useInstrumentPriceAutoFetchingState from '../../hooks/useInstrumentPriceAutoFetchingState';
import { DateTime } from 'luxon';
import { Instrument, InstrumentType } from '../../types';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import TextInput from './fields/TextInput';
import InstrumentTypeDropdown from './fields/InstrumentTypeDropdown';
import NumberInput from './fields/NumberInput';
import DateTimeSelector from './fields/DateTimeSelector';
import CurrencyDropdown from './fields/CurrencyDropdown';
import { useAddPositionMutation } from '../../redux/api/positionApi';

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
    const [price, setPriceFetchInstrument, setPriceFetchTime, setPrice] = useInstrumentPriceAutoFetchingState(instrumentId, DateTime.now());

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
        }
        else {
            handleCreatePosition();
        }        
        
        e.preventDefault();
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={handleSubmit}>
                <InstrumentDropdown instruments={instruments.data ?? []} value={instrumentId} creatable onChange={handleInstrumentChange} onCreate={handleInstrumentCreationStart} onCancelCreate={handleInstrumentCreationCancel} />
                { 
                    isNewInstrument &&
                    <>
                        <TextInput label='Instrument name' placeholder='e.g. Apple Inc.' value={instrumentName} onChange={setInstrumentName} />
                        <TextInput label='Instrument symbol' placeholder='e.g. AAPL' value={instrumentSymbol} onChange={setInstrumentSymbol} />
                        <TextInput label='Instrument exchange' placeholder='e.g. NASDAQ' value={instrumentExchange} onChange={setInstrumentExchange} />
                        <CurrencyDropdown currencies={currencies.data ?? []} value={instrumentCurrency} onChange={setInstrumentCurrency} />
                        <InstrumentTypeDropdown value={instrumentType} onChange={setInstrumentType} />
                    </>
                }
                <NumberInput label='Amount' allowFloat value={amount} onChange={setAmount} />
                <NumberInput label='Price' allowFloat value={price} onChange={setPrice} />
                <DateTimeSelector label='Date' timeInterval={1} enableTime format='MMM dd, yyyy, HH:mm' value={time} onChange={handleTimeChange} />
                <TextInput label='Note' value={positionNote} onChange={setPositionNote} />
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
    )
}