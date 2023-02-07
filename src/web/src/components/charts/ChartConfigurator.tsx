import React, { useContext, useEffect, useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import ChartLineConfigurationContext from '../../context/ChartLineConfigurationContext';
import CurrencyDropdown from '../forms/fields/CurrencyDropdown';
import DateTimeSelector from '../forms/fields/DateTimeSelector';
import useUserSettings from '../../hooks/useUserSettings';
import ToDateRangeSelector from '../forms/fields/ToDateRangeSelector';
import ChartFrequencyDropdown from '../forms/fields/ChartFrequencyDropdown';
import ChartTypeDropdown from '../forms/fields/ChartTypeDropdown';

import Form from 'react-bootstrap/Form';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

import { isAfter, isBefore, subMonths } from 'date-fns';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import { ChartConfig, isPriceDataChart, isAggregatedChart,
    AggregationFrequency, ChartType, ChartToDateRange } from '../../types';
import { checkIsLoaded, checkIsError } from '../../utils/queries';

import './ChartConfigurator.css';


type Props = {
    /**
     * A callback which is invoked every time the chart configuration changes.
     */
    onChange?: (chart: ChartConfig) => void;
}

/**
 * Renders a chart configuration component, enabling modification of the chart's type, date range,
 * frequency, and currency.
 *
 * @category Chart
 * @component
 */
function ChartConfigurator({ onChange }: Props): JSX.Element {
    const [userSettings] = useUserSettings();
    const context = useContext(ChartLineConfigurationContext);
    const currencies = useGetAllKnownCurrenciesQuery();

    // get default currency from query response if there is one, set to USD otherwise
    const defaultCurrency = currencies.data ? currencies.data.find(c => c.isDefault)?.code ?? 'USD' : 'USD';

    const [currentChart, setCurrentChart] = useState(context.chart);

    const isLoaded = checkIsLoaded(currencies);
    const isError = checkIsError(currencies);

    // adjust internal state chart if context chart changes
    useEffect(() => {
        setCurrentChart(context.chart);
    }, [context.chart]);

    const handleTypeChange = (type: ChartType) => {
        if(currentChart) {
            const newChart: ChartConfig = {...currentChart};

            newChart.type = type;

            // set frequency if new chart type is aggregated
            if(newChart.type === 'aggregatedProfit' || newChart.type === 'aggregatedPerformance') {
                const frequency = isAggregatedChart(currentChart) ? newChart.frequency : 'week';
                newChart.frequency = frequency;
            }

            // set currency if new chart type is price-based
            if(newChart.type === 'price' || newChart.type === 'profit' || newChart.type === 'aggregatedProfit') {
                const currencyCode = isPriceDataChart(currentChart) ? currentChart.currencyCode : defaultCurrency;
                newChart.currencyCode = currencyCode;
            }
            
            setCurrentChart(newChart);
            onChange && onChange(newChart);
        }
    }

    const handleCurrencyChange = (currencyCode: string) => {
        if(currentChart && isPriceDataChart(currentChart)) {
            const newChart = {...currentChart, currencyCode};
            setCurrentChart(newChart);
            onChange && onChange(newChart);
        }
    }

    const handleFrequencyChange = (frequency: AggregationFrequency) => {
        if(currentChart && isAggregatedChart(currentChart)) {
            const newChart = {...currentChart, frequency};
            setCurrentChart(newChart);
            onChange && onChange(newChart);
        }
    }

    const handleStartDateChange = (date: Date) => {
        if(currentChart) {
            if(currentChart.isToDate || isBefore(date, new Date(currentChart.dateRangeEnd))) {
                const newChart: ChartConfig = {
                    ...currentChart,
                    isToDate: false,
                    dateRangeStart: date.toISOString(),
                    dateRangeEnd: currentChart.isToDate ? new Date().toISOString() : currentChart.dateRangeEnd
                }
                setCurrentChart(newChart);
                onChange && onChange(newChart);
            }
        }
    }

    const handleEndDateChange = (date: Date) => {
        if(currentChart) {
            if(currentChart.isToDate || isAfter(date, new Date(currentChart.dateRangeStart))) {
                const newChart: ChartConfig = {
                    ...currentChart,
                    isToDate: false,
                    dateRangeStart: currentChart.isToDate
                        ? subMonths(date, 1).toISOString()
                        : currentChart.dateRangeStart,
                    dateRangeEnd: date.toISOString()
                }
                setCurrentChart(newChart);
                onChange && onChange(newChart);
            }
        }
    }

    const handleToDateRangeChange = (range: ChartToDateRange) => {
        if(currentChart) {
            const newChart: ChartConfig = {
                ...currentChart,
                isToDate: true,
                toDateRange: range
            }
            setCurrentChart(newChart);
            onChange && onChange(newChart);
        }
    }

    return (
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            { currentChart && 
                <Form autoComplete="off" onSubmit={(e) => e.preventDefault() }>
                    <Row className="align-items-center justify-content-xs-start justify-content-md-center">
                        <Col xs="auto">
                            <ChartTypeDropdown
                                className="chart-configurator-setting"
                                value={currentChart.type}
                                onChange={handleTypeChange}
                                label='Frequency'
                            />
                        </Col>
                        {
                            isPriceDataChart(currentChart) &&
                                <Col xs="auto">
                                    <CurrencyDropdown
                                        className="chart-configurator-setting" 
                                        currencies={currencies.data ?? []}
                                        onChange={handleCurrencyChange}
                                        value={currentChart.currencyCode}
                                    />
                                </Col>
                        }
                        {
                            isAggregatedChart(currentChart) &&
                                <Col xs="auto">
                                    <ChartFrequencyDropdown
                                        className="chart-configurator-setting"
                                        label='Frequency'
                                        value={currentChart.frequency}
                                        onChange={handleFrequencyChange}
                                    />
                                </Col>
                        }
                        <Col xs="auto">
                            <DateTimeSelector
                                className='chart-configurator-setting'
                                dateFormat={userSettings.dateFormat}
                                label='Range start'
                                onChange={handleStartDateChange}
                                value={currentChart.isToDate
                                    ? undefined
                                    : new Date(currentChart.dateRangeStart)}
                            />
                        </Col>
                        <Col xs="auto">
                            <DateTimeSelector
                                className='chart-configurator-setting'
                                dateFormat={userSettings.dateFormat}
                                label='Range end'
                                onChange={handleEndDateChange}
                                value={
                                    currentChart.isToDate
                                        ? undefined
                                        : new Date(currentChart.dateRangeEnd)
                                }
                            />
                        </Col>
                        <Col xs="auto">
                            <ToDateRangeSelector
                                className='chart-configurator-setting'
                                label='To-date range'
                                onChange={handleToDateRangeChange}
                                value={
                                    currentChart.isToDate
                                        ? currentChart.toDateRange
                                        : undefined
                                }
                            />
                        </Col>
                    </Row>
                </Form>
            }
        </LoadingWrapper>
    )
}

export default ChartConfigurator;