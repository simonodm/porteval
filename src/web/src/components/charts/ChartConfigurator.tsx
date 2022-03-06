import React, { useContext, useEffect, useState } from 'react';

import { DateTime } from 'luxon';

import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import { ChartConfig, isPriceDataChart, isAggregatedChart,
    AggregationFrequency, ChartType, ChartToDateRange } from '../../types';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import LoadingWrapper from '../ui/LoadingWrapper';

import 'react-datepicker/dist/react-datepicker.css';
import './ChartConfigurator.css';
import { camelToProperCase } from '../../utils/string';
import ChartLineConfigurationContext from '../../context/ChartLineConfigurationContext';
import CurrencyDropdown from '../forms/fields/CurrencyDropdown';
import DateTimeSelector from '../forms/fields/DateTimeSelector';
import useUserSettings from '../../hooks/useUserSettings';

type Props = {
    onChange?: (chart: ChartConfig) => void;
}

export default function ChartConfigurator({ onChange }: Props): JSX.Element {
    const frequencies: AggregationFrequency[] = ['day', 'week', 'month', 'year'];
    const types: ChartType[] = ['price', 'profit', 'performance', 'aggregatedProfit', 'aggregatedPerformance'];
    const toDateRanges: ChartToDateRange[] = [
        {unit: 'day', value: 1},
        {unit: 'day', value: 5},
        {unit: 'month', value: 1},
        {unit: 'month', value: 3},
        {unit: 'month', value: 6},
        {unit: 'year', value: 1}
    ];

    const [userSettings] = useUserSettings();

    const context = useContext(ChartLineConfigurationContext);

    const currencies = useGetAllKnownCurrenciesQuery();
    const defaultCurrency = currencies.data ? currencies.data.find(c => c.isDefault)?.code ?? 'USD' : 'USD';

    const [currentChart, setCurrentChart] = useState(context.chart);
    const isLoaded = checkIsLoaded(currencies);
    const isError = checkIsError(currencies);

    useEffect(() => {
        setCurrentChart(context.chart);
    }, [context.chart]);

    const handleTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const type = e.target.value as ChartType;

        if(currentChart) {
            const newChart: ChartConfig = {...currentChart};

            newChart.type = type;

            if(newChart.type === 'aggregatedProfit' || newChart.type === 'aggregatedPerformance') {
                const frequency = isAggregatedChart(currentChart) ? newChart.frequency : 'week';
                newChart.frequency = frequency;
            }

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

    const handleFrequencyChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        if(currentChart && isAggregatedChart(currentChart)) {
            const newChart = {...currentChart, frequency: e.target.value as AggregationFrequency};
            setCurrentChart(newChart);
            onChange && onChange(newChart);
        }
    }

    const handleStartDateChange = (date: DateTime) => {
        if(currentChart) {
            const newChart: ChartConfig = {
                ...currentChart,
                isToDate: false,
                dateRangeStart: date.toISO(),
                dateRangeEnd: currentChart.isToDate ? DateTime.now().toISO() : currentChart.dateRangeEnd
            }
            setCurrentChart(newChart);
            onChange && onChange(newChart);
        }
    }

    const handleEndDateChange = (date: DateTime) => {
        if(currentChart) {
            const newChart: ChartConfig = {
                ...currentChart,
                isToDate: false,
                dateRangeStart: currentChart.isToDate
                    ? date.minus({ months: 1 }).toISO()
                    : currentChart.dateRangeStart,
                dateRangeEnd: date.toISO()
            }
            setCurrentChart(newChart);
            onChange && onChange(newChart);
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
                <form autoComplete="off" className="chart-configurator" onSubmit={(e) => e.preventDefault() }>
                    <span className="chart-configurator-setting">
                        <label htmlFor="chart-type">Type:</label>
                        <select className="form-select" id="chart-type" onChange={handleTypeChange}>
                            {types.map(type =>
                                <option
                                    key={type}
                                    selected={currentChart.type === type}
                                    value={type}
                                >{camelToProperCase(type)}
                                </option>)}
                        </select>
                    </span>
                    {
                        isPriceDataChart(currentChart) 
                            &&
                            <CurrencyDropdown
                                className='chart-configurator-setting'
                                currencies={currencies.data ?? []}
                                onChange={handleCurrencyChange}
                                value={currentChart.currencyCode}
                            />
                    }
                    {
                        currentChart && isAggregatedChart(currentChart)
                            && 
                            <span className="chart-configurator-setting">
                                <label htmlFor="chart-frequency">Frequency:</label>
                                <select className="form-select" id="chart-frequency" onChange={handleFrequencyChange}>
                                    {frequencies.map(frequency =>
                                        <option
                                            key={frequency}
                                            selected={
                                                isAggregatedChart(currentChart) && currentChart.frequency === frequency
                                            }
                                            value={frequency}
                                        >{camelToProperCase(frequency)}
                                        </option>)}
                                </select>
                            </span>
                    }
                    <DateTimeSelector
                        className='chart-configurator-setting'
                        dateFormat={userSettings.dateFormat}
                        label='Range start'
                        onChange={handleStartDateChange}
                        timeFormat={userSettings.timeFormat}
                        value={currentChart.isToDate
                            ? undefined
                            : DateTime.fromISO(currentChart.dateRangeStart)}
                    />
                    <DateTimeSelector
                        className='chart-configurator-setting'
                        dateFormat={userSettings.dateFormat}
                        label='Range start'
                        onChange={handleEndDateChange}
                        timeFormat={userSettings.timeFormat}
                        value={currentChart.isToDate
                            ? undefined
                            : DateTime.fromISO(currentChart.dateRangeEnd)}
                    />
                    <span className="chart-configurator-setting">
                        <label htmlFor="to-date-range">To-date range</label>
                        <div id="to-date-range">
                            {
                                toDateRanges.map(range =>
                                    <button
                                        className={
                                            'btn btn-sm ' +
                                            (currentChart.isToDate && currentChart.toDateRange === range
                                                ? 'btn-dark'
                                                : 'btn-light')
                                        }
                                        key={range.value + range.unit}
                                        onClick={() => handleToDateRangeChange(range)}
                                        type="button"
                                    >{range.value + range.unit[0].toUpperCase()}
                                    </button>)
                            }
                        </div>
                    </span>
                </form>
            }
        </LoadingWrapper>
    )
}
