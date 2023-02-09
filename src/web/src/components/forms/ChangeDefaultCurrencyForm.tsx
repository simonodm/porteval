import React, { useState } from 'react';
import CurrencyDropdown from './fields/CurrencyDropdown';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { useUpdateCurrencyMutation } from '../../redux/api/currencyApi';
import { Currency } from '../../types';

type Props = {
    /**
     * A collection of application's currencies.
     */
    currencies: Currency[];

    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess?: () => void;
}

/**
 * Renders a form allowing the user to change the application's default currency.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function ChangeDefaultCurrencyForm({ currencies, onSuccess }: Props): JSX.Element {
    const [selectedCurrency, setSelectedCurrency] = useState<Currency | undefined>(currencies.find(c => c.isDefault));
    const [updateCurrency] = useUpdateCurrencyMutation();

    const handleSubmit = () => {
        if(selectedCurrency !== undefined) {
            const updatedCurrency = {
                ...selectedCurrency,
                isDefault: true
            };

            setSelectedCurrency(updatedCurrency);
            updateCurrency(updatedCurrency).then(onSuccess);
        }
    }

    return (
        <Form onSubmit={handleSubmit} aria-label="Change default currency form">
            <CurrencyDropdown
                className="mb-3"
                currencies={currencies}
                label='Choose default currency'
                value={selectedCurrency?.code ?? 'USD'}
                onChange={(code) => setSelectedCurrency(currencies.find(c => c.code === code))}
            />
            <Button
                variant="primary"
                type="submit"
            >
                Save
            </Button>
        </Form>
    )
}

export default ChangeDefaultCurrencyForm;