import React, { useEffect, useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import useInstrumentPriceAutoFetchingState from '../../hooks/useInstrumentPriceAutoFetchingState';
import useUserSettings from '../../hooks/useUserSettings';
import InstrumentDropdown from './fields/InstrumentDropdown';
import TextInput from './fields/TextInput';
import InstrumentTypeDropdown from './fields/InstrumentTypeDropdown';
import NumberInput from './fields/NumberInput';
import DateTimeSelector from './fields/DateTimeSelector';
import CurrencyDropdown from './fields/CurrencyDropdown';
import ExchangeDropdown from './fields/ExchangeDropdown';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { useCreateInstrumentMutation, useGetAllInstrumentsQuery } from '../../redux/api/instrumentApi';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../../utils/queries';
import { Instrument, InstrumentType } from '../../types';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import { useAddPositionMutation } from '../../redux/api/positionApi';
import { useGetAllKnownExchangesQuery } from '../../redux/api/exchangeApi';

type Props = {
    /**
     * ID of portfolio to open position for.
     */
    portfolioId: number;

    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess: () => void;
}

/**
 * Renders a position creation form.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function OpenPositionForm({ portfolioId, onSuccess }: Props): JSX.Element {
    const instruments = useGetAllInstrumentsQuery();
    const currencies = useGetAllKnownCurrenciesQuery();
    const exchanges = useGetAllKnownExchangesQuery();
    
    const [createInstrument, instrumentMutationStatus] = useCreateInstrumentMutation();
    const [createPosition, positionMutationStatus] = useAddPositionMutation();

    const [instrumentId, setInstrumentId] = useState<number | undefined>(undefined);

    // State related to instrument creation subform
    const [isNewInstrument, setIsNewInstrument] = useState(false);
    const [instrumentSymbol, setInstrumentSymbol] = useState('');
    const [instrumentName, setInstrumentName] = useState('');
    const [instrumentExchange, setInstrumentExchange] = useState('');
    const [instrumentType, setInstrumentType] = useState<InstrumentType>('stock');
    const [instrumentCurrency, setInstrumentCurrency] = useState('');

    const [amount, setAmount] = useState<number | undefined>(undefined);
    const [time, setTime] = useState(new Date());
    const [
        price,
        setPriceFetchInstrument,
        setPriceFetchTime,
        setPrice,
        setAutoUpdateEnabled
    ] = useInstrumentPriceAutoFetchingState(instrumentId, time);

    const [positionNote, setPositionNote] = useState('');

    const [userSettings] = useUserSettings();

    const isLoaded = checkIsLoaded(instruments, currencies, exchanges,
        instrumentMutationStatus, positionMutationStatus);
    const isError = checkIsError(instruments, currencies, exchanges);

    // set instrument currency to default after currencies are loaded
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

    const handleTimeChange = (time: Date) => {
        setTime(time);
        setPriceFetchTime(time);
    }

    const handleInstrumentCreationStart = () => {
        setIsNewInstrument(true);
        setPrice(undefined);
        setAutoUpdateEnabled(false);
    }

    const handleInstrumentCreationCancel = () => {
        setIsNewInstrument(false);
        setAutoUpdateEnabled(true);
    }

    const handlePriceChange = (price: number) => {
        setPrice(price);
        setAutoUpdateEnabled(false);
    }

    const handleSubmit = (e: React.FormEvent) => {
        let positionInstrumentId: number | undefined = instrumentId;

        const handleNewInstrumentCreated = (newInstrument: Instrument) => {
            setInstrumentId(newInstrument.id);
            setIsNewInstrument(false);
            positionInstrumentId = newInstrument.id;
        }

        const handleCreatePosition = () => {
            if(portfolioId && positionInstrumentId && amount && price) {
                createPosition({
                    portfolioId,
                    instrumentId: positionInstrumentId,
                    note: positionNote,
                    amount,
                    price,
                    time: time.toISOString()
                }).then(res => onSuccessfulResponse(res, onSuccess));
            }
        }

        // If new instrument is being created, then it needs to be successfully created first
        // before we attempt to create the position.
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
            <Form onSubmit={handleSubmit} aria-label="Open position form">
                <InstrumentDropdown
                    className="mb-3" 
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
                            className="mb-3" 
                            label='Instrument name'
                            onChange={setInstrumentName}
                            placeholder='e.g. Apple Inc.'
                            value={instrumentName}
                        />
                        <TextInput
                            className="mb-3" 
                            label='Instrument symbol'
                            onChange={setInstrumentSymbol}
                            placeholder='e.g. AAPL'
                            value={instrumentSymbol}
                        />
                        <ExchangeDropdown
                            className="mb-3" 
                            exchanges={exchanges.data ?? []}
                            onChange={(e) => setInstrumentExchange(e.symbol)}
                        />
                        <CurrencyDropdown
                            className="mb-3" 
                            currencies={currencies.data ?? []}
                            onChange={setInstrumentCurrency}
                            value={instrumentCurrency}
                        />
                        <InstrumentTypeDropdown className="mb-3" onChange={setInstrumentType} value={instrumentType} />
                    </>
                }
                <NumberInput className="mb-3" allowFloat label='Amount'
                    onChange={setAmount} value={amount}
                />
                <NumberInput className="mb-3" allowFloat label='Price'
                    onChange={handlePriceChange} value={price}
                />
                <DateTimeSelector dateFormat={userSettings.dateFormat} enableTime label='Date'
                    onChange={handleTimeChange} timeFormat={userSettings.timeFormat} timeInterval={1}
                    value={time} className="mb-3" 
                />
                <TextInput className="mb-3" label='Note' onChange={setPositionNote}
                    value={positionNote}
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

export default OpenPositionForm;