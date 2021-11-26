import React, { Fragment } from 'react';
import PortfoliosTableHeaders from '../tables/PortfoliosTableHeaders';
import PositionRows from '../tables/PositionRows';
import LoadingWrapper from '../ui/LoadingWrapper';

import useGetPortfolioToDatePerformanceQueryWrapper from '../../hooks/useGetPortfolioToDatePerformanceQueryWrapper';
import useGetPortfolioToDateProfitsQueryWrapper from '../../hooks/useGetPortfolioToDateProfitsQueryWrapper';
import { useGetCurrencyQuery } from '../../redux/api/currencyApi';
import { useGetPortfolioByIdQuery, useGetPortfolioCurrentValueQuery } from '../../redux/api/portfolioApi';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useParams } from 'react-router-dom';

import { skipToken } from '@reduxjs/toolkit/dist/query';
import { getPerformanceString, getPriceString } from '../utils/string';
import * as constants from '../../constants';
import PortEvalChart from '../charts/PortEvalChart';
import { generateDefaultPortfolioChart } from '../utils/chart';
import PageHeading from '../ui/PageHeading';

type Params = {
    portfolioId?: string;
}

export default function PortfolioView(): JSX.Element {
    const params = useParams<Params>();
    const portfolioId = params.portfolioId ? parseInt(params.portfolioId) : 0;
    const portfolio = useGetPortfolioByIdQuery(portfolioId);
    
    const value = useGetPortfolioCurrentValueQuery(portfolioId, { pollingInterval: constants.REFRESH_INTERVAL })
    const currency = useGetCurrencyQuery(portfolio.data?.currencyCode ?? skipToken);
    const profitData = useGetPortfolioToDateProfitsQueryWrapper(portfolioId);
    const performanceData = useGetPortfolioToDatePerformanceQueryWrapper(portfolioId);

    const isLoaded = checkIsLoaded(portfolio, value);
    const isError = checkIsError(portfolio, value);

    const chart = portfolio.data ? generateDefaultPortfolioChart(portfolio.data) : undefined;

    return (
        <Fragment>
            <PageHeading heading={portfolio.data?.name ?? 'Portfolio'} />
            <div className="row mb-5">
                <div className="col-xs-12 col-sm-6">
                    <h5>Data</h5>
                    <LoadingWrapper isLoaded={isLoaded} isError={isError}>
                        <table className="entity-data w-100">
                            <tbody>
                                <tr>
                                    <td>Name:</td>
                                    <td>{portfolio.data?.name}</td>
                                </tr>
                                <tr>
                                    <td>Current value:</td>
                                    <td>{getPriceString(value.data?.value, currency.data?.symbol)}</td>
                                </tr>
                                <tr>
                                    <td>Total profit:</td>
                                    <td>{getPriceString(profitData.total, currency.data?.symbol)}</td>
                                </tr>
                                <tr>
                                    <td>Total performance:</td>
                                    <td>{getPerformanceString(performanceData.total)}</td>
                                </tr>
                                <tr>
                                    <td>Daily/weekly/monthly profit:</td>
                                    <td>
                                        {getPriceString(profitData.lastDay, currency.data?.symbol) + ' / '}
                                        {getPriceString(profitData.lastWeek, currency.data?.symbol) + ' / '}
                                        {getPriceString(profitData.lastMonth, currency.data?.symbol)}
                                    </td>
                                </tr>
                                <tr>
                                    <td>Daily/weekly/monthly performance:</td>
                                    <td>
                                        {getPerformanceString(performanceData.lastDay) + ' / '}
                                        {getPerformanceString(performanceData.lastWeek) + ' / '}
                                        {getPerformanceString(performanceData.lastMonth)}
                                    </td>
                                </tr>
                                <tr>
                                    <td>Note:</td>
                                    <td>{portfolio.data?.note}</td>
                                </tr>
                            </tbody>
                        </table>
                    </LoadingWrapper>
                </div>
                <div className="col-xs-12 col-sm-6">
                    { chart && <PortEvalChart chart={chart} /> }
                </div>
            </div>
            <div className="row">
                <div className="col-xs-12 container-fluid">
                    <h5>Positions</h5>
                    <table className="entity-list w-100">
                        <PortfoliosTableHeaders />
                        <tbody>
                            { portfolio.data
                                ? <PositionRows portfolioId={portfolio.data?.id} />
                                : <Fragment />
                            }
                        </tbody>
                    </table>
                </div>
            </div>
        </Fragment>
        
    )
}