import React, { useState } from 'react';
import CreatableSelect from 'react-select/creatable'; 
import { Exchange } from '../../../types';

type Props = {
    /**
     * Custom class name to use for the form field.
     */
    className?: string;

    /**
     * Binding property for the dropdown's current value.
     */
    value?: Exchange,

    /**
     * Determines whether the form field is disabled.
     */
    disabled?: boolean;

    /**
     * An array of stock exchanges to display in the dropdown. 
     */
    exchanges: Array<Exchange>

    /**
     * A callback which is invoked every time the dropdown selection changes.
     */
    onChange?: (exchange: Exchange) => void;
}

type Option = {
    label: string;
    value: string;
}

/**
 * Renders a stock exchange dropdown form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function ExchangeDropdown({ className, value, disabled, exchanges, onChange }: Props): JSX.Element {
    const [exchange, setExchange] = useState(value);

    const handleExchangeChange = (newValue: Option | null) => {
        if(!newValue) {
            return;
        }
        const newExchange: Exchange = {
            symbol: newValue.value
        };

        setExchange(newExchange);
        onChange && onChange(newExchange);
    }

    const createOption = (e: Exchange): Option => {
        return {
            label: e.name ?? e.symbol,
            value: e.symbol
        };
    }

    return (
        <div className={`form-group ${className ?? ''}`}>
            <label htmlFor="exchange">Exchange:</label>
            <CreatableSelect
                aria-label='Exchange'
                id="exchange"
                isDisabled={disabled}
                isSearchable
                onChange={handleExchangeChange}
                options={exchanges.map(e => ({ label: e.name ?? e.symbol, value: e.symbol }))}
                placeholder='e.g. NASDAQ'
                value={exchange ? createOption(exchange) : undefined}
            />
        </div>
    )
}

export default ExchangeDropdown;