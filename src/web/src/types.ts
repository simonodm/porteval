export type PaginatedResponse<T> = {
    page: number;
    limit: number;
    count: number;
    totalCount: number;
    data: Array<T>
}

export type Portfolio = {
    id: number;
    name: string;
    currencyCode: string;
    note: string;
}

export type Position = {
    id: number;
    portfolioId: number;
    instrumentId: number;
    note: string;
    instrument: Instrument;
}

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

export type InstrumentType = 'stock' | 'bond' | 'mutualFund' | 'etf' |
    'commodity' | 'cryptoCurrency' | 'index' | 'other';

export type Instrument = {
    id: number;
    name: string;
    symbol: string;
    type: InstrumentType;
    exchange: string;
    currencyCode: string;
    note: string;
    isTracked?: boolean;
    lastPriceUpdate?: string;
}

export type InstrumentPriceConfig = {
    instrumentId: number;
    time: string;
    price: number;
}

export type InstrumentPrice = InstrumentPriceConfig & {
    id: number;
}

export type Currency = {
    code: string;
    name: string;
    symbol: string;
    isDefault: boolean;
}

export type CurrencyExchangeRate = {
    id: number;
    currencyFromCode: string;
    currencyToCode: string;
    time: string;
    exchangeRate: number;
}

export type ChartType = 'price' | 'profit' | 'performance' | 'aggregatedProfit' | 'aggregatedPerformance';
export type ChartToDateRange = {
    unit: 'day' | 'week' | 'month' | 'year';
    value: number;
};
export type AggregationFrequency = '5min' | 'hour' | 'day' | 'week' | 'month' | 'year';

type ChartBase = {
    name: string;
    type: ChartType;
    lines: Array<ChartLine>;
}

export type PriceChart = ChartBase & {
    type: 'price';
    currencyCode: string;
}

export type ProfitChart = ChartBase & {
    type: 'profit';
    currencyCode: string;
}

export type PerformanceChart = ChartBase & {
    type: 'performance';
}

export type AggregatedProfitChart = ChartBase & {
    type: 'aggregatedProfit';
    currencyCode: string;
    frequency: AggregationFrequency;
}

export type AggregatedPerformanceChart = ChartBase & {
    type: 'aggregatedPerformance';
    frequency: AggregationFrequency;
}

export type PriceDataChart = PriceChart | ProfitChart | AggregatedProfitChart;
export type PercentageDataChart = PerformanceChart | AggregatedPerformanceChart;
export type AggregatedChart = AggregatedProfitChart | AggregatedPerformanceChart;

export type ChartCategory = PriceDataChart | PercentageDataChart;

export type ToDateChart = ChartCategory & {
    isToDate: true;
    toDateRange: ChartToDateRange;
}

export type DateRangeChart = ChartCategory & {
    isToDate?: false;
    dateRangeStart: string;
    dateRangeEnd: string;
}

type ChartIdentification = {
    id: number;
}

export type ChartConfig = ToDateChart | DateRangeChart;
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

export type ChartLineDashType = 'solid' | 'dashed' | 'dotted';

export type ChartLineBase = {
    name: string;
    width: number;
    dash: ChartLineDashType;
    color: string;
}

export type ChartLinePortfolio = ChartLineBase & {
    portfolioId: number;
    type: 'portfolio'
}

export type ChartLinePosition = ChartLineBase & {
    positionId: number;
    type: 'position'
}

export type ChartLineInstrument = ChartLineBase & {
    instrumentId: number;
    type: 'instrument';
}

export type ChartLine = ChartLinePortfolio | ChartLinePosition | ChartLineInstrument;

export type DashboardPosition = {
    dashboardPositionX: number;
    dashboardPositionY: number;
    dashboardWidth: number;
    dashboardHeight: number;
}

export type DashboardChartItem = {
    chartId: number;
} & DashboardPosition

export type DashboardLayout = {
    items: Array<DashboardChartItem>;
}

export type Exchange = {
    name: string;
}

export type ImportStartedResponse = {
    importId: string;
}

export type TemplateType = 'portfolios' | 'positions' | 'transactions' | 'prices' | 'instruments';
export type ImportStatus = 'finished' | 'error' | 'inProgress' | 'pending';

export type ImportEntry = {
    importId: string;
    templateType: TemplateType;
    status: ImportStatus;
    statusDetails: string;
    errorLogAvailable: boolean;
    errorLogUrl: string;
    time: string;
}

export type EntityValue = {
    value: number;
    time: string;
    currencyCode: string;
}

export type EntityProfit = {
    profit: number;
    currencyCode: string;
}

export type EntityPerformance = {
    performance: number;
    from: string;
    to: string;
}

export type EntityChartDataPoint = {
    time: string;
    value: number;
}

export type Error = {
    errorMessage: string;
}

export type ToDateFinancialDataQueryResponse = {
    lastDay: number;
    lastWeek: number;
    lastMonth: number;
    total: number;
    isLoading: boolean;
}

export type ChartLineDataQueryParams = {
    line: ChartLine;
    from: string;
    to: string;
    frequency: Omit<AggregationFrequency, '5min' | 'hour'>;
}

export type ModalCallbacks = {
    closeModal: () => void
}

export type ChartLineConfigurationContextType = {
    chart?: ChartConfig;
    addInstrumentLine: (instrument: Instrument) => void;
    addPositionLine: (position: Position) => void;
    addPortfolioPositionLines: (positions: Array<Position>) => void;
    addPortfolioLine: (portfolio: Portfolio) => void;
    configureLine: (line: ChartLine) => void;
    removeLine: (line: ChartLine) => void;
}

export type UserSettings = {
    dateFormat: string;
    timeFormat: string;
    decimalSeparator: string;
    thousandsSeparator: string;
}