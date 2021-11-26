import React from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import PortfolioRow from './PortfolioRow';
import PortfoliosTableHeaders from './PortfoliosTableHeaders';
import { Fragment } from 'react';

import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useGetAllPortfoliosQuery } from '../../redux/api/portfolioApi';

export default function PortfoliosTable(): JSX.Element {
    const portfolios = useGetAllPortfoliosQuery();
    const isLoaded = checkIsLoaded(portfolios);
    const isError = checkIsError(portfolios);

    return (
        <Fragment>
            <LoadingWrapper isLoaded={isLoaded} isError={isError}>
                <table className="entity-list w-100">
                    <PortfoliosTableHeaders />
                    <tbody>
                        {portfolios.data?.map(portfolio => <PortfolioRow key={`portfolio_${portfolio.id}`} portfolio={portfolio} />)}
                    </tbody>
                </table>
            </LoadingWrapper>
        </Fragment>
    )
    
}