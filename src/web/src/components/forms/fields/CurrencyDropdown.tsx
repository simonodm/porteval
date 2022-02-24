import React, { useEffect, useState } from 'react';

import { Currency } from '../../../types';

type Props = {
    currencies: Array<Currency>;
    disabled?: boolean;
    value?: string;
    onChange?: (currencyCode: string) => void
}

export default function CurrencyDropdown({ currencies, disabled, value, onChange }: Props): JSX.Element {
    const [currencyCode, setCurrencyCode] = useState(value);

    useEffect(() => {
        if(value !== undefined) {
            setCurrencyCode(value);
            onChange && onChange(value);
        }
    }, [value]);

    useEffect(() => {
        if(!currencyCode && currencies.length > 0) {
            setCurrencyCode(currencies[0].code);
            onChange && onChange(currencies[0].code);
        }
    })

    return (
        <div className="form-group">
            <label htmlFor="currency">Currency:</label>
            <select className="form-control" disabled={disabled} id="currency"
                onChange={(e) => setCurrencyCode(e.target.value)} value={currencyCode}
            >
                {currencies.map(currency => <option key={currency.code} value={currency.code}>{currency.code}</option>)}
            </select>
        </div>
    )
}