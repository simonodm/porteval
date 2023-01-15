import React, { Fragment, useState } from 'react';
import PositionsTable from '../tables/PositionsTable';
import LoadingWrapper from '../ui/LoadingWrapper';
import PageHeading from '../ui/PageHeading';
import useUserSettings from '../../hooks/useUserSettings';
import ModalWrapper from '../modals/ModalWrapper';
import OpenPositionForm from '../forms/OpenPositionForm';
import ExpandAllButtons from '../tables/ExpandAllButtons';
import ChartPreview from '../charts/ChartPreview';

import { useParams } from 'react-router-dom';
import { generateDefaultPortfolioChart } from '../../utils/chart';
import { getPerformanceString, getPriceString } from '../../utils/string';
import { useGetPortfolioByIdQuery, useGetPortfolioCurrentValueQuery, useGetPortfolioStatisticsQuery } from '../../redux/api/portfolioApi';
import { checkIsLoaded, checkIsError } from '../../utils/queries';


type Params = {
    /**
     * ID of portfolio to display.
     */
    portfolioId?: string;
}

/**
 * Renders a portfolio view based on query parameters.
 * 
 * @category Views
 * @component
 */
function PortfolioView(): JSX.Element {
    const params = useParams<Params>();
    const portfolioId = params.portfolioId ? parseInt(params.portfolioId) : 0;
    const portfolio = useGetPortfolioByIdQuery(portfolioId);

    const [modalIsOpen, setModalIsOpen] = useState(false);
    
    const value = useGetPortfolioCurrentValueQuery(portfolioId);
    const stats = useGetPortfolioStatisticsQuery(portfolioId);

    const [userSettings] = useUserSettings();

    const isLoaded = checkIsLoaded(portfolio, stats, value);
    const isError = checkIsError(portfolio, stats, value);

    const chart = portfolio.data ? generateDefaultPortfolioChart(portfolio.data) : undefined;

    return (
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <PageHeading heading={portfolio.data?.name ?? 'Portfolio'} />
            <div className="row mb-5">
                <div className="col-xs-12 col-sm-6">
                    <h5>Data</h5>
                    <table className="entity-data w-100">
                        <tbody>
                            <tr>
                                <td>Name:</td>
                                <td>{portfolio.data?.name}</td>
                            </tr>
                            <tr>
                                <td>Current value:</td>
                                <td>
                                    {
                                            getPriceString(
                                                value.data?.value,
                                                portfolio.data?.currencyCode,
                                                userSettings)
                                        }
                                </td>
                            </tr>
                            <tr>
                                <td>Total profit:</td>
                                <td>
                                    {
                                            getPriceString(
                                                stats.data?.totalProfit,
                                                portfolio.data?.currencyCode,
                                                userSettings
                                            )
                                        }
                                </td>
                            </tr>
                            <tr>
                                <td>Total performance:</td>
                                <td>
                                    {
                                            getPerformanceString(stats.data?.totalPerformance, userSettings)
                                        }
                                </td>
                            </tr>
                            <tr>
                                <td>Daily/weekly/monthly profit:</td>
                                <td>
                                    {
                                            getPriceString(
                                                stats.data?.lastDayProfit,
                                                portfolio.data?.currencyCode,
                                                userSettings
                                            ) + ' / '
                                        }
                                    {
                                            getPriceString(
                                                stats.data?.lastWeekProfit,
                                                portfolio.data?.currencyCode,
                                                userSettings
                                            ) + ' / '
                                        }
                                    {
                                            getPriceString(
                                                stats.data?.lastMonthProfit,
                                                portfolio.data?.currencyCode,
                                                userSettings
                                            )
                                        }
                                </td>
                            </tr>
                            <tr>
                                <td>Daily/weekly/monthly performance:</td>
                                <td>
                                    {
                                            getPerformanceString(
                                                stats.data?.lastDayPerformance,
                                                userSettings
                                            ) + ' / '
                                        }
                                    {
                                            getPerformanceString(
                                                stats.data?.lastWeekPerformance,
                                                userSettings
                                            ) + ' / '
                                        }
                                    {
                                            getPerformanceString(
                                                stats.data?.lastMonthPerformance,
                                                userSettings
                                            )
                                        }
                                </td>
                            </tr>
                            <tr>
                                <td>Note:</td>
                                <td>{portfolio.data?.note}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div className="col-xs-12 col-sm-6">
                    { chart && <ChartPreview chart={chart} /> }
                </div>
            </div>
            <div className="action-buttons">
                <button
                    className="btn btn-success btn-sm float-right"
                    onClick={() => setModalIsOpen(true)} role="button"
                >
                    Open position
                </button>
            </div>
            <div className="row">
                <div className="col-xs-12 container-fluid">
                    <h5>Positions</h5>
                    <ExpandAllButtons />
                    <PositionsTable className="w-100 entity-list" portfolioId={portfolioId} />
                </div>
            </div>
            <ModalWrapper closeModal={() => setModalIsOpen(false)} heading="Open position" isOpen={modalIsOpen}>
                <OpenPositionForm onSuccess={() => setModalIsOpen(false)} portfolioId={portfolioId} />
            </ModalWrapper>
        </LoadingWrapper>
    )
}

export default PortfolioView;