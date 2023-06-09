import React, { useMemo, useState } from 'react';
import DataTable, { ColumnDefinition } from './DataTable';
import useUserSettings from '../../hooks/useUserSettings';
import ModalWrapper from '../modals/ModalWrapper';
import EditTransactionForm from '../forms/EditTransactionForm';

import Button from 'react-bootstrap/Button';

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
    const [deleteTransaction] = useDeleteTransactionMutation();
    const [modalIsOpen, setModalIsOpen] = useState(false);
    const [transactionBeingEdited, setTransactionBeingEdited] = useState<Transaction | undefined>(undefined);
    
    const [userSettings] = useUserSettings();

    const isLoaded = checkIsLoaded(transactions);
    const isError = checkIsError(transactions);

    const columnsCompact = useMemo<ColumnDefinition<Transaction>[]>(() => [
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
            id: 'actions',
            header: 'Actions',
            render: t =>
                <>
                    <Button
                        variant="primary"
                        className="btn-xs"
                        onClick={() => {
                            setTransactionBeingEdited(t);
                            setModalIsOpen(true);
                        }}
                    >
                        Edit
                    </Button>
                    <Button
                        variant="danger"
                        className="btn-xs"
                        onClick={() => deleteTransaction(t)}
                        role="button"
                    >
                        Remove
                    </Button>
                </>
        }
    ], []);

    const columnsFull = useMemo<ColumnDefinition<Transaction>[]>(() => [
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
                    <Button
                        variant="primary"
                        className="btn-xs"
                        onClick={() => {
                            setTransactionBeingEdited(t);
                            setModalIsOpen(true);
                        }}
                    >
                        Edit
                    </Button>
                    <Button
                        variant="danger"
                        className="btn-xs"
                        onClick={() => deleteTransaction(t)}
                        role="button"
                    >
                        Remove
                    </Button>
                </>
        }
    ], []);

    return (
        <>
            <DataTable className={className} sortable
                columnDefinitions={{
                    lg: columnsFull,
                    xs: columnsCompact
                }}
                data={{
                    data: transactions.data ?? [],
                    isLoading: !isLoaded,
                    isError
                }}
                ariaLabel={`Position ${positionId} transactions table`}
                idSelector={t => t.id}
            />
            <ModalWrapper closeModal={() => setModalIsOpen(false)} heading="Edit transaction" isOpen={modalIsOpen}>
                {
                    transactionBeingEdited !== undefined
                        ?
                            <EditTransactionForm
                                onSuccess={() => setModalIsOpen(false)}
                                transaction={transactionBeingEdited}
                            />
                        : null
                }
            </ModalWrapper>
        </>
    )
}

export default TransactionsTable;