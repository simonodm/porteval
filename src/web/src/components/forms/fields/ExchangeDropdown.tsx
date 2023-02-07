import React, { useState } from 'react';
import CreatableSelect from 'react-select/creatable'; 
import Form from 'react-bootstrap/Form';
import { Exchange, FormFieldProps } from '../../../types';

type Props = FormFieldProps<Exchange> & {
    /**
     * An array of stock exchanges to display in the dropdown. 
     */
    exchanges: Array<Exchange>
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
        <Form.Group className={className} controlId="form-exchange">
            <Form.Label>Exchange:</Form.Label>
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
        </Form.Group>
    )
}

export default ExchangeDropdown;