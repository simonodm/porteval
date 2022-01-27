import * as d3 from 'd3';
import { DateTime, Duration } from 'luxon';
import { ChartConfig, ChartFrequency, ChartLine, ChartLineDashType, ChartToDateRange, Instrument, isAggregatedChart, Portfolio, Position } from '../../types';
import { XAxisInterval } from '../charts/LineChart';

export function convertDashToStrokeDashArray(dash: ChartLineDashType): string {
    switch(dash) {
        case 'solid':
            return '0';
        case 'dashed':
            return '6';
        case 'dotted':
            return '2';
    }
}

export function calculateXAxisInterval(from: DateTime, to: DateTime): XAxisInterval {
    const diff = to.diff(from);

    if(diff >= Duration.fromObject({ years: 2 })) return 'year';
    if(diff >= Duration.fromObject({ months: 6 })) return 'month';
    if(diff >= Duration.fromObject({ months: 1 })) return 'week';
    if(diff >= Duration.fromObject({ days: 2 })) return 'day';

    return 'hour';
}

export function getXAxisD3Interval(interval: XAxisInterval | undefined): d3.TimeInterval {
    switch(interval) {
        case 'hour':
            return d3.timeHour;
        case 'day':
            return d3.timeDay;
        case undefined:
        case 'week':
            return d3.timeWeek;
        case 'month':
            return d3.timeMonth;
        case 'year':
            return d3.timeYear;
    }
}

export function getXAxisD3Format(from: DateTime, to: DateTime): (date: Date) => string {
    const diff = to.diff(from);

    if(diff >= Duration.fromObject({ years: 2 })) {
        return d3.timeFormat('%Y');
    }
    if(diff >= Duration.fromObject({ months: 6 })) {
        return d3.timeFormat('%b %Y');
    }
    if(diff >= Duration.fromObject({ days: 2 })) {
        return d3.timeFormat('%b %d');
    }

    return d3.timeFormat('%H:%M');
}

export function calculateAppropriateChartFrequency(from: DateTime, to: DateTime): ChartFrequency {
    const diff = to.diff(from);

    if(diff > Duration.fromObject({ years: 10 })) return 'year';
    if(diff > Duration.fromObject({ years: 2 })) return 'month';
    if(diff > Duration.fromObject({ years: 1})) return 'week';
    if(diff > Duration.fromObject({ days: 7 })) return 'day';
    if(diff > Duration.fromObject({ days: 1 })) return 'hour';

    return '5min';
}

export function getChartFrequency(chart: ChartConfig): ChartFrequency {
    if(isAggregatedChart(chart)) {
        return chart.frequency;
    }

    const [from, to] = getChartDateRange(chart);
    return calculateAppropriateChartFrequency(from, to);
}

export function getChartDateRange(chart: ChartConfig): [DateTime, DateTime] {
    const currentTime = DateTime.now();

    let from, to;
    if(chart.isToDate) {
        // round down to 5 minutes
        const roundedTime = DateTime.fromMillis(currentTime.toMillis() - (currentTime.toMillis() % (300 * 1000)));
        from = roundedTime.minus(getDurationFromToDateRange(chart.toDateRange));
        to = roundedTime;
    }
    else {
        from = DateTime.fromISO(chart.dateRangeStart)
        to = DateTime.fromISO(chart.dateRangeEnd);
    }

    return [from, to];
}

export function generateDefaultInstrumentChart(instrument: Instrument): ChartConfig {
    const instrumentPriceLine: ChartLine = {
        instrumentId: instrument.id,
        width: 1,
        name: instrument.symbol,
        dash: 'solid',
        color: '#0000FF',
        type: 'instrument'
    }

    const instrumentPriceChart: ChartConfig = {
        type: 'price',
        name: 'New chart',
        currencyCode: `${instrument.currencyCode}`,
        isToDate: true,
        toDateRange: '5days',
        lines: [instrumentPriceLine]
    }

    return instrumentPriceChart;
}

export function generateDefaultPortfolioChart(portfolio: Portfolio): ChartConfig {
    const portfolioPriceLine: ChartLine = {
        portfolioId: portfolio.id,
        width: 1,
        name: portfolio.name,
        dash: 'solid',
        color: '#0000FF',
        type: 'portfolio'
    }

    const portfolioPriceChart: ChartConfig = {
        type: 'price',
        name: 'New chart',
        currencyCode: `${portfolio.currencyCode}`,
        isToDate: true,
        toDateRange: '5days',
        lines: [portfolioPriceLine]
    }

    return portfolioPriceChart;
}

export function generateDefaultPositionChart(position: Position): ChartConfig {
    const positionPriceLine: ChartLine = {
        positionId: position.id,
        width: 1,
        name: position.instrument.symbol,
        dash: 'solid',
        color: '#0000FF',
        type: 'position'
    };

    const positionPriceChart: ChartConfig = {
        type: 'price',
        name: 'New chart',
        currencyCode: position.instrument.currencyCode,
        isToDate: true,
        toDateRange: '5days',
        lines: [positionPriceLine]
    };

    return positionPriceChart;
}

function getDurationFromToDateRange(toDateRange: ChartToDateRange) {
    switch(toDateRange) {
        case '1day':
            return Duration.fromObject({ days: 1 });
        case '5days':
            return Duration.fromObject({ days: 5 });
        case '1month':
            return Duration.fromObject({ months: 1 });
        case '3months':
            return Duration.fromObject({ months: 3 });
        case '6months':
            return Duration.fromObject({ months: 6 });
        case '1year':
        default:
            return Duration.fromObject({ years: 1 });
    }
}