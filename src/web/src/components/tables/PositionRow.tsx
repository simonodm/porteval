import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import TransactionsTable from './TransactionsTable';

import useGetPositionToDatePerformanceQueryWrapper from '../../hooks/useGetPositionToDatePerformanceQueryWrapper';
import useGetPositionToDateProfitsQueryWrapper from '../../hooks/useGetPositionToDateProfitsQueryWrapper';
import { useGetCurrencyQuery } from '../../redux/api/currencyApi';
import { useDeletePositionMutation } from '../../redux/api/positionApi';
import { checkIsLoaded, checkIsError } from '../utils/queries';

import { Position } from '../../types';
import { getPerformanceString, getPriceString } from '../utils/string';
import CreateTransactionModal from '../modals/CreateTransactionModal';
import ModalWrapper from '../modals/ModalWrapper';
import EditPositionModal from '../modals/EditPositionModal';
import { NavLink } from 'react-router-dom';
import { generateDefaultPositionChart } from '../utils/chart';

type Props = {
    position: Position
}

export default function PositionRow({ position }: Props): JSX.Element {
    const currency = useGetCurrencyQuery(position.instrument.currencyCode);

    const [isRemoved, setIsRemoved] = useState(false); // set after deletion to prevent profit and performance refetch before the new positions are loaded

    const profitData = useGetPositionToDateProfitsQueryWrapper(position.id, isRemoved);
    const performanceData = useGetPositionToDatePerformanceQueryWrapper(position.id, isRemoved);

    const [ deletePosition ] = useDeletePositionMutation();

    const [instrumentExpanded, setInstrumentExpanded] = useState(false);
    const [createModalIsOpen, setCreateModalIsOpen] = useState(false);
    const [updateModalIsOpen, setUpdateModalIsOpen] = useState(false);

    const isLoaded = checkIsLoaded(currency, profitData, performanceData);
    const isError = checkIsError(currency, profitData, performanceData);

    return (
        <>
            <tr className="nested">
                <td>
                    <i role="button" className="bi bi-arrow-down-short" onClick={() => setInstrumentExpanded(!instrumentExpanded)}></i>
                    <a href={`/instruments/${position.instrument.id}`}>{position.instrument.name}</a>
                </td>
                <td>
                    {position.instrument.exchange}
                </td>
                <td>
                    {position.instrument.currencyCode}
                </td>
                <td>
                    <LoadingWrapper isLoaded={isLoaded} isError={isError}>
                        <>{getPriceString(profitData.lastDay, currency.data?.symbol)}</>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isLoaded={isLoaded} isError={isError}>
                        <>{getPriceString(profitData.lastWeek, currency.data?.symbol)}</>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isLoaded={isLoaded} isError={isError}>
                        <>{getPriceString(profitData.lastMonth, currency.data?.symbol)}</>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isLoaded={isLoaded} isError={isError}>
                        <>{getPriceString(profitData.total, currency.data?.symbol)}</>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isLoaded={isLoaded} isError={isError}>
                        <>{getPerformanceString(performanceData.lastDay)}</>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isLoaded={isLoaded} isError={isError}>
                        <>{getPerformanceString(performanceData.lastWeek)}</>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isLoaded={isLoaded} isError={isError}>
                        <>{getPerformanceString(performanceData.lastMonth)}</>
                    </LoadingWrapper>
                </td>
                <td>
                    <LoadingWrapper isLoaded={isLoaded} isError={isError}>
                        <>{getPerformanceString(performanceData.total)}</>
                    </LoadingWrapper>
                </td>
                <td></td>
                <td>{position.note}</td>
                <td>
                    <button role="button" className="btn btn-primary btn-extra-sm mr-1" onClick={() => setCreateModalIsOpen(true)}>
                        Add transaction
                    </button>
                    <button role="button" className="btn btn-primary btn-extra-sm mr-1" onClick={() => setUpdateModalIsOpen(true)}>
                        Edit
                    </button>
                    <NavLink
                        className="btn btn-primary btn-extra-sm mr-1"
                        to={{pathname: '/charts/view', state: {chart: generateDefaultPositionChart(position)}}}
                        >Chart</NavLink>
                    <button
                        role="button"
                        className="btn btn-danger btn-extra-sm"
                        onClick={() => { deletePosition(position); setIsRemoved(true); }}>
                        Remove
                    </button>
                </td>
            </tr>
            <>
            { instrumentExpanded &&
                <tr>
                    <td colSpan={14}>
                        <TransactionsTable portfolioId={position.portfolioId} positionId={position.id} currency={currency.data} />
                    </td>
                </tr>
            }
            </>
            <ModalWrapper isOpen={createModalIsOpen} closeModal={() => setCreateModalIsOpen(false)}>
                <CreateTransactionModal closeModal={() => setCreateModalIsOpen(false)} portfolioId={position.portfolioId} positionId={position.id} />
            </ModalWrapper>
            <ModalWrapper isOpen={updateModalIsOpen} closeModal={() => setUpdateModalIsOpen(false)}>
                <EditPositionModal closeModal={() => setUpdateModalIsOpen(false)} position={position} />
            </ModalWrapper>
        </>
    )
}