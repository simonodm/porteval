import React, { useMemo, useState } from 'react';
import DataTable, { ColumnDefinition } from './DataTable';
import useUserSettings from '../../hooks/useUserSettings';
import PositionsTable from './PositionsTable';
import EditPortfolioForm from '../forms/EditPortfolioForm';
import OpenPositionForm from '../forms/OpenPositionForm';
import ModalWrapper from '../modals/ModalWrapper';
import ExpandAllButtons from './ExpandAllButtons';

import Button from 'react-bootstrap/Button';

import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import { getPerformanceString, getPriceString } from '../../utils/string';
import {
    useDeletePortfolioMutation,
    useGetAllPortfoliosQuery,
    useGetAllPortfoliosStatisticsQuery
} from '../../redux/api/portfolioApi';
import { Link, NavLink } from 'react-router-dom';
import { generateDefaultPortfolioChart } from '../../utils/chart';
import { EntityStatistics, Portfolio } from '../../types';

/**
 * Represents merged portfolio data and its statistics.
 * @ignore
 */
type PortfolioWithStats = Portfolio & EntityStatistics;

/**
 * Loads portfolios and renders portfolios table.
 * 
 * @category Tables
 * @component
 */
function PortfoliosTable(): JSX.Element {
    const currencies = useGetAllKnownCurrenciesQuery();
    const portfolios = useGetAllPortfoliosQuery();
    const portfolioStats = useGetAllPortfoliosStatisticsQuery();
    const [deletePortfolio] = useDeletePortfolioMutation();

    const [portfolioBeingEdited, setPortfolioBeingEdited] = useState<Portfolio | undefined>(undefined);
    const [openPositionPortfolio, setOpenPositionPortfolio] = useState<Portfolio | undefined>(undefined);
    const [openPositionModalIsOpen, setOpenPositionModalIsOpen] = useState(false);
    const [editModalIsOpen, setEditModalIsOpen] = useState(false);

    // because removal of a portfolio invalidates portfolios RTK tag, the application immediately attempts to re-fetch
    // the removed portfolio's positions, which typically results in an error
    // for this reason, we cache the ID of the last removed portfolio here, and later prevent
    // render of the nested positions' table for that portfolio
    const [removedPortfolioId, setRemovedPortfolioId] = useState<number | undefined>(undefined);

    const [userSettings] = useUserSettings();

    // As portfolios' data and statistics are retrieved from 2 separate endpoints, we merge and memoize them here 
    const portfoliosWithStats = useMemo<Array<PortfolioWithStats>>(() => {
        if(portfolios.data && portfolioStats.data && portfolios.data.length === portfolioStats.data.length) {
            return portfolios.data.map((portfolio, idx) => ({
                ...portfolio,
                ...portfolioStats.data![idx]
            }));
        }

        return []
    }, [portfolios.data, portfolioStats.data]);

    const columnsCompact = useMemo<ColumnDefinition<PortfolioWithStats>[]>(() => [
        {
            id: 'name',
            header: 'Name',
            accessor: p => p.name,
            render: p => <Link to={`/portfolios/${p.id}`}>{p.name}</Link>
        },
        {
            id: 'currency',
            header: 'Currency',
            accessor: p => p.currencyCode
        },
        {
            id: 'profit',
            header: 'Profit',
            accessor: p => p.totalProfit,
            render: p => getPriceString(p.totalProfit, p.currencyCode, userSettings)
        },
        {
            id: 'performance',
            header: 'Performance',
            accessor: p => p.totalPerformance,
            render: p => getPerformanceString(p.totalPerformance, userSettings)
        },
        {
            id: 'actions',
            header: 'Actions',
            render: (data) => 
                <>
                    <Button
                        variant="primary"
                        className="btn-xs"
                        onClick={() => {
                            setOpenPositionPortfolio(data);
                            setOpenPositionModalIsOpen(true);
                        }}
                    >Open position
                    </Button>
                    <Button
                        variant="primary"
                        className="btn-xs"
                        onClick={() => {
                            setPortfolioBeingEdited(data);
                            setEditModalIsOpen(true);
                        }}
                    >Edit
                    </Button>
                    <Button
                        variant="danger"
                        className="btn-xs"
                        onClick={() => {
                            setRemovedPortfolioId(data.id);
                            deletePortfolio(data.id);
                        }}
                    >Remove
                    </Button>
                </>
        }
    ], []);

    const columnsFull = useMemo<ColumnDefinition<PortfolioWithStats>[]>(() => [
        {
            id: 'name',
            header: 'Name',
            accessor: p => p.name,
            render: p => <Link to={`/portfolios/${p.id}`}>{p.name}</Link>
        },
        {
            id: 'currency',
            header: 'Currency',
            accessor: p => p.currencyCode
        },
        {
            id: 'profit',
            header: 'Profit',
            columns: [
                {
                    id: 'profitDaily',
                    header: 'Daily',
                    accessor: p => p.lastDayProfit,
                    render: p => getPriceString(p.lastDayProfit, p.currencyCode, userSettings)
                },
                {
                    id: 'profitWeekly',
                    header: 'Weekly',
                    accessor: p => p.lastWeekProfit,
                    render: p => getPriceString(p.lastWeekProfit, p.currencyCode, userSettings)
                },
                {
                    id: 'profitMonthly',
                    header: 'Monthly',
                    accessor: p => p.lastMonthProfit,
                    render: p => getPriceString(p.lastMonthProfit, p.currencyCode, userSettings)
                },
                {
                    id: 'profitTotal',
                    header: 'Total',
                    accessor: p => p.totalProfit,
                    render: p => getPriceString(p.totalProfit, p.currencyCode, userSettings)
                }
            ]
        },
        {
            id: 'performance',
            header: 'Performance',
            columns: [
                {
                    id: 'performanceDaily',
                    header: 'Daily',
                    accessor: p => p.lastDayPerformance,
                    render: p => getPerformanceString(p.lastDayPerformance, userSettings)
                },
                {
                    id: 'performanceWeekly',
                    header: 'Weekly',
                    accessor: p => p.lastWeekPerformance,
                    render: p => getPerformanceString(p.lastWeekPerformance, userSettings)
                },
                {
                    id: 'performanceMonthly',
                    header: 'Monthly',
                    accessor: p => p.lastMonthPerformance,
                    render: p => getPerformanceString(p.lastMonthPerformance, userSettings)
                },
                {
                    id: 'performanceTotal',
                    header: 'Total',
                    accessor: p => p.totalPerformance,
                    render: p => getPerformanceString(p.totalPerformance, userSettings)
                }
            ]
        },
        {
            id: 'note',
            header: 'Note',
            accessor: p => p.note
        },
        {
            id: 'actions',
            header: 'Actions',
            render: (data) => 
                <>
                    <Button
                        variant="primary"
                        className="btn-xs"
                        onClick={() => {
                            setOpenPositionPortfolio(data);
                            setOpenPositionModalIsOpen(true);
                        }}
                    >Open position
                    </Button>
                    <Button
                        variant="primary"
                        className="btn-xs"
                        onClick={() => {
                            setPortfolioBeingEdited(data);
                            setEditModalIsOpen(true);
                        }}
                        role="button"
                    >Edit
                    </Button>
                    <NavLink
                        className="btn btn-primary btn-xs mr-1"
                        state={{chart: generateDefaultPortfolioChart(data)}}
                        to="/charts/view"
                        role="button"
                    >Chart
                    </NavLink>
                    <Button
                        variant="danger"
                        className="btn-xs"
                        onClick={() => {
                            setRemovedPortfolioId(data.id);
                            deletePortfolio(data.id);
                        }}
                        role="button"
                    >Remove
                    </Button>
                </>
        }
    ], []);

    const isLoaded = checkIsLoaded(portfolios, portfolioStats, currencies);
    const isError = checkIsError(portfolios, portfolioStats, currencies);

    return (
        <>
            <ExpandAllButtons />
            <DataTable className="w-100 entity-list" sortable expandable
                columnDefinitions={{
                    lg: columnsFull,
                    xs: columnsCompact
                }}
                data={{
                    data: portfoliosWithStats,
                    isLoading: !isLoaded,
                    isError
                }}
                ariaLabel="Portfolios table"
                idSelector={p => p.id}
                expandElement={portfolio =>
                    removedPortfolioId !== portfolio.id
                        ? 
                            <PositionsTable
                                className="w-100 entity-list entity-list-nested"
                                portfolioId={portfolio.id}
                            />
                        : null                 
                }
            />
            <ModalWrapper closeModal={() => setOpenPositionModalIsOpen(false)} heading="Open new position"
                isOpen={openPositionModalIsOpen}
            >
                {
                    openPositionPortfolio !== undefined
                        ? 
                            <OpenPositionForm
                                onSuccess={() => setOpenPositionModalIsOpen(false)}
                                portfolioId={openPositionPortfolio.id}
                            />
                        : null
                }
            </ModalWrapper>
            <ModalWrapper closeModal={() => setEditModalIsOpen(false)} heading={`Edit ${portfolioBeingEdited?.name}`}
                isOpen={editModalIsOpen}
            >
                {
                    portfolioBeingEdited !== undefined
                        ? 
                            <EditPortfolioForm
                                onSuccess={() => setEditModalIsOpen(false)}
                                portfolio={portfolioBeingEdited}
                            />
                        : null
                }
            </ModalWrapper>
        </>
    )
}

export default PortfoliosTable;