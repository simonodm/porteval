import React, { useEffect, useState } from 'react';

import { useGetPositionQuery } from '../../redux/api/positionApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../../utils/queries';

import useInstrumentPriceAutoFetchingState from '../../hooks/useInstrumentPriceAutoFetchingState';

import { useAddTransactionMutation } from '../../redux/api/transactionApi';

import useUserSettings from '../../hooks/useUserSettings';

import NumberInput from './fields/NumberInput';
import DateTimeSelector from './fields/DateTimeSelector';
import TextInput from './fields/TextInput';

type Props = {
    positionId: number;
    onSuccess: () => void;
}

export default function CreateTransactionForm({ positionId, onSuccess }: Props): JSX.Element {
    const [amount, setAmount] = useState<number | undefined>(undefined);
    const [time, setTime] = useState(new Date());
    const [note, setNote] = useState('');

    const [userSettings] = useUserSettings();

    const [
        price,
        setPriceFetchInstrument,
        setPriceFetchTime,
        setPrice
    ] = useInstrumentPriceAutoFetchingState(undefined, time);

    const [createTransaction, mutationStatus] = useAddTransactionMutation();
    const position = useGetPositionQuery({ positionId });

    const isLoaded = checkIsLoaded(position, mutationStatus);
    const isError = checkIsError(position);

    useEffect(() => {
        if(position.data) {
            setPriceFetchInstrument(position.data.instrumentId);
        }
    }, [position.data]);

    const handleAmountChange = (newAmount: number) => {
        setAmount(newAmount);
    }

    const handlePriceChange = (newPrice: number) => {
        setPrice(newPrice);
    }

    const handleTimeChange = (dt: Date) => {
        setTime(dt);
        setPriceFetchTime(dt);
    }

    const handleNoteChange = (newNote: string) => {
        setNote(newNote);
    }

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        if(positionId !== undefined && amount !== undefined && price !== undefined) {
            createTransaction({
                positionId: positionId,
                time: time.toISOString(),
                amount,
                price,
                note
            }).then(res => onSuccessfulResponse(res, onSuccess));
        }

        e.preventDefault();
    }

    return (
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <form onSubmit={handleSubmit}>
                <NumberInput allowFloat allowNegativeValues label='Amount'
                    onChange={handleAmountChange} value={amount}
                />
                <NumberInput allowFloat label='Price' onChange={handlePriceChange}
                    value={price}
                />
                <DateTimeSelector dateFormat={userSettings.dateFormat} enableTime label='Date'
                    onChange={handleTimeChange} timeFormat={userSettings.timeFormat} timeInterval={1}
                    value={time}
                />
                <TextInput label='Note' onChange={handleNoteChange} value={note} />
                <button 
                    className="btn btn-primary"
                    role="button"
                >Save
                </button>
            </form>
        </LoadingWrapper>
    )
}