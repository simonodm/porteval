import React, { useEffect, useState } from 'react';

import { Portfolio } from '../../../types';

type Props = {
    portfolios: Array<Portfolio>;
    className?: string;
    disabled?: boolean;
    value?: number;
    onChange?: (portfolioId: number) => void;
}

export default function PortfolioDropdown({ className, portfolios, disabled, value, onChange }: Props): JSX.Element {
    const [portfolioId, setPortfolioId] = useState(value);

    const handlePortfolioChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newPortfolioId = parseInt(e.target.value);
        if(!isNaN(newPortfolioId)) {
            setPortfolioId(newPortfolioId);
            onChange && onChange(newPortfolioId);
        }
    }

    useEffect(() => {
        if(value !== undefined) {
            setPortfolioId(value);
        }
    }, [value]);

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
                onChange={handlePortfolioChange} value={portfolioId}
            >
                {portfolios.map(portfolio => <option key={portfolio.id} value={portfolio.id}>{portfolio.name}</option>)}
            </select>
        </div>
    )
}