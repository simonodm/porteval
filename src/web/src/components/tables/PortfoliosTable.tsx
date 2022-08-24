import React, { useMemo, useState } from 'react';

import { checkIsLoaded, checkIsError } from '../../utils/queries';

import { useDeletePortfolioMutation, useGetAllPortfoliosQuery, useGetAllPortfoliosStatisticsQuery } from '../../redux/api/portfolioApi';

import { EntityStatistics, Portfolio } from '../../types';
import DataTable, { ColumnDefinition } from './DataTable';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import { getPerformanceString, getPriceString } from '../../utils/string';
import useUserSettings from '../../hooks/useUserSettings';
import useCurrencyCodeMap from '../../hooks/useCurrencyCodeMap';
import PositionsTable from './PositionsTable';
import DataTableExpandableComponent from './DataTableExpandableComponent';
import { Link, NavLink } from 'react-router-dom';
import { generateDefaultPortfolioChart } from '../../utils/chart';
import EditPortfolioForm from '../forms/EditPortfolioForm';
import OpenPositionForm from '../forms/OpenPositionForm';
import ModalWrapper from '../modals/ModalWrapper';
import ExpandAllButtons from './ExpandAllButtons';

type PortfolioWithStats = Portfolio & EntityStatistics;

export default function PortfoliosTable(): JSX.Element {
    const currencies = useGetAllKnownCurrenciesQuery();
    const portfolios = useGetAllPortfoliosQuery();
    const portfolioStats = useGetAllPortfoliosStatisticsQuery();
    const [deletePortfolio] = useDeletePortfolioMutation();

    const [portfolioBeingEdited, setPortfolioBeingEdited] = useState<Portfolio | undefined>(undefined);
    const [openPositionPortfolio, setOpenPositionPortfolio] = useState<Portfolio | undefined>(undefined);
    const [openPositionModalIsOpen, setOpenPositionModalIsOpen] = useState(false);
    const [editModalIsOpen, setEditModalIsOpen] = useState(false);

    const [userSettings] = useUserSettings();

    const portfoliosWithStats = useMemo<Array<PortfolioWithStats>>(() => {
        if(portfolios.data && portfolioStats.data && portfolios.data.length == portfolioStats.data.length) {
            return portfolios.data.map((portfolio, idx) => ({
                ...portfolio,
                ...portfolioStats.data![idx]
            }));
        }

        return []
    }, [portfolios.data, portfolioStats.data]);

    const columns = useMemo<ColumnDefinition<PortfolioWithStats>[]>(() => [
        {
            id: 'name',
            header: 'Name',
            accessor: p => <Link to={`/portfolios/${p.id}`}>{p.name}</Link>
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
                <button
                    className="btn btn-primary btn-extra-sm mr-1"
                    onClick={() => {
                        setOpenPositionPortfolio(data);
                        setOpenPositionModalIsOpen(true);
                    }}
                    role="button"
                >Open position
                </button>
                <button
                    className="btn btn-primary btn-extra-sm mr-1"
                    onClick={() => {
                        setPortfolioBeingEdited(data);
                        setEditModalIsOpen(true);
                    }}
                    role="button"
                >Edit
                </button>
                <NavLink
                    className="btn btn-primary btn-extra-sm mr-1"
                    to={{pathname: '/charts/view', state: {chart: generateDefaultPortfolioChart(data)}}}
                >Chart
                </NavLink>
                <button className="btn btn-danger btn-extra-sm"
                    onClick={() => {
                        deletePortfolio(data.id);
                    }}
                    role="button"
                >Remove
                </button>
            </>
        }
    ], []);

    const isLoaded = checkIsLoaded(portfolios, portfolioStats, currencies);
    const isError = checkIsError(portfolios, portfolioStats, currencies);

    return (
        <>
            <ExpandAllButtons />
            <DataTable className="w-100 entity-list" sortable expandable columns={columns}
                data={{
                    data: portfoliosWithStats,
                    isLoading: !isLoaded,
                    isError
                }} 
                idSelector={p => p.id}
                expandElement={portfolio =>
                    <PositionsTable className="w-100 entity-list entity-list-nested" portfolioId={portfolio.id} />
                }
            />
            <ModalWrapper closeModal={() => setOpenPositionModalIsOpen(false)} heading="Open new position"
                isOpen={openPositionModalIsOpen}
            >
                {
                    openPositionPortfolio !== undefined
                        ? <OpenPositionForm onSuccess={() => setOpenPositionModalIsOpen(false)} portfolioId={openPositionPortfolio.id} />
                        : null
                }
            </ModalWrapper>
            <ModalWrapper closeModal={() => setEditModalIsOpen(false)} heading={`Edit ${portfolioBeingEdited?.name}`}
                isOpen={editModalIsOpen}
            >
                {
                    portfolioBeingEdited !== undefined
                        ? <EditPortfolioForm onSuccess={() => setEditModalIsOpen(false)} portfolio={portfolioBeingEdited} />
                        : null
                }
            </ModalWrapper>
        </>
    )
    
}