import React, { useMemo, useState } from 'react';

import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { useDeletePositionMutation, useGetPortfolioPositionsStatisticsQuery, useGetPositionsQuery } from '../../redux/api/positionApi';

import { Position, PositionStatistics } from '../../types';
import DataTable, { ColumnDefinition } from './DataTable';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import useCurrencyCodeMap from '../../hooks/useCurrencyCodeMap';
import useUserSettings from '../../hooks/useUserSettings';
import { getPriceString, getPerformanceString } from '../../utils/string';
import TransactionsTable from './TransactionsTable';
import DataTableExpandableComponent from './DataTableExpandableComponent';
import { Link, NavLink } from 'react-router-dom';
import { generateDefaultPositionChart } from '../../utils/chart';
import CreateTransactionForm from '../forms/CreateTransactionForm';
import EditPositionForm from '../forms/EditPositionForm';
import ModalWrapper from '../modals/ModalWrapper';

type Props = {
    className?: string;
    portfolioId: number;
}

type PositionWithStats = Position & PositionStatistics;

export default function PositionsTable({ className, portfolioId }: Props): JSX.Element {
    const positions = useGetPositionsQuery(portfolioId);
    const positionStats = useGetPortfolioPositionsStatisticsQuery(portfolioId);
    const positionsWithStats = useMemo(() => {
        if(positions.data && positionStats.data && positions.data.length == positionStats.data.length) {
            return positions.data.map((position, idx) => ({
                ...position,
                ...positionStats.data![idx]
            }));
        }

        return [];
    }, [positions, positionStats]);
    const [deletePosition] = useDeletePositionMutation();

    const [createTransactionModalIsOpen, setCreateTransactionModalIsOpen] = useState(false);
    const [updatePositionModalIsOpen, setUpdatePositionModalIsOpen] = useState(false);
    const [createTransactionPosition, setCreateTransactionPosition] = useState<Position | undefined>(undefined);
    const [positionBeingEdited, setPositionBeingEdited] = useState<Position | undefined>(undefined);

    const [userSettings] = useUserSettings();

    const isLoaded = checkIsLoaded(positions, positionStats);
    const isError = checkIsError(positions, positionStats);

    const columns = useMemo<ColumnDefinition<PositionWithStats>[]>(() => [
        {
            id: 'name',
            header: 'Name',
            accessor: p => <Link to={`/instruments/${p.instrument.id}`}>{p.instrument.name}</Link>
        },
        {
            id: 'exchange',
            header: 'Exchange',
            accessor: p => p.instrument.exchange
        },
        {
            id: 'currency',
            header: 'Currency',
            accessor: p => p.instrument.currencyCode
        },
        {
            id: 'profit',
            header: 'Profit',
            columns: [
                {
                    id: 'profitDaily',
                    header: 'Daily',
                    accessor: p => p.lastDayProfit,
                    render: p => getPriceString(p.lastDayProfit, p.instrument.currencyCode, userSettings)
                },
                {
                    id: 'profitWeekly',
                    header: 'Weekly',
                    accessor: p => p.lastWeekProfit,
                    render: p => getPriceString(p.lastWeekProfit, p.instrument.currencyCode, userSettings)
                },
                {
                    id: 'profitMonthly',
                    header: 'Monthly',
                    accessor: p => p.lastMonthProfit,
                    render: p => getPriceString(p.lastMonthProfit, p.instrument.currencyCode, userSettings)
                },
                {
                    id: 'profitTotal',
                    header: 'Total',
                    accessor: p => p.totalProfit,
                    render: p => getPriceString(p.totalProfit, p.instrument.currencyCode, userSettings)
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
            id: 'bep',
            header: 'BEP',
            accessor: p => getPriceString(p.breakEvenPoint, p.instrument.currencyCode, userSettings)
        },
        {
            id: 'note',
            header: 'Note',
            accessor: p => p.note
        },
        {
            id: 'actions',
            header: 'Actions',
            render: (position: PositionWithStats) => 
                <>
                    <button
                        className="btn btn-primary btn-extra-sm mr-1"
                        onClick={() => {
                            setCreateTransactionPosition(position);
                            setCreateTransactionModalIsOpen(true);
                        }}
                        role="button"
                    >
                        Add transaction
                    </button>
                    <button
                        className="btn btn-primary btn-extra-sm mr-1"
                        onClick={() => {
                            setPositionBeingEdited(position);
                            setUpdatePositionModalIsOpen(true);
                        }} role="button"
                    >
                        Edit
                    </button>
                    <NavLink
                        className="btn btn-primary btn-extra-sm mr-1"
                        to={{pathname: '/charts/view', state: {chart: generateDefaultPositionChart(position)}}}
                    >
                        Chart
                    </NavLink>
                    <button
                        className="btn btn-danger btn-extra-sm"
                        onClick={() => {
                            deletePosition(position);
                        }}
                        role="button"
                    >
                        Remove
                    </button>
                </>
        }
    ], []);

    return (
        <>
            <DataTable className={className} sortable expandable
                columns={columns}
                data={{
                    data: positionsWithStats,
                    isLoading: !isLoaded,
                    isError
                }}
                idSelector={p => p.id}
                expandElement={p =>
                    <TransactionsTable className="w-100 entity-list entity-list-nested" positionId={p.id} currencyCode={p.instrument.currencyCode} />
                }
            />
            <ModalWrapper closeModal={() => setCreateTransactionModalIsOpen(false)} heading="Add new transaction"
                isOpen={createTransactionModalIsOpen}
            >
                {
                    createTransactionPosition !== undefined
                        ? <CreateTransactionForm onSuccess={() => setCreateTransactionModalIsOpen(false)} positionId={createTransactionPosition.id} />
                        : null
                }                
            </ModalWrapper>
            <ModalWrapper
                closeModal={() => setUpdatePositionModalIsOpen(false)}
                heading={`Edit position ${positionBeingEdited?.instrument.symbol}.`}
                isOpen={updatePositionModalIsOpen}
            >
                {
                    positionBeingEdited !== undefined
                        ? <EditPositionForm onSuccess={() => setUpdatePositionModalIsOpen(false)} position={positionBeingEdited} />
                        : null
                }
            </ModalWrapper>
        </>
    )
}