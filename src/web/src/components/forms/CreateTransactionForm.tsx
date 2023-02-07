import React, { useEffect, useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import useInstrumentPriceAutoFetchingState from '../../hooks/useInstrumentPriceAutoFetchingState';
import useUserSettings from '../../hooks/useUserSettings';
import NumberInput from './fields/NumberInput';
import DateTimeSelector from './fields/DateTimeSelector';
import TextInput from './fields/TextInput';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { useGetPositionQuery } from '../../redux/api/positionApi';
import { useAddTransactionMutation } from '../../redux/api/transactionApi';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../../utils/queries';

type Props = {
    /**
     * ID of position to create transaction for.
     */
    positionId: number;

    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess: () => void;
}

/**
 * Renders a transaction creation form.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function CreateTransactionForm({ positionId, onSuccess }: Props): JSX.Element {
    const [amount, setAmount] = useState<number | undefined>(undefined);
    const [time, setTime] = useState(new Date());
    const [note, setNote] = useState('');

    const [userSettings] = useUserSettings();

    const [
        price,
        setPriceFetchInstrument,
        setPriceFetchTime,
        setPrice,
        setAutoUpdateEnabled
    ] = useInstrumentPriceAutoFetchingState(undefined, time);

    const [createTransaction, mutationStatus] = useAddTransactionMutation();
    const position = useGetPositionQuery({ positionId });

    const isLoaded = checkIsLoaded(position, mutationStatus);
    const isError = checkIsError(position);

    // set price auto-fetcher instrument ID to position's instrument ID after position is loaded
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
        setAutoUpdateEnabled(false);
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
            <Form onSubmit={handleSubmit} aria-label="Create transaction form">
                <NumberInput className="mb-3" allowFloat allowNegativeValues
                    label='Amount' onChange={handleAmountChange} value={amount}
                />
                <NumberInput className="mb-3" allowFloat label='Price'
                    onChange={handlePriceChange} value={price}
                />
                <DateTimeSelector dateFormat={userSettings.dateFormat} enableTime label='Date'
                    onChange={handleTimeChange} timeFormat={userSettings.timeFormat} timeInterval={1}
                    value={time} className="mb-3" 
                />
                <TextInput className="mb-3" label='Note' onChange={handleNoteChange}
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

export default CreateTransactionForm;