import React, { useContext, useEffect, useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import ChartLineConfigurationContext from '../../context/ChartLineConfigurationContext';
import CurrencyDropdown from '../forms/fields/CurrencyDropdown';
import DateTimeSelector from '../forms/fields/DateTimeSelector';
import useUserSettings from '../../hooks/useUserSettings';
import NumberInput from '../forms/fields/NumberInput';

import { isAfter, isBefore, subMonths } from 'date-fns';
import { Popover } from 'react-tiny-popover';
import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import { ChartConfig, isPriceDataChart, isAggregatedChart,
    AggregationFrequency, ChartType, ChartToDateRange } from '../../types';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { camelToProperCase } from '../../utils/string';

import 'react-datepicker/dist/react-datepicker.css';
import './ChartConfigurator.css';

type Props = {
    /**
     * A callback which is invoked every time the chart configuration changes.
     */
    onChange?: (chart: ChartConfig) => void;
}

/**
 * Renders a chart configuration component, enabling modification of the chart's type, date range, frequency, and currency.
 *
 * @category Chart
 * @component
 */
function ChartConfigurator({ onChange }: Props): JSX.Element {
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

    // get default currency from query response if there is one, set to USD otherwise
    const defaultCurrency = currencies.data ? currencies.data.find(c => c.isDefault)?.code ?? 'USD' : 'USD';

    const [currentChart, setCurrentChart] = useState(context.chart);
    const [isPopoverOpen, setIsPopoverOpen] = useState(false);
    const [isCustomToDateRange, setIsCustomToDateRange] = useState(false);

    const isLoaded = checkIsLoaded(currencies);
    const isError = checkIsError(currencies);

    // adjust internal state chart if context chart changes
    useEffect(() => {
        setCurrentChart(context.chart);
    }, [context.chart]);

    const handleTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const type = e.target.value as ChartType;

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

    const handleFrequencyChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        if(currentChart && isAggregatedChart(currentChart)) {
            const newChart = {...currentChart, frequency: e.target.value as AggregationFrequency};
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

            if(!toDateRanges.reduce((prev, curr) => prev || range === curr, false)) {
                setIsCustomToDateRange(true);
            } else {
                setIsCustomToDateRange(false);
            }
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
                        value={currentChart.isToDate
                            ? undefined
                            : new Date(currentChart.dateRangeStart)}
                    />
                    <DateTimeSelector
                        className='chart-configurator-setting'
                        dateFormat={userSettings.dateFormat}
                        label='Range end'
                        onChange={handleEndDateChange}
                        value={currentChart.isToDate
                            ? undefined
                            : new Date(currentChart.dateRangeEnd)}
                    />
                    <span className="chart-configurator-setting">
                        <label htmlFor="to-date-range">To-date range</label>
                        <div id="to-date-range">
                            {
                                toDateRanges.map(range =>
                                    <button
                                        className={
                                            'btn btn-sm ' +
                                            (!isCustomToDateRange
                                                && currentChart.isToDate
                                                && currentChart.toDateRange.unit === range.unit
                                                && currentChart.toDateRange.value === range.value
                                                ? 'btn-dark'
                                                : 'btn-light')
                                        }
                                        key={range.value + range.unit}
                                        onClick={() => handleToDateRangeChange(range)}
                                        type="button"
                                    >{range.value + range.unit[0].toUpperCase()}
                                    </button>) 
                            }
                            <Popover
                                content={() =>
                                    <div className="popover-content">
                                        <NumberInput
                                            label="Custom range (in days)"
                                            onChange={(days) => handleToDateRangeChange({ unit: 'day', value: days })}
                                        />
                                    </div>
                                }
                                isOpen={isPopoverOpen}
                                onClickOutside={() => setIsPopoverOpen(false)}
                                positions={['bottom']}
                            >
                                <button
                                    className={
                                        'btn btn-sm ' +
                                        (isCustomToDateRange ? 'btn-dark' : 'btn-light')
                                    }
                                    onClick={() => setIsPopoverOpen(!isPopoverOpen)}
                                    type="button"
                                >
                                    Custom
                                </button>
                            </Popover>
                                
                        </div>
                    </span>
                </form>
            }
        </LoadingWrapper>
    )
}

export default ChartConfigurator;