import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import useUserSettings from '../../hooks/useUserSettings';
import DateTimeSelector from './fields/DateTimeSelector';
import NumberInput from './fields/NumberInput';
import TextInput from './fields/TextInput';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { checkIsLoaded, onSuccessfulResponse } from '../../utils/queries';
import { useUpdateTransactionMutation } from '../../redux/api/transactionApi';
import { Transaction } from '../../types';

type Props = {
    /**
     * Transaction to edit.
     */
    transaction: Transaction;

    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess: () => void;
}

/**
 * Renders a transaction edit form.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function EditTransactionForm({ transaction, onSuccess }: Props): JSX.Element {
    const [amount, setAmount] = useState(transaction.amount);
    const [price, setPrice] = useState(transaction.price);
    const [time, setTime] = useState(new Date(transaction.time));
    const [note, setNote] = useState(transaction.note);
    const [updateTransaction, mutationStatus] = useUpdateTransactionMutation();

    const [userSettings] = useUserSettings();

    const isLoaded = checkIsLoaded(mutationStatus);

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        updateTransaction({
            ...transaction,
            amount,
            price,
            time: time.toISOString(),
            note
        }).then(res => onSuccessfulResponse(res, onSuccess));

        e.preventDefault();
    }

    return (
        <LoadingWrapper isLoaded={isLoaded}>
            <Form onSubmit={handleSubmit} aria-label="Edit transaction form">
                <NumberInput className="mb-3" allowFloat allowNegativeValues
                    label='Amount' onChange={setAmount} value={amount}
                />
                <NumberInput className="mb-3" allowFloat label='Price'
                    onChange={setPrice} value={price}
                />
                <DateTimeSelector className="mb-3" dateFormat={userSettings.dateFormat} label='Date'
                    onChange={setTime} timeFormat={userSettings.timeFormat}
                    timeInterval={1} value={time} enableTime
                />
                <TextInput className="mb-3" label='Note' onChange={setNote}
                    value={note}
                />
                <Button 
                    variant="primary"
                    type="submit"
                >Save
                </Button>
            </Form>
        </LoadingWrapper>
    )
}

export default EditTransactionForm;