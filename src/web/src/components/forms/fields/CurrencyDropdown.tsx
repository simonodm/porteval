import React, { useEffect, useState } from 'react';
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
function CurrencyDropdown({ currencies, className, disabled, value, onChange }: Props): JSX.Element {
    const [currencyCode, setCurrencyCode] = useState(value);

    useEffect(() => {
        if(value !== undefined) {
            handleCurrencyChange(value);
        }
    }, [value]);

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
        <div className={`form-group ${className ?? ''}`}>
            <label htmlFor="currency">Currency:</label>
            <select className="form-control" disabled={disabled} id="currency"
                onChange={(e) => handleCurrencyChange(e.target.value)} value={currencyCode}
            >
                {currencies.map(currency => <option key={currency.code} value={currency.code}>{currency.code}</option>)}
            </select>
        </div>
    )
}

export default CurrencyDropdown;