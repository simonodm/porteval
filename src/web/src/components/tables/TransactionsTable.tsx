import React from 'react';

import LoadingWrapper from '../ui/LoadingWrapper';

import { useGetPositionTransactionsQuery } from '../../redux/api/transactionApi';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { Currency } from '../../types';

import TransactionRow from './TransactionRow';

type Props = {
    positionId: number;
    currency?: Currency;
}

export default function TransactionsTable({ positionId, currency }: Props): JSX.Element {
    const transactions = useGetPositionTransactionsQuery({ positionId });
    const isLoaded = checkIsLoaded(transactions);
    const isError = checkIsError(transactions);

    return (
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
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
                    {transactions.data?.map(
                        transaction =>
                            <TransactionRow
                                currency={currency}
                                key={`transaction_${transaction.id}`}
                                transaction={transaction}
                            />
                    )}
                </tbody>
            </table>
        </LoadingWrapper>
    )
}