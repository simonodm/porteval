/**
 * Represents a paginated API response.
 */
export type PaginatedResponse<T> = {
    page: number;
    limit: number;
    count: number;
    totalCount: number;
    data: Array<T>
}

/**
 * Represents an investment portfolio.
 */
export type Portfolio = {
    id: number;
    name: string;
    currencyCode: string;
    note: string;
}

/**
 * Represents an investment portfolio's position.
 */
export type Position = {
    id: number;
    portfolioId: number;
    instrumentId: number;
    positionSize: number;
    note: string;
    instrument: Instrument;
}

/**
 * Represents a single transaction.
 */
export type Transaction = {
    id: number;
    positionId: number;
    portfolioId: number;
    instrument: Instrument;
    time: string;
    amount: number;
    price: number;
    note: string;
}

/**
 * Represents the type of an investment instrument.
 */
export type InstrumentType = 'stock' | 'bond' | 'mutualFund' | 'etf' |
    'commodity' | 'cryptoCurrency' | 'index' | 'other';

/**
 * Represents an investment instrument.
 */
export type Instrument = {
    id: number;
    name: string;
    symbol: string;
    type: InstrumentType;
    exchange: string;
    currencyCode: string;
    currentPrice?: number;
    note: string;
    isTracked?: boolean;
    lastPriceUpdate?: string;
}

/**
 * Represents an instrument's price data.
 */
export type InstrumentPriceConfig = {
    instrumentId: number;
    time: string;
    price: number;
}

/**
 * Represents an instrument's price.
 */
export type InstrumentPrice = InstrumentPriceConfig & {
    id: number;
}

/**
 * Represents a currency.
 */
export type Currency = {
    code: string;
    name: string;
    symbol: string;
    isDefault: boolean;
}

/**
 * Represents an exchange rate between two currencies.
 */
export type CurrencyExchangeRate = {
    id: number;
    currencyFromCode: string;
    currencyToCode: string;
    time: string;
    exchangeRate: number;
}

/**
 * Represents a chart's type.
 */
export type ChartType = 'price' | 'profit' | 'performance' | 'aggregatedProfit' | 'aggregatedPerformance';

/**
 * Represents a chart's to-date range.
 */
export type ChartToDateRange = {
    unit: 'day' | 'week' | 'month' | 'year';
    value: number;
};

/**
 * Represents a chart's data point frequency.
 */
export type AggregationFrequency = '5min' | 'hour' | 'day' | 'week' | 'month' | 'year';

type ChartBase = {
    name: string;
    type: ChartType;
    lines: Array<ChartLine>;
}

/**
 * Represents a price chart.
 */
export type PriceChart = ChartBase & {
    type: 'price';
    currencyCode: string;
}

/**
 * Represents a profit chart.
 */
export type ProfitChart = ChartBase & {
    type: 'profit';
    currencyCode: string;
}

/**
 * Represents a performance chart.
 */
export type PerformanceChart = ChartBase & {
    type: 'performance';
}

/**
 * Represents an aggregated profit chart.
 */
export type AggregatedProfitChart = ChartBase & {
    type: 'aggregatedProfit';
    currencyCode: string;
    frequency: AggregationFrequency;
}

/**
 * Represents an aggregated performance chart.
 */
export type AggregatedPerformanceChart = ChartBase & {
    type: 'aggregatedPerformance';
    frequency: AggregationFrequency;
}

/**
 * Represents a chart consisting of price data.
 */
export type PriceDataChart = PriceChart | ProfitChart | AggregatedProfitChart;

/**
 * Represents a chart consisting of performance/percentage data.
 */
export type PercentageDataChart = PerformanceChart | AggregatedPerformanceChart;

/**
 * Represents a chart displaying aggregated values.
 */
export type AggregatedChart = AggregatedProfitChart | AggregatedPerformanceChart;

/**
 * Represents a chart type category.
 */
export type ChartCategory = PriceDataChart | PercentageDataChart;

/**
 * Represents a to-date chart.
 */
export type ToDateChart = ChartCategory & {
    isToDate: true;
    toDateRange: ChartToDateRange;
}

/**
 * Represents a chart with a fixed date range.
 */
export type DateRangeChart = ChartCategory & {
    isToDate?: false;
    dateRangeStart: string;
    dateRangeEnd: string;
}

type ChartIdentification = {
    id: number;
}

/**
 * Represents a chart configuration.
 */
export type ChartConfig = ToDateChart | DateRangeChart;

/**
 * Represents an identified chart.
 */
export type Chart = ChartConfig & ChartIdentification;

