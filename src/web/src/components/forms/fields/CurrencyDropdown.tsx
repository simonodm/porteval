import React, { useEffect, useState } from 'react';
import { Currency } from '../../../types';

type Props = {
    currencies: Array<Currency>;
    defaultCurrency?: string;
    onChange: (currencyCode: string) => void
}

export default function CurrencyDropdown({ currencies, defaultCurrency, onChange }: Props): JSX.Element {
    const [currencyCode, setCurrencyCode] = useState(defaultCurrency ?? (currencies.find(c => c.isDefault)?.code ?? 'USD'));

    useEffect(() => {
        onChange(currencyCode);
    }, [currencyCode]);

    return (
        <div className="form-group">
            <label htmlFor="currency">Currency:</label>
            <select id="currency" className="form-control" onChange={(e) => setCurrencyCode(e.target.value)}>
                {currencies.map(currency => <option selected={currency.isDefault}>{currency.code}</option>)}
            </select>
        </div>
    )
}