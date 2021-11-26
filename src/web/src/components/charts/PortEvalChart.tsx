import React from 'react';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { ChartConfig, isPriceDataChart } from '../../types';
import LoadingWrapper from '../ui/LoadingWrapper';
import LineChart from './LineChart';
import { useGetCurrencyQuery } from '../../redux/api/currencyApi';
import { skipToken } from '@reduxjs/toolkit/dist/query';
import { getPriceString, getPerformanceString } from '../utils/string';
import { convertDashToStrokeDashArray, calculateXAxisInterval, getChartDateRange, getChartFrequency, getXAxisD3Format } from '../utils/chart';
import { useGetChartDataQuery } from '../../redux/api/chartApi';

type Props = {
    chart: ChartConfig;   
}

export default function PortEvalChart({ chart }: Props): JSX.Element {
    const [from, to] = getChartDateRange(chart);
    const frequency = getChartFrequency(chart);

    const chartData = useGetChartDataQuery({ chart, frequency, from: from.toISO(), to: to.toISO() });
    const currency = useGetCurrencyQuery(isPriceDataChart(chart) ? chart.currencyCode : skipToken);

    const lineSettings = chart.lines.map(line => ({
        width: line.width,
        color: line.color,
        strokeDashArray: convertDashToStrokeDashArray(line.dash)
    }));

    const config = {
        yFormat: isPriceDataChart(chart)
            ? (yValue: number) => getPriceString(yValue, currency.data?.symbol)
            : (yValue: number) => getPerformanceString(yValue),
        xFormat: getXAxisD3Format(from, to),
        xInterval: calculateXAxisInterval(from, to)
    };

    const isLoaded = checkIsLoaded(chartData, currency);
    const isError = checkIsError(chartData, currency);

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <LineChart config={config} data={chartData.data ?? []} lineSettings={lineSettings} />
        </LoadingWrapper>
    )
}