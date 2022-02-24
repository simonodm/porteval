import { DateTime } from 'luxon';
import React, { useState } from 'react';

import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../../utils/queries';
import { useUpdateTransactionMutation } from '../../redux/api/transactionApi';
import LoadingWrapper from '../ui/LoadingWrapper';

import { Transaction } from '../../types';

import DateTimeSelector from './fields/DateTimeSelector';
import NumberInput from './fields/NumberInput';
import TextInput from './fields/TextInput';

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
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <form onSubmit={handleSubmit}>
                <NumberInput allowFloat allowNegativeValues disabled
                    label='Amount' value={transaction.amount}
                />
                <NumberInput allowFloat disabled label='Price'
                    value={transaction.price}
                />
                <DateTimeSelector disabled format='MMM dd, yyyy, HH:mm' label='Date'
                    timeInterval={1} value={DateTime.fromISO(transaction.time)}
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