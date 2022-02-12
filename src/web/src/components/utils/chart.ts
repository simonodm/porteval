import * as d3 from 'd3';
import { DateTime, Duration } from 'luxon';
import { CHART_TRANSACTION_SIGN_CIRCLE_RADIUS, CHART_TRANSACTION_SIGN_SIZE } from '../../constants';
import { ChartConfig, ChartFrequency, ChartLine, ChartLineDashType, ChartToDateRange, Instrument, isAggregatedChart, Portfolio, Position, Transaction } from '../../types';
import { Line, XAxisInterval } from '../charts/LineChart';
import { RenderedDataPointInfo } from './lineChart';

type LineWithTransactions = Line & {
    transactions: Array<Transaction>
};

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

export function getXAxisD3Interval(interval: XAxisInterval | undefined): d3.CountableTimeInterval {
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

export function generateTooltipTransactionList(lines: Array<LineWithTransactions>, from: string | undefined, to: string | undefined): HTMLElement | null {
    const transactions = lines.reduce<Array<Transaction>>((prev, curr) => {
        return prev.concat(findLineTransactionsInRange(curr.transactions, from, to));
    }, [])

    if(transactions.length > 0) {
        const rootElement = document.createElement('div');
        const transactionsHeader = document.createElement('p');
        transactionsHeader.className = 'tooltip-transactions-header';
        transactionsHeader.innerHTML = 'Transactions:';
        rootElement.append(transactionsHeader);

        const transactionsList = document.createElement('ul');
        transactionsList.className = 'tooltip-transactions';

        transactions.forEach(transaction => {
            const transactionRowElement = document.createElement('li');
            const isPurchase = transaction.amount > 0;
            transactionRowElement.innerHTML = `${Math.abs(transaction.amount)} ${isPurchase ? 'BUY' : 'SELL'}`;
            transactionsList.append(transactionRowElement);
        });

        rootElement.append(transactionsList);
        return rootElement;
    }

    return null;
}

export function generateChartLineTransactionIcons(dataPoint: RenderedDataPointInfo, transactions: Array<Transaction>): SVGElement {
    const amount = getTransactionTotalAmount(transactions, dataPoint.point.time, dataPoint.nextPoint?.time);

    const element = document.createElementNS('http://www.w3.org/2000/svg', 'g');
    if(amount === 0) return element;

    const color = amount < 0 ? 'red' : 'green';
    const signFn = amount < 0 ? generateMinusSignSVG : generatePlusSignSVG;

    const circle = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
    circle.setAttribute('cx', `${dataPoint.x}`)
    circle.setAttribute('cy', `${dataPoint.y}`)
    circle.setAttribute('r', `${CHART_TRANSACTION_SIGN_CIRCLE_RADIUS}`);
    circle.setAttribute('stroke', color);
    circle.setAttribute('fill', color);
    circle.setAttribute('stroke-width', '1');
    element.append(circle);
    
    const sign = signFn(dataPoint.x, dataPoint.y, CHART_TRANSACTION_SIGN_SIZE, 'white');
    element.append(sign);

    return element;
}

function getTransactionTotalAmount(transactions: Array<Transaction>, from: string | undefined, to: string | undefined): number {
    const transactionsInRange = findLineTransactionsInRange(transactions, from, to);
    return transactionsInRange.reduce((prev, curr) => prev + curr.amount, 0);
}

function findLineTransactionsInRange(transactions: Array<Transaction>, from: string | undefined, to: string | undefined): Array<Transaction> {
    const convertedFrom = from && DateTime.fromISO(from);
    const convertedTo = to && DateTime.fromISO(to);

    return transactions.filter(t => {
        const time = DateTime.fromISO(t.time)
        return (convertedFrom === undefined || time >= convertedFrom) && (convertedTo === undefined || time <= convertedTo);
    });
}

function generatePlusSignSVG(x: number, y: number, size: number, color: string): SVGElement {
    const sign = document.createElementNS('http://www.w3.org/2000/svg', 'path');
    sign.setAttribute('d', `M ${x-size/2},${y} H ${x+size/2} M ${x},${y+size/2} V ${y-size/2}`);
    sign.setAttribute('stroke', color);
    return sign;
}

function generateMinusSignSVG(x: number, y: number, size: number, color: string): SVGElement {
    const sign = document.createElementNS('http://www.w3.org/2000/svg', 'path');
    sign.setAttribute('d', `M ${x-size/2},${y} H ${x+size/2}`);
    sign.setAttribute('stroke', color);
    return sign;
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