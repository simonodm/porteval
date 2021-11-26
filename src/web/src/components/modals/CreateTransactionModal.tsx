import React from 'react';
import { DateTime } from 'luxon';
import { useAddTransactionMutation } from '../../redux/api/transactionApi';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import LoadingWrapper from '../ui/LoadingWrapper';
import { ModalCallbacks } from '../../types';
import TransactionForm from '../forms/TransactionForm';

type Props = {
    portfolioId?: number;
    positionId?: number;
} & ModalCallbacks

export default function CreateTransactionModal({ portfolioId, positionId, closeModal }: Props): JSX.Element {
    const [createTransaction, mutationStatus] = useAddTransactionMutation();

    const isLoaded = checkIsLoaded(mutationStatus);
    const isError = checkIsError(mutationStatus);

    const handleSubmit = (portfolioId: number, positionId: number, amount: number, price: number, time: DateTime, note: string) => {
        const transaction = {
            portfolioId,
            positionId,
            price,
            amount,
            time: time.toISO(),
            note
        };

        createTransaction(transaction).then(() => closeModal());
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <TransactionForm portfolioId={portfolioId} positionId={positionId} onSubmit={handleSubmit} />
        </LoadingWrapper>
    )
}