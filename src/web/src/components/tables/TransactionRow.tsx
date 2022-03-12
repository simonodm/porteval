import React, { useState } from 'react';

import { useDeleteTransactionMutation } from '../../redux/api/transactionApi';
import { Currency, Transaction } from '../../types';
import ModalWrapper from '../modals/ModalWrapper';
import { formatDateTimeString, getPriceString } from '../../utils/string';
import EditTransactionForm from '../forms/EditTransactionForm';
import useUserSettings from '../../hooks/useUserSettings';

type Props = {
    transaction: Transaction;
    currency?: Currency;
}

export default function TransactionRow({ transaction, currency }: Props): JSX.Element {
    const [ deleteTransaction ] = useDeleteTransactionMutation();

    const [userSettings] = useUserSettings();

    const [modalIsOpen, setModalIsOpen] = useState(false);

    return (
        <tr>
            <td>{formatDateTimeString(transaction.time, userSettings.dateFormat + ' ' + userSettings.timeFormat)}</td>
            <td>{transaction.amount}</td>
            <td>{getPriceString(transaction.price, currency?.symbol, userSettings)}</td>
            <td>{transaction.note}</td>
            <td>
                <button
                    className="btn btn-primary btn-extra-sm mr-1"
                    onClick={() => setModalIsOpen(true)}
                    role="button"
                >
                    Edit
                </button>
                <button 
                    className="btn btn-danger btn-extra-sm"
                    onClick={() => deleteTransaction(transaction)}
                    role="button"
                >
                    Remove
                </button>
            </td>
            <ModalWrapper closeModal={() => setModalIsOpen(false)} heading="Edit transaction" isOpen={modalIsOpen}>
                <EditTransactionForm onSuccess={() => setModalIsOpen(false)} transaction={transaction} />
            </ModalWrapper>
        </tr>
    )
}