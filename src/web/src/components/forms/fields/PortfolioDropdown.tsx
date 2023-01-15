import React, { useEffect, useState } from 'react';
import { Portfolio } from '../../../types';

type Props = {
    /**
     * An array of portfolios to display in the dropdown.
     */
    portfolios: Array<Portfolio>;

    /**
     * Custom class name to use for the form field.
     */
    className?: string;

    /**
     * Determines whether the form field is disabled.
     */
    disabled?: boolean;

    /**
     * Binding property for the dropdown's current portfolio ID.
     */
    value?: number;

    /**
     * A callback which is invoked whenever the dropdown's selection changes.
     */
    onChange?: (portfolioId: number) => void;
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
        <div className={`form-group ${className ?? ''}`}>
            <label htmlFor="portfolio">Portfolio:</label>
            <select className="form-control" disabled={disabled} id="portfolio"
                onChange={handlePortfolioChange} value={portfolioId} aria-label="Portfolio"
            >
                {portfolios.map(portfolio => <option key={portfolio.id} value={portfolio.id}>{portfolio.name}</option>)}
            </select>
        </div>
    )
}

export default PortfolioDropdown;