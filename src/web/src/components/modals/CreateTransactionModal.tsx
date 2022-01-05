import React from 'react';
import { DateTime } from 'luxon';
import { useAddTransactionMutation } from '../../redux/api/transactionApi';
import { ModalCallbacks } from '../../types';
import TransactionForm from '../forms/TransactionForm';
import { onSuccessfulResponse } from '../utils/modal';

type Props = {
    portfolioId?: number;
    positionId?: number;
} & ModalCallbacks

export default function CreateTransactionModal({ portfolioId, positionId, closeModal }: Props): JSX.Element {
    const [createTransaction] = useAddTransactionMutation();

    const handleSubmit = (portfolioId: number, positionId: number, amount: number, price: number, time: DateTime, note: string) => {
        const transaction = {
            portfolioId,
            positionId,
            price,
            amount,
            time: time.toISO(),
            note
        };

        createTransaction(transaction).then((val) => {
            onSuccessfulResponse(val, closeModal);
        });
    }

    return (
        <TransactionForm portfolioId={portfolioId} positionId={positionId} onSubmit={handleSubmit} />
    )
}