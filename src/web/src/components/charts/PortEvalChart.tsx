import React from 'react';

import { skipToken } from '@reduxjs/toolkit/dist/query';

import { useGetCurrencyQuery } from '../../redux/api/currencyApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { ChartConfig, isPriceDataChart } from '../../types';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { getPriceString, getPerformanceString } from '../../utils/string';
import { convertDashToStrokeDashArray, calculateXAxisInterval, getChartDateRange,
    getChartFrequency, getXAxisD3Format, generateTooltipTransactionList,
    generateChartLineTransactionIcons } from '../../utils/chart';
import { useGetChartDataQuery, useGetChartTransactionsQuery } from '../../redux/api/chartApi';
import { RenderedDataPointInfo } from '../../utils/lineChart';

import LineChart from './LineChart';

type Props = {
    chart: ChartConfig;   
}

export default function PortEvalChart({ chart }: Props): JSX.Element {
    const [from, to] = getChartDateRange(chart);
    const frequency = getChartFrequency(chart);

    const chartData = useGetChartDataQuery({ chart, frequency, from: from.toISO(), to: to.toISO() });
    const transactionData = useGetChartTransactionsQuery({ chart, from: from.toISO(), to: to.toISO() });
    const currency = useGetCurrencyQuery(isPriceDataChart(chart) ? chart.currencyCode : skipToken);

    const lines = chart.lines.map((line, idx) => ({
        name: line.name,
        width: line.width,
        color: line.color,
        strokeDashArray: convertDashToStrokeDashArray(line.dash),
        data: chartData?.data ? chartData.data[idx] : [],
        transactions: transactionData?.data ? transactionData.data[idx] : []
    }));

    const transactionSignRenderCallback = (dataPoint: RenderedDataPointInfo) => {
        const transactions = lines[dataPoint.lineIndex].transactions;
        return generateChartLineTransactionIcons(dataPoint, transactions);
    }

    const config = {
        yFormat: isPriceDataChart(chart)
            ? (yValue: number) => getPriceString(yValue, currency.data?.symbol)
            : (yValue: number) => getPerformanceString(yValue),
        xFormat: getXAxisD3Format(from, to),
        xInterval: calculateXAxisInterval(from, to),
        tooltipCallback: transactionData?.data
            ? (from: string | undefined, to: string | undefined) => generateTooltipTransactionList(lines, from, to)
            : () => null,
        additionalRenderCallback: transactionSignRenderCallback
    };

    const isLoaded = checkIsLoaded(chartData, transactionData, currency);
    const isError = checkIsError(chartData, transactionData, currency);

    return (
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <LineChart config={config} lines={lines} />
        </LoadingWrapper>
    )
}