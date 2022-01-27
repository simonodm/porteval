import React from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import TransactionRow from './TransactionRow';
import { useGetPositionTransactionsQuery } from '../../redux/api/transactionApi';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { Currency } from '../../types';

type Props = {
    positionId: number;
    currency?: Currency;
}

export default function TransactionsTable({ positionId, currency }: Props): JSX.Element {
    const transactions = useGetPositionTransactionsQuery({ positionId });
    const isLoaded = checkIsLoaded(transactions);
    const isError = checkIsError(transactions);

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <table className="w-50 entity-list-nested ml-auto mr-auto">
                <thead>
                    <tr>
                        <th>Transaction time</th>
                        <th>Transaction amount</th>
                        <th>Transaction price</th>
                        <th>Transaction note</th>
                    </tr>
                </thead>
                <tbody>
                    {transactions.data?.map(transaction => <TransactionRow key={`transaction_${transaction.id}`} transaction={transaction} currency={currency}/>)}
                </tbody>
            </table>
        </LoadingWrapper>
    )
}