import React, { useContext, useEffect, useState } from 'react';

import DatePicker from 'react-datepicker';

import { DateTime } from 'luxon';

import { useGetAllKnownCurrenciesQuery } from '../../redux/api/currencyApi';
import { ChartConfig, isPriceDataChart, isAggregatedChart,
    ChartFrequency, ChartType, ChartToDateRange } from '../../types';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import LoadingWrapper from '../ui/LoadingWrapper';

import 'react-datepicker/dist/react-datepicker.css';
import './ChartConfigurator.css';
import { camelToProperCase } from '../../utils/string';
import ChartLineConfigurationContext from '../../context/ChartLineConfigurationContext';

type Props = {
    onChange?: (chart: ChartConfig) => void;
}

export default function ChartConfigurator({ onChange }: Props): JSX.Element {
    const frequencies: ChartFrequency[] = ['day', 'week', 'month', 'year'];
    const types: ChartType[] = ['price', 'profit', 'performance', 'aggregatedProfit', 'aggregatedPerformance'];
    const toDateRanges: ChartToDateRange[] = ['1day', '5days', '1month', '3months', '6months', '1year'];

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

    const handleCurrencyChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        if(currentChart && isPriceDataChart(currentChart)) {
            const newChart = {...currentChart, currencyCode: e.target.value};
            setCurrentChart(newChart);
            onChange && onChange(newChart);
        }
    }

    const handleFrequencyChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        if(currentChart && isAggregatedChart(currentChart)) {
            const newChart = {...currentChart, frequency: e.target.value as ChartFrequency};
            setCurrentChart(newChart);
            onChange && onChange(newChart);
        }
    }

    const handleStartDateChange = (date: Date) => {
        if(currentChart) {
            const newChart: ChartConfig = {
                ...currentChart,
                isToDate: false,
                dateRangeStart: date.toISOString(),
                dateRangeEnd: currentChart.isToDate ? DateTime.now().toISO() : currentChart.dateRangeEnd
            }
            setCurrentChart(newChart);
            onChange && onChange(newChart);
        }
    }

    const handleEndDateChange = (date: Date) => {
        if(currentChart) {
            const newChart: ChartConfig = {
                ...currentChart,
                isToDate: false,
                dateRangeStart: currentChart.isToDate
                    ? DateTime.fromJSDate(date).minus({ months: 1 }).toISO()
                    : currentChart.dateRangeStart,
                dateRangeEnd: date.toISOString()
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
                            <span className="chart-configurator-setting">
                                <label htmlFor="chart-currency">Currency:</label>
                                <select className="form-select" id="chart-currency" onChange={handleCurrencyChange}>
                                    {currencies.data?.map(currency =>
                                        <option
                                            key={currency.code}
                                            selected={currentChart.currencyCode === currency.code}
                                            value={currency.code}
                                        >{currency.code}
                                        </option>)}
                                </select>
                            </span>
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
                    <span className="chart-configurator-setting">
                        <label htmlFor="chart-date-start">Range start:</label>
                        <DatePicker 
                            id="chart-date-start"
                            onChange={handleStartDateChange}
                            popperClassName="chart-datepicker-popper"
                            selected={
                                currentChart.isToDate
                                    ? undefined
                                    : DateTime.fromISO(currentChart.dateRangeStart).toJSDate()
                            }
                            wrapperClassName="chart-datepicker-wrapper"
                        />
                    </span>
                    <span className="chart-configurator-setting">
                        <label htmlFor="chart-date-end">Range end:</label>
                        <DatePicker 
                            id="chart-date-end"
                            onChange={handleEndDateChange}
                            popperClassName="chart-datepicker-popper"
                            selected={
                                currentChart.isToDate
                                    ? undefined
                                    : DateTime.fromISO(currentChart.dateRangeEnd).toJSDate()
                            }
                            wrapperClassName="chart-datepicker-wrapper"
                        />
                    </span>
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
                                        key={range}
                                        onClick={() => handleToDateRangeChange(range)}
                                        type="button"
                                    >{range[0] + range[1].toUpperCase()}
                                    </button>)
                            }
                        </div>
                    </span>
                </form>
            }
        </LoadingWrapper>
    )
}