import React, { useState } from 'react';

import { useDeleteTransactionMutation } from '../../redux/api/transactionApi';
import { Currency, Transaction } from '../../types';
import ModalWrapper from '../modals/ModalWrapper';
import { getDateTimeLocaleString, getPriceString } from '../../utils/string';
import EditTransactionForm from '../forms/EditTransactionForm';

type Props = {
    transaction: Transaction;
    currency?: Currency;
}

export default function TransactionRow({ transaction, currency }: Props): JSX.Element {
    const [ deleteTransaction ] = useDeleteTransactionMutation();

    const [modalIsOpen, setModalIsOpen] = useState(false);

    return (
        <tr>
            <td>{getDateTimeLocaleString(transaction.time)}</td>
            <td>{transaction.amount}</td>
            <td>{getPriceString(transaction.price, currency?.symbol)}</td>
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
            <ModalWrapper closeModal={() => setModalIsOpen(false)} isOpen={modalIsOpen}>
                <EditTransactionForm onSuccess={() => setModalIsOpen(false)} transaction={transaction} />
            </ModalWrapper>
        </tr>
    )
}