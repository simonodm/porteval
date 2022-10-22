import React, { useMemo, useState } from 'react';
import DataTable, { ColumnDefinition } from './DataTable';
import useUserSettings from '../../hooks/useUserSettings';
import ModalWrapper from '../modals/ModalWrapper';
import EditTransactionForm from '../forms/EditTransactionForm';

import { formatDateTimeString, getPriceString } from '../../utils/string';
import { useDeleteTransactionMutation, useGetPositionTransactionsQuery } from '../../redux/api/transactionApi';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { Transaction } from '../../types';

type Props = {
    /**
     * Custom class name to use for the table.
     */
    className?: string;

    /**
     * ID of position to display transactions for.
     */
    positionId: number;

    /**
     * Currency code of currency to display transaction prices in.
     */
    currencyCode?: string;
}

/**
 * Loads position's transactions and displays them in a table.
 * 
 * @category Tables
 * @component
 */
function TransactionsTable({ className, positionId, currencyCode }: Props): JSX.Element {
    const transactions = useGetPositionTransactionsQuery({ positionId });
    const [deleteTransaction, mutationStatus] = useDeleteTransactionMutation();
    const [modalIsOpen, setModalIsOpen] = useState(false);
    const [transactionBeingEdited, setTransactionBeingEdited] = useState<Transaction | undefined>(undefined);
    
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
        },
        {
            id: 'actions',
            header: 'Actions',
            render: t =>
                <>
                    <button
                        className="btn btn-primary btn-extra-sm mr-1"
                        onClick={() => {
                            setTransactionBeingEdited(t);
                            setModalIsOpen(true);
                        }}
                        role="button"
                    >
                        Edit
                    </button>
                    <button 
                        className="btn btn-danger btn-extra-sm"
                        onClick={() => deleteTransaction(t)}
                        role="button"
                    >
                        Remove
                    </button>
                </>
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
            <ModalWrapper closeModal={() => setModalIsOpen(false)} heading="Edit transaction" isOpen={modalIsOpen}>
                {
                    transactionBeingEdited !== undefined
                        ? <EditTransactionForm onSuccess={() => setModalIsOpen(false)} transaction={transactionBeingEdited} />
                        : null
                }
            </ModalWrapper>
        </>
    )
}

export default TransactionsTable;