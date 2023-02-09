import { toast } from 'react-toastify';
import { ChartConfig, ChartLineBase, InstrumentType, ChartToDateRange, InstrumentSplitProcessingStatus } from './types';

/**
 * Default date format setting.
 * @category Constants
 */
export const DEFAULT_DATE_FORMAT = 'yyyy/MM/dd';

/**
 * Default time format setting.
 * @category Constants
 */
export const DEFAULT_TIME_FORMAT = 'HH:mm';

/**
 * Default decimal separator setting.
 * @category Constants
 */
export const DEFAULT_DECIMAL_SEPARATOR = '.';

/**
 * Default thousands separator setting.
 * @category Constants
 */
export const DEFAULT_THOUSANDS_SEPARATOR = ' ';

/**
 * Local storage key to store the date format setting.
 * @category Constants
 */
export const DATE_FORMAT_STORAGE_KEY = 'dateFormat';

/**
 * Local storage key to store the time format setting.
 * @category Constants
 */
export const TIME_FORMAT_STORAGE_KEY = 'timeFormat';

/**
 * Local storage key to store the decimal separator setting.
 * @category Constants
 */
export const DECIMAL_SEPARATOR_STORAGE_KEY = 'decimalSeparator';

/**
 * Local storage key to store the thousands separator setting.
 * @category Constants
 */
export const THOUSANDS_SEPARATOR_STORAGE_KEY = 'thousandsSeparator';

/**
 * Maximum entity name length allowed by PortEval's API.
 * @category Constants
 */
export const API_NAME_MAX_LENGTH = 64;

/**
 * Maximum entity note length allowed by PortEval's API.
 * @category Constants
 */
export const API_NOTE_MAX_LENGTH = 255;

/**
 * Maximum chart line width allowed by PortEval's API.
 * @category Constants
 */
export const API_MAX_CHART_LINE_WIDTH = 8;

/**
 * Default chart X axis tick interval in pixels.
 * @category Constants
 */
export const CHART_TICK_INTERVAL = 100;

/**
 * GUI main color.
 * @category Constants
 */
export const MAIN_COLOR = '#343a40';

/**
 * API data polling interval in milliseconds.
 * @category Constants
 */
export const REFRESH_INTERVAL = 300_000;

/**
 * Error string to display on unknown API error.
 * @category Constants
 */
export const ERROR_STRING = 'Failed to connect to the back-end. Please try again later.';

/**
 * Chart line transaction purchase/sell SVG circle radius in pixels.
 * @category Constants
 */
export const CHART_TRANSACTION_SIGN_CIRCLE_RADIUS = 4;

/**
 * Chart line transaction purchase/sell SVG circle sign size in pixels.
 * @category Constants
 */
export const CHART_TRANSACTION_SIGN_SIZE = 5;

/**
 * Default length of a chart line preview.
 * @category Constants
 */
export const LINE_PREVIEW_LENGTH = 30;

/**
 * Name of the event to expand all table rows.
 * @category Constants
 */
export const EXPAND_ALL_ROWS_EVENT_NAME = 'dataTableExpandAll';

/**
 * Name of the event to collapse all table rows.
 * @category Constants
 */
export const COLLAPSE_ALL_ROWS_EVENT_NAME = 'dataTableCollapseAll';

/**
 * Default chart to-date range.
 * @category Constants
 */
export const DEFAULT_CHART_TODATE_RANGE: ChartToDateRange = {
    unit: 'month',
    value: 1
};

/**
 * Default chart configuration.
 * @category Constants
 */
export const DEFAULT_CHART: ChartConfig = {
    name: 'New chart',
    type: 'price',
    currencyCode: 'USD',
    isToDate: true,
    toDateRange: DEFAULT_CHART_TODATE_RANGE,
    lines: []
}

/**
 * Default chart line configuration.
 * @category Constants
 */
export const DEFAULT_CHART_LINE: ChartLineBase = {
    name: '',
    width: 1,
    dash: 'solid',
    color: '#000000'
};

/**
 * Map between {@link InstrumentType} and its string representation in the UI.
 * @category Constants
 */
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

/**
 * Map between {@link InstrumentSplitProcessingStatus} and its string representation in the UI.
 * @category Constants
 */
export const INSTRUMENT_SPLIT_STATUS_TO_STRING: Record<InstrumentSplitProcessingStatus, string> = {
    'notProcessed': 'Not processed',
    'processed': 'Processed',
    'rollbackRequested': 'Rollback requested',
    'rolledBack': 'Rolled back'
}

/**
 * All tags used by RTK invalidation mechanism.
 * @category Constants
 */
export const RTK_API_TAGS = [
    'Portfolios',
    'Portfolio',
    'PortfolioCalculations',
    'PortfolioTransactions',
    'Positions',
    'Position',
    'PositionInstrument',
    'PositionCalculations',
    'PositionTransactions',
    'Transaction',
    'Instruments',
    'Instrument',
    'InstrumentCalculations',
    'InstrumentPrices',
    'InstrumentPrice',
    'InstrumentTransactions',
    'InstrumentSplits',
    'Charts',
    'Chart',
    'DashboardLayout',
    'Exchanges',
    'Currencies',
    'CurrencyExchangeRates',
    'Imports',
    'Import'
];

/**
 * Default options for toast notifications.
 * @category Constants
 */
export const TOAST_OPTIONS = {
    position: toast.POSITION.BOTTOM_RIGHT,
    autoClose: 5000,
    hideProgressBar: true,
    closeOnClick: true,
    pauseOnHover: true,
    draggable: true,
    progress: undefined,
}

/**
 * Default chart line colors for the first 8 lines added to the chart.
 * @category Constants
 */
export const CHART_LINE_COLOR_CODE_PROGRESSION = [
    '#00b300',
    '#ff0000',
    '#e3ae00',
    '#00bac7',
    '#b500a0',
    '#0082b5',
    '#eb5470',
    '#0000ff',
];

/**
 * A collection of responsive breakpoints used by PortEval in the key => minWidth format.
 * @category Constants
 */
export const RESPONSIVE_BREAKPOINTS = {
    xs: 0,
    sm: 576,
    md: 768,
    lg: 992,
    xl: 1200,
    xxl: 1400
};