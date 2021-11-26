import React, { useState } from 'react';
import { useDeleteTransactionMutation } from '../../redux/api/transactionApi';
import { Currency, Transaction } from '../../types';
import ModalWrapper from '../modals/ModalWrapper';
import { getDateTimeLocaleString, getPriceString } from '../utils/string';
import EditTransactionModal from '../modals/EditTransactionModal';

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
                <button role="button" className="btn btn-primary btn-extra-sm mr-1" onClick={() => setModalIsOpen(true)}>Edit</button>
                <button 
                    role="button"
                    className="btn btn-danger btn-extra-sm"
                    onClick={() => deleteTransaction(transaction)}>
                Remove
                </button>
            </td>
            <ModalWrapper isOpen={modalIsOpen} closeModal={() => setModalIsOpen(false)}>
                <EditTransactionModal closeModal={() => setModalIsOpen(false)} transaction={transaction} />
            </ModalWrapper>
        </tr>
    )
}