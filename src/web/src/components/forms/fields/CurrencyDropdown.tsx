import React, { useEffect, useState } from 'react';

import Form from 'react-bootstrap/Form';

import { Currency } from '../../../types';

type Props = {
    /**
     * Currencies to display in the dropdown.
     */
    currencies: Array<Currency>;

    /**
     * Class name to append to the form field.
     */
    className?: string;

    /**
     * Determines whether the dropdown is disabled.
     */
    disabled?: boolean;

    /**
     * Custom default label to use for the dropdown.
     * 
     * Default value is 'Currency'.
     */
    label?: string;

    /**
     * Binding property for the dropdown's current value.
     */
    value?: string;

    /**
     * A callback which is invoked every time the dropdown selection changes.
     */
    onChange?: (currencyCode: string) => void
}

/**
 * Renders a currency dropdown form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function CurrencyDropdown({ currencies, className, label, disabled, value, onChange }: Props): JSX.Element {
    const [currencyCode, setCurrencyCode] = useState(value);

    // adjust internal state if `value` prop changes
    useEffect(() => {
        if(value !== undefined) {
            setCurrencyCode(value);
        }
    }, [value]);

    // simulate dropdown value change to first available value if source currencies change
    useEffect(() => {
        if(!currencyCode && currencies.length > 0) {
            handleCurrencyChange(currencies[0].code);
        }
    })

    const handleCurrencyChange = (code: string) => {
        setCurrencyCode(code);
        onChange && onChange(code);
    }

    return (
        <Form.Group className={className ?? ''} controlId="form-currency">
            <Form.Label>{label ?? 'Currency'}:</Form.Label>
            <Form.Select disabled={disabled}
                aria-label='Currency' onChange={(e) => handleCurrencyChange(e.target.value)} value={currencyCode}
            >
                {currencies.map(currency => <option key={currency.code} value={currency.code}>{currency.code}</option>)}
            </Form.Select>
        </Form.Group>
    )
}

export default CurrencyDropdown;