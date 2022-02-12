import { ChartConfig, ChartLineBase, InstrumentType } from './types';

export const API_NAME_MAX_LENGTH = 64;
export const API_NOTE_MAX_LENGTH = 255;
export const CHART_TICK_INTERVAL = 100;
export const MAIN_COLOR = '#343a40';
export const REFRESH_INTERVAL = 300_000;
export const ERROR_STRING = 'An error has occured. Please try again later.';
export const CHART_TRANSACTION_SIGN_CIRCLE_RADIUS = 4;
export const CHART_TRANSACTION_SIGN_SIZE = 5;
export const DEFAULT_CHART: ChartConfig = {
    name: 'New chart',
    type: 'price',
    currencyCode: 'USD',
    isToDate: true,
    toDateRange: '5days',
    lines: []
}
export const DEFAULT_CHART_LINE: ChartLineBase = {
    name: '',
    width: 1,
    dash: 'solid',
    color: '#000000'
};

export const INSTRUMENT_TYPE_TO_STRING: Record<InstrumentType, string> = {
    'stock': 'Stock',
    'bond': 'Bond',
    'mutualFund': 'Mutual Fund',
    'etf': 'ETF',
    'commodity': 'Commodity',
    'cryptoCurrency': 'Cryptocurrency',
    'index': 'Index',
    'other': 'Other'
}