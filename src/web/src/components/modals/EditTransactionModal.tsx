import React from 'react';
import { DateTime } from 'luxon';
import { useUpdateTransactionMutation } from '../../redux/api/transactionApi';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import LoadingWrapper from '../ui/LoadingWrapper';
import { ModalCallbacks, Transaction } from '../../types';
import TransactionForm from '../forms/TransactionForm';

type Props = {
    transaction: Transaction;
} & ModalCallbacks

export default function EditTransactionModal({ transaction, closeModal }: Props): JSX.Element {
    const [updateTransaction, mutationStatus] = useUpdateTransactionMutation();

    const isLoaded = checkIsLoaded(mutationStatus);
    const isError = checkIsError(mutationStatus);

    const handleSubmit = (portfolioId: number, positionId: number, amount: number, price: number, time: DateTime, note: string) => {
        const updatedTransaction = {
            ...transaction,
            note
        };

        updateTransaction(updatedTransaction).then(() => closeModal());
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <TransactionForm
                portfolioId={transaction.portfolioId}
                positionId={transaction.positionId}
                price={transaction.price}
                amount={transaction.amount}
                time={DateTime.fromISO(transaction.time)}
                note={transaction.note}
                onSubmit={handleSubmit} />
        </LoadingWrapper>
    )
}