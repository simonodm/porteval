import React from 'react';
import { DateTime } from 'luxon';
import { useUpdateTransactionMutation } from '../../redux/api/transactionApi';
import { ModalCallbacks, Transaction } from '../../types';
import TransactionForm from '../forms/TransactionForm';
import { onSuccessfulResponse } from '../utils/modal';

type Props = {
    transaction: Transaction;
} & ModalCallbacks

export default function EditTransactionModal({ transaction, closeModal }: Props): JSX.Element {
    const [updateTransaction] = useUpdateTransactionMutation();

    const handleSubmit = (portfolioId: number, positionId: number, amount: number, price: number, time: DateTime, note: string) => {
        const updatedTransaction = {
            ...transaction,
            note
        };

        updateTransaction(updatedTransaction).then((val) => {
            onSuccessfulResponse(val, closeModal);
        });
    }

    return (
        <TransactionForm
            portfolioId={transaction.portfolioId}
            positionId={transaction.positionId}
            price={transaction.price}
            amount={transaction.amount}
            time={DateTime.fromISO(transaction.time)}
            note={transaction.note}
            onSubmit={handleSubmit} />
    )
}