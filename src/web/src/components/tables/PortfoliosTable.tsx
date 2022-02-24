import React from 'react';

import LoadingWrapper from '../ui/LoadingWrapper';

import { checkIsLoaded, checkIsError } from '../../utils/queries';

import { useGetAllPortfoliosQuery } from '../../redux/api/portfolioApi';

import PortfolioRow from './PortfolioRow';
import PortfoliosTableHeaders from './PortfoliosTableHeaders';


export default function PortfoliosTable(): JSX.Element {
    const portfolios = useGetAllPortfoliosQuery();
    const isLoaded = checkIsLoaded(portfolios);
    const isError = checkIsError(portfolios);

    return (
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <table className="entity-list w-100">
                <PortfoliosTableHeaders />
                <tbody>
                    {portfolios.data?.map(portfolio =>
                        <PortfolioRow key={`portfolio_${portfolio.id}`} portfolio={portfolio} />)}
                </tbody>
            </table>
        </LoadingWrapper>
    )
    
}