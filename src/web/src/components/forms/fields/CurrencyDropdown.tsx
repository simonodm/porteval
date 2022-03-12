import React, { useEffect, useState } from 'react';

import { Currency } from '../../../types';

type Props = {
    currencies: Array<Currency>;
    className?: string;
    disabled?: boolean;
    value?: string;
    onChange?: (currencyCode: string) => void
}

export default function CurrencyDropdown({ currencies, className, disabled, value, onChange }: Props): JSX.Element {
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