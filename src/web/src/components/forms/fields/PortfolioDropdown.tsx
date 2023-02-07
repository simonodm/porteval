import React, { useEffect, useState } from 'react';
import Form from 'react-bootstrap/Form';
import { FormFieldProps, Portfolio } from '../../../types';

type Props = FormFieldProps<number> & {
    /**
     * An array of portfolios to display in the dropdown.
     */
    portfolios: Array<Portfolio>;
}

/**
 * Renders a portfolio dropdown form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function PortfolioDropdown({ className, portfolios, disabled, value, onChange }: Props): JSX.Element {
    const [portfolioId, setPortfolioId] = useState(value);

    const handlePortfolioChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newPortfolioId = parseInt(e.target.value);
        if(!isNaN(newPortfolioId)) {
            setPortfolioId(newPortfolioId);
            onChange && onChange(newPortfolioId);
        }
    }

    // adjust internal state to `value` prop if it's changed
    useEffect(() => {
        if(value !== undefined) {
            setPortfolioId(value);
        }
    }, [value]);

    // refresh dropdown if input portfolio list changes
    useEffect(() => {
        if(!portfolioId && portfolios.length > 0) {
            setPortfolioId(portfolios[0].id);
            onChange && onChange(portfolios[0].id);
        }
    }, [portfolios]);

    return (
        <Form.Group className={className} controlId="form-portfolio">
            <Form.Label>Portfolio:</Form.Label>
            <Form.Select disabled={disabled}
                onChange={handlePortfolioChange} value={portfolioId} aria-label="Portfolio"
            >
                {portfolios.map(portfolio => <option key={portfolio.id} value={portfolio.id}>{portfolio.name}</option>)}
            </Form.Select>
        </Form.Group>
    )
}

export default PortfolioDropdown;