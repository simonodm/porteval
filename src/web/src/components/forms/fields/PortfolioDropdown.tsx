import React, { useState } from 'react';
import { Portfolio } from '../../../types';

type Props = {
    portfolios: Array<Portfolio>
    defaultPortfolioId?: number;
    onChange: (portfolioId: number) => void;
}

export default function PortfolioDropdown({ portfolios, defaultPortfolioId, onChange }: Props): JSX.Element {
    const [portfolioId, setPortfolioId] = useState(defaultPortfolioId);

    const handlePortfolioChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newPortfolioId = parseInt(e.target.value);
        if(!isNaN(newPortfolioId)) {
            setPortfolioId(newPortfolioId);
            onChange(newPortfolioId);
        }
    }

    return (
        <div className="form-group">
            <label htmlFor="portfolio">Portfolio:</label>
            <select id="portfolio" className="form-control" disabled={defaultPortfolioId !== undefined} onChange={handlePortfolioChange}>
                {portfolios.map(portfolio => <option value={portfolio.id} selected={portfolio.id === portfolioId}>{portfolio.name}</option>)}
            </select>
        </div>
    )
}