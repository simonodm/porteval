import React, { useMemo } from 'react';

import { useGetPositionTransactionsQuery } from '../../redux/api/transactionApi';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { Currency, Transaction } from '../../types';

import DataTable, { ColumnDefinition } from './DataTable';
import useUserSettings from '../../hooks/useUserSettings';
import { formatDateTimeString, getPriceString } from '../../utils/string';

type Props = {
    className?: string;
    positionId: number;
    currencyCode?: string;
}

export default function TransactionsTable({ className, positionId, currencyCode }: Props): JSX.Element {
    const transactions = useGetPositionTransactionsQuery({ positionId });
    
    const [userSettings] = useUserSettings();

    const isLoaded = checkIsLoaded(transactions);
    const isError = checkIsError(transactions);

    const columns = useMemo<ColumnDefinition<Transaction>[]>(() => [
        {
            id: 'time',
            header: 'Time',
            accessor: t => formatDateTimeString(t.time, userSettings.dateFormat + ' ' + userSettings.timeFormat)
        },
        {
            id: 'amount',
            header: 'Amount',
            accessor: t => t.amount
        },
        {
            id: 'price',
            header: 'Price',
            accessor: t => t.price,
            render: t => getPriceString(t.price, currencyCode, userSettings)

        },
        {
            id: 'note',
            header: 'Note',
            accessor: t => t.note
        }
    ], []);

    return (
        <>
            <DataTable className={className} sortable
                columns={columns}
                data={{
                    data: transactions.data ?? [],
                    isLoading: !isLoaded,
                    isError
                }}
                idSelector={t => t.id}
            />
        </>
    )
}