import React from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import PositionRows from './PositionRows';
import { Link, NavLink } from 'react-router-dom';

import useGetPortfolioToDatePerformanceQueryWrapper from '../../hooks/useGetPortfolioToDatePerformanceQueryWrapper';
import useGetPortfolioToDateProfitsQueryWrapper from '../../hooks/useGetPortfolioToDateProfitsQueryWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useGetCurrencyQuery } from '../../redux/api/currencyApi';
import { useDeletePortfolioMutation } from '../../redux/api/portfolioApi';
import { useState } from 'react';

import { Portfolio } from '../../types';
import { getPriceString, getPerformanceString } from '../utils/string';
import ModalWrapper from '../modals/ModalWrapper';
import CreatePositionModal from '../modals/CreatePositionModal';
import EditPortfolioModal from '../modals/EditPortfolioModal';
import { generateDefaultPortfolioChart } from '../utils/chart';

type Props = {
    portfolio: Portfolio,
    expanded?: boolean,
}

export default function PortfolioRow({ portfolio }: Props): JSX.Element {
    const currency = useGetCurrencyQuery(portfolio.currencyCode);

    const [isRemoved, setIsRemoved] = useState(false);

    const profitData = useGetPortfolioToDateProfitsQueryWrapper(portfolio.id, isRemoved);
    const performanceData = useGetPortfolioToDatePerformanceQueryWrapper(portfolio.id, isRemoved);
    
    const [ deletePortfolio ] = useDeletePortfolioMutation();

    const [portfolioExpanded, setPortfolioExpanded] = useState(false);
    const [createModalIsOpen, setCreateModalIsOpen] = useState(false);
    const [updateModalIsOpen, setUpdateModalIsOpen] = useState(false);

    const isLoaded = checkIsLoaded(currency, profitData, performanceData);
    const isError = checkIsError(currency, profitData, performanceData);

    return (
        <>
            <tr key={'p_' + portfolio.name}>
                <td>
                    <i role="button" className="bi bi-arrow-down-short" onClick={() => setPortfolioExpanded(!portfolioExpanded)}></i>
                    <Link to={`/portfolios/${portfolio.id}`}>{portfolio.name}</Link>
                </td>
                <td></td>
                <td>
                    {portfolio.currencyCode}
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
                <td>{portfolio.note}</td>
                <td>
                    <button role="button" className="btn btn-primary btn-extra-sm mr-1" onClick={() => setCreateModalIsOpen(true)}>Add position</button>
                    <button role="button" className="btn btn-primary btn-extra-sm mr-1" onClick={() => setUpdateModalIsOpen(true)}>Edit</button>
                    <NavLink className="btn btn-primary btn-extra-sm mr-1" to={{pathname: '/charts/view', state: {chart: generateDefaultPortfolioChart(portfolio)}}}>Chart</NavLink>
                    <button role="button" className="btn btn-danger btn-extra-sm" onClick={() => { deletePortfolio(portfolio.id); setIsRemoved(true) }}>Remove</button>
                </td>
            </tr>
            <>
            { portfolioExpanded &&
                <PositionRows portfolioId={portfolio.id} />
            }
            </>
            <ModalWrapper isOpen={createModalIsOpen} closeModal={() => setCreateModalIsOpen(false)}>
                <CreatePositionModal closeModal={() => setCreateModalIsOpen(false)} portfolioId={portfolio.id} />
            </ModalWrapper>
            <ModalWrapper isOpen={updateModalIsOpen} closeModal={() => setUpdateModalIsOpen(false)}>
                <EditPortfolioModal closeModal={() => setUpdateModalIsOpen(false)} portfolio={portfolio} />
            </ModalWrapper>
        </>
    )
}