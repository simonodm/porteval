import { DateTime } from 'luxon';
import React, { useState } from 'react';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../utils/queries';
import { useUpdateTransactionMutation } from '../../redux/api/transactionApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import DateTimeSelector from './fields/DateTimeSelector';
import NumberInput from './fields/NumberInput';
import TextInput from './fields/TextInput';
import { Transaction } from '../../types';

type Props = {
    transaction: Transaction;
    onSuccess: () => void;
}

export default function EditTransactionForm({ transaction, onSuccess }: Props): JSX.Element {
    const [note, setNote] = useState(transaction.note);
    const [updateTransaction, mutationStatus] = useUpdateTransactionMutation();

    const isLoaded = checkIsLoaded(mutationStatus);
    const isError= checkIsError(mutationStatus);

    const handleNoteChange = (newNote: string) => {
        setNote(newNote);
    }

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        updateTransaction({
            ...transaction,
            note
        }).then(res => onSuccessfulResponse(res, onSuccess));

        e.preventDefault();
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={handleSubmit}>
                <NumberInput label='Amount' disabled value={transaction.amount} allowNegativeValues allowFloat />
                <NumberInput label='Price' disabled value={transaction.price} allowFloat />
                <DateTimeSelector label='Date' format='MMM dd, yyyy, HH:mm' timeInterval={1} disabled value={DateTime.fromISO(transaction.time)} />
                <TextInput label='Note' value={note} onChange={handleNoteChange} />
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
    )
}