export function isPriceDataChart(chart: ChartCategory): chart is PriceDataChart {
    return chart.type === 'price' || chart.type === 'profit' || chart.type === 'aggregatedProfit';
}
export function isPercentageDataChart(chart: ChartCategory): chart is PercentageDataChart {
    return chart.type === 'performance' || chart.type === 'aggregatedPerformance';
}
export function isAggregatedChart(chart: ChartCategory): chart is AggregatedChart {
    return chart.type === 'aggregatedProfit' || chart.type === 'aggregatedPerformance';
}

/**
 * Represents a chart line's dash type.
 */
export type ChartLineDashType = 'solid' | 'dashed' | 'dotted';

/**
 * Represents a chart line's base configuration.
 */
export type ChartLineBase = {
    name: string;
    width: number;
    dash: ChartLineDashType;
    color: string;
}

/**
 * Represents a portfolio chart line.
 */
export type ChartLinePortfolio = ChartLineBase & {
    portfolioId: number;
    type: 'portfolio'
}

/**
 * Represents a position chart line.
 */
export type ChartLinePosition = ChartLineBase & {
    positionId: number;
    type: 'position'
}

/**
 * Represents an instrument chart line.
 */
export type ChartLineInstrument = ChartLineBase & {
    instrumentId: number;
    type: 'instrument';
}

/**
 * Represents a chart line.
 */
export type ChartLine = ChartLinePortfolio | ChartLinePosition | ChartLineInstrument;

/**
 * Represents dashboard coordinates.
 */
export type DashboardPosition = {
    dashboardPositionX: number;
    dashboardPositionY: number;
    dashboardWidth: number;
    dashboardHeight: number;
}

/**
 * Represents a chart positioned in the dashboard.
 */
export type DashboardChartItem = {
    chartId: number;
} & DashboardPosition

/**
 * Represents a dashboard layout.
 */
export type DashboardLayout = {
    items: Array<DashboardChartItem>;
}

/**
 * Represents a stock exchange.
 */
export type Exchange = {
    symbol: string;
    name?: string;
}

/**
 * Represents a data import initialization response.
 */
export type ImportStartedResponse = {
    importId: string;
}

/**
 * Represents data/import export type.
 */
export type TemplateType = 'portfolios' | 'positions' | 'transactions' | 'prices' | 'instruments';

/**
 * Represents data import status.
 */
export type ImportStatus = 'finished' | 'error' | 'inProgress' | 'pending';

/**
 * Represents a single data import.
 */
export type ImportEntry = {
    importId: string;
    templateType: TemplateType;
    status: ImportStatus;
    statusDetails: string;
    errorLogAvailable: boolean;
    errorLogUrl: string;
    time: string;
}

/**
 * Represents a financial entity's value.
 */
export type EntityValue = {
    value: number;
    time: string;
    currencyCode: string;
}

/**
 * Represents a financial entity's profit.
 */
export type EntityProfit = {
    profit: number;
    currencyCode: string;
}

/**
 * Represents a financial entity's performance.
 */
export type EntityPerformance = {
    performance: number;
    from: string;
    to: string;
}

/**
 * Represents aggregated statistics of a financial entity.
 */
export type EntityStatistics = {
    id: number;
    totalPerformance: number;
    lastDayPerformance: number;
    lastWeekPerformance: number;
    lastMonthPerformance: number;
    totalProfit: number;
    lastDayProfit: number;
    lastWeekProfit: number;
    lastMonthProfit: number;
}

/**
 * Represents aggregated statistics of a position.
 */
export type PositionStatistics = EntityStatistics & {
    breakEvenPoint: number
};

/**
 * Represents a single point in a financial entity's chart data.
 */
export type EntityChartDataPoint = {
    time: string;
    value: number;
}

/**
 * Represents an error.
 */
export type Error = {
    errorMessage: string;
}

/**
 * Represents query parameters for fetching chart line data.
 */
export type ChartLineDataQueryParams = {
    line: ChartLine;
    from: string;
    to: string;
    frequency: Omit<AggregationFrequency, '5min' | 'hour'>;
}

/**
 * Represents callbacks allowed by a modal window.
 */
export type ModalCallbacks = {
    closeModal: () => void
}

/**
 * Represents {@link ChartLineConfigurationContext} data.
 */
export type ChartLineConfigurationContextType = {
    chart?: ChartConfig;
    addInstrumentLine: (instrument: Instrument) => void;
    addPositionLine: (position: Position) => void;
    addPortfolioPositionLines: (positions: Array<Position>) => void;
    addPortfolioLine: (portfolio: Portfolio) => void;
    configureLine: (line: ChartLine) => void;
    removeLine: (line: ChartLine) => void;
}

/**
 * Represents user's configurable settings.
 */
export type UserSettings = {
    dateFormat: string;
    timeFormat: string;
    decimalSeparator: string;
    thousandsSeparator: string;
}