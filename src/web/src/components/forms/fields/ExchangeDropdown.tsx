import React, { useState } from 'react';
import CreatableSelect from 'react-select/creatable'; 

import { Exchange } from '../../../types';

type Props = {
    className?: string;
    value?: Exchange,
    disabled?: boolean;
    exchanges: Array<Exchange>
    onChange?: (exchange: Exchange) => void;
}

type Option = {
    label: string;
    value: string;
}

export default function ExchangeDropdown({ className, value, disabled, exchanges, onChange }: Props): JSX.Element {
    const [exchange, setExchange] = useState(value);

    const handleExchangeChange = (newValue: Option | null) => {
        if(!newValue) {
            return;
        }
        const newExchange: Exchange = {
            name: newValue.value
        };

        setExchange(newExchange);
        onChange && onChange(newExchange);
    }

    const createOption = (e: Exchange): Option => {
        return {
            label: e.name,
            value: e.name
        };
    }

    return (
        <div className={`form-group ${className ?? ''}`}>
            <label htmlFor="exchange">Exchange:</label>
            <CreatableSelect
                id="exchange"
                isDisabled={disabled}
                isSearchable
                onChange={handleExchangeChange}
                options={exchanges.map(e => ({ label: e.name, value: e.name }))}
                placeholder='e.g. NASDAQ'
                value={exchange ? createOption(exchange) : undefined}
            />
        </div>
    )
}