import React, { useState } from 'react';

import { Link, NavLink } from 'react-router-dom';

import LoadingWrapper from '../ui/LoadingWrapper';

import useGetPortfolioToDatePerformanceQueryWrapper from '../../hooks/useGetPortfolioToDatePerformanceQueryWrapper';
import useGetPortfolioToDateProfitsQueryWrapper from '../../hooks/useGetPortfolioToDateProfitsQueryWrapper';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { useGetCurrencyQuery } from '../../redux/api/currencyApi';
import { useDeletePortfolioMutation } from '../../redux/api/portfolioApi';


import { Portfolio } from '../../types';
import { getPriceString, getPerformanceString } from '../../utils/string';
import ModalWrapper from '../modals/ModalWrapper';
import { generateDefaultPortfolioChart } from '../../utils/chart';
import EditPortfolioForm from '../forms/EditPortfolioForm';
import OpenPositionForm from '../forms/OpenPositionForm';

import useUserSettings from '../../hooks/useUserSettings';

import PositionRows from './PositionRows';

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

    const [userSettings] = useUserSettings();

    const isLoaded = checkIsLoaded(currency, profitData, performanceData);
    const isError = checkIsError(currency, profitData, performanceData);

    return (
        <>
            <tr key={'p_' + portfolio.name}>
                <td>
                    <i
                        className="bi bi-arrow-down-short"
                        onClick={() => setPortfolioExpanded(!portfolioExpanded)}
                        role="button"
                    >
                    </i>
                    <Link to={`/portfolios/${portfolio.id}`}>{portfolio.name}</Link>
                </td>
                <td></td>
                <td>
                    {portfolio.currencyCode}
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
                <td>{portfolio.note}</td>
                <td>
                    <button
                        className="btn btn-primary btn-extra-sm mr-1"
                        onClick={() => setCreateModalIsOpen(true)}
                        role="button"
                    >Open position
                    </button>
                    <button
                        className="btn btn-primary btn-extra-sm mr-1"
                        onClick={() => setUpdateModalIsOpen(true)}
                        role="button"
                    >Edit
                    </button>
                    <NavLink
                        className="btn btn-primary btn-extra-sm mr-1"
                        to={{pathname: '/charts/view', state: {chart: generateDefaultPortfolioChart(portfolio)}}}
                    >Chart
                    </NavLink>
                    <button className="btn btn-danger btn-extra-sm"
                        onClick={() => {
                            deletePortfolio(portfolio.id); setIsRemoved(true) 
                        }}
                        role="button"
                    >Remove
                    </button>
                </td>
            </tr>
            { portfolioExpanded &&
                <PositionRows portfolioId={portfolio.id} />
            }
            <ModalWrapper closeModal={() => setCreateModalIsOpen(false)} heading="Open new position"
                isOpen={createModalIsOpen}
            >
                <OpenPositionForm onSuccess={() => setCreateModalIsOpen(false)} portfolioId={portfolio.id} />
            </ModalWrapper>
            <ModalWrapper closeModal={() => setUpdateModalIsOpen(false)} heading={`Edit ${portfolio.name}`}
                isOpen={updateModalIsOpen}
            >
                <EditPortfolioForm onSuccess={() => setUpdateModalIsOpen(false)} portfolio={portfolio} />
            </ModalWrapper>
        </>
    )
}