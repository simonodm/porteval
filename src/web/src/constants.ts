import { ChartConfig, ChartLineBase, InstrumentType, ChartToDateRange } from './types';

export const DEFAULT_DATE_FORMAT = 'yyyy/MM/dd';
export const DEFAULT_TIME_FORMAT = 'HH:mm';
export const DEFAULT_DECIMAL_SEPARATOR = '.';
export const DEFAULT_THOUSANDS_SEPARATOR = ' ';

export const DATE_FORMAT_STORAGE_KEY = 'dateFormat';
export const TIME_FORMAT_STORAGE_KEY = 'timeFormat';
export const DECIMAL_SEPARATOR_STORAGE_KEY = 'decimalSeparator';
export const THOUSANDS_SEPARATOR_STORAGE_KEY = 'thousandsSeparator';

export const API_NAME_MAX_LENGTH = 64;
export const API_NOTE_MAX_LENGTH = 255;
export const API_MAX_CHART_LINE_WIDTH = 8;
export const CHART_TICK_INTERVAL = 100;
export const MAIN_COLOR = '#343a40';
export const REFRESH_INTERVAL = 300_000;
export const ERROR_STRING = 'An error has occured. Please try again later.';
export const CHART_TRANSACTION_SIGN_CIRCLE_RADIUS = 4;
export const CHART_TRANSACTION_SIGN_SIZE = 5;
export const LINE_PREVIEW_LENGTH = 30;
export const DEFAULT_CHART_TODATE_RANGE: ChartToDateRange = {
    unit: 'day',
    value: 5
};
export const DEFAULT_CHART: ChartConfig = {
    name: 'New chart',
    type: 'price',
    currencyCode: 'USD',
    isToDate: true,
    toDateRange: DEFAULT_CHART_TODATE_RANGE,
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