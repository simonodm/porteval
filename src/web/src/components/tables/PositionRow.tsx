import React, { useState } from 'react';


import { NavLink } from 'react-router-dom';

import useGetPositionToDatePerformanceQueryWrapper from '../../hooks/useGetPositionToDatePerformanceQueryWrapper';
import useGetPositionToDateProfitsQueryWrapper from '../../hooks/useGetPositionToDateProfitsQueryWrapper';
import { useGetCurrencyQuery } from '../../redux/api/currencyApi';
import { useDeletePositionMutation } from '../../redux/api/positionApi';
import { checkIsLoaded, checkIsError } from '../../utils/queries';

import { Position } from '../../types';
import { getPerformanceString, getPriceString } from '../../utils/string';
import ModalWrapper from '../modals/ModalWrapper';
import LoadingWrapper from '../ui/LoadingWrapper';
import { generateDefaultPositionChart } from '../../utils/chart';
import CreateTransactionForm from '../forms/CreateTransactionForm';
import EditPositionForm from '../forms/EditPositionForm';

import useUserSettings from '../../hooks/useUserSettings';

import TransactionsTable from './TransactionsTable';

type Props = {
    position: Position
}

export default function PositionRow({ position }: Props): JSX.Element {
    const currency = useGetCurrencyQuery(position.instrument.currencyCode);

    // set after deletion to prevent profit and performance refetch before the new positions are loaded
    const [isRemoved, setIsRemoved] = useState(false);

    const profitData = useGetPositionToDateProfitsQueryWrapper(position.id, isRemoved);
    const performanceData = useGetPositionToDatePerformanceQueryWrapper(position.id, isRemoved);

    const [ deletePosition ] = useDeletePositionMutation();

    const [instrumentExpanded, setInstrumentExpanded] = useState(false);
    const [createModalIsOpen, setCreateModalIsOpen] = useState(false);
    const [updateModalIsOpen, setUpdateModalIsOpen] = useState(false);

    const [userSettings] = useUserSettings();

    const isLoaded = checkIsLoaded(currency, profitData, performanceData);
    const isError = checkIsError(currency, profitData, performanceData);

    return (
        <>
            <tr className="nested">
                <td>
                    <i
                        className="bi bi-arrow-down-short"
                        onClick={() => setInstrumentExpanded(!instrumentExpanded)}
                        role="button"
                    >
                    </i>
                    <a href={`/instruments/${position.instrument.id}`}>{position.instrument.name}</a>
                </td>
                <td>
                    {position.instrument.exchange}
                </td>
                <td>
                    {position.instrument.currencyCode}
                </td>
                <td>
                    <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                        <>{getPriceString(profitData.lastDay, currency.data?.symbol, userSettings)}</>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                        <>{getPriceString(profitData.lastWeek, currency.data?.symbol, userSettings)}</>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                        <>
                            {getPriceString(profitData.lastMonth, currency.data?.symbol, userSettings)}
                        </>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                        <>{getPriceString(profitData.total, currency.data?.symbol, userSettings)}</>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                        <>{getPerformanceString(performanceData.lastDay, userSettings)}</>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                        <>{getPerformanceString(performanceData.lastWeek, userSettings)}</>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                        <>{getPerformanceString(performanceData.lastMonth, userSettings)}</>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                        <>{getPerformanceString(performanceData.total, userSettings)}</>
                    </LoadingWrapper>
                </td>
                <td></td>
                <td>{position.note}</td>
                <td>
                    <button
                        className="btn btn-primary btn-extra-sm mr-1"
                        onClick={() => setCreateModalIsOpen(true)}
                        role="button"
                    >
                        Add transaction
                    </button>
                    <button
                        className="btn btn-primary btn-extra-sm mr-1"
                        onClick={() => setUpdateModalIsOpen(true)} role="button"
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
                            deletePosition(position); setIsRemoved(true); 
                        }}
                        role="button"
                    >
                        Remove
                    </button>
                </td>
            </tr>
            { instrumentExpanded && !isRemoved &&
                <tr>
                    <td colSpan={14}>
                        <TransactionsTable currency={currency.data} positionId={position.id} />
                    </td>
                </tr>
            }
            <ModalWrapper closeModal={() => setCreateModalIsOpen(false)} heading="Add new transaction"
                isOpen={createModalIsOpen}
            >
                <CreateTransactionForm onSuccess={() => setCreateModalIsOpen(false)} positionId={position.id} />
            </ModalWrapper>
            <ModalWrapper
                closeModal={() => setUpdateModalIsOpen(false)}
                heading={`Edit position ${position.instrument.symbol}.`}
                isOpen={updateModalIsOpen}
            >
                <EditPositionForm onSuccess={() => setUpdateModalIsOpen(false)} position={position} />
            </ModalWrapper>
        </>
    )
}