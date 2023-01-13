import { Currency, Portfolio, EntityStatistics, Instrument, Position,
    PositionStatistics, Transaction, InstrumentPrice, Chart, CurrencyExchangeRate,
    ImportEntry, DashboardLayout, InstrumentSplit, Exchange
} from '../../types';

export const testCurrencies: Array<Currency> = [
    {
        name: 'US Dollar',
        code: 'USD',
        symbol: 'US$',
        isDefault: true
    },
    {
        name: 'European Euro',
        code: 'EUR',
        symbol: '€',
        isDefault: false
    }
];

export const testPortfolios: Array<Portfolio> = [
    {
        id: 1,
        name: 'Test portfolio 1',
        currencyCode: 'USD',
        note: 'Test note 1'
    },
    {
        id: 2,
        name: 'Test portfolio 2',
        currencyCode: 'EUR',
        note: 'Test note 2'
    }
];

export const testPortfoliosStats: Array<EntityStatistics> = [
    {
        id: testPortfolios[0].id,
        totalPerformance: 0.1,
        lastDayPerformance: 0.01,
        lastWeekPerformance: -0.02,
        lastMonthPerformance: 0.04,
        totalProfit: 100,
        lastDayProfit: 10,
        lastWeekProfit: -20,
        lastMonthProfit: 40
    },
    {
        id: testPortfolios[1].id,
        totalPerformance: -0.1,
        lastDayPerformance: -0.01,
        lastWeekPerformance: 0.02,
        lastMonthPerformance: -0.04,
        totalProfit: -100,
        lastDayProfit: -10,
        lastWeekProfit: 20,
        lastMonthProfit: -40
    }
];

export const testInstruments: Array<Instrument> = [
    {
        id: 1,
        name: 'Apple Inc.',
        symbol: 'AAPL',
        currencyCode: 'USD',
        type: 'stock',
        exchange: 'NASDAQ',
        note: 'Test note AAPL',
        currentPrice: 100
    },
    {
        id: 2,
        name: 'Bitcoin USD',
        symbol: 'BTC',
        currencyCode: 'USD',
        type: 'cryptoCurrency',
        note: 'Test note BTC',
        currentPrice: 80
    }
];

export const testPositions: Array<Position> = [
    {
        id: 1,
        portfolioId: testPortfolios[0].id,
        instrumentId: testInstruments[0].id,
        instrument: testInstruments[0],
        positionSize: 1,
        note: 'Test note AAPL position'
    },
    {
        id: 2,
        portfolioId: testPortfolios[1].id,
        instrumentId: testInstruments[1].id,
        instrument: testInstruments[1],
        positionSize: 0.5,
        note: 'Test note BTC position'
    },
];

export const testPositionStats: Array<PositionStatistics> = [
    {
        id: testPositions[0].id,
        totalPerformance: 0.1,
        lastDayPerformance: 0.01,
        lastWeekPerformance: -0.02,
        lastMonthPerformance: 0.04,
        totalProfit: 100,
        lastDayProfit: 10,
        lastWeekProfit: -20,
        lastMonthProfit: 40,
        breakEvenPoint: 100
    },
    {
        id: testPositions[1].id,
        totalPerformance: -0.1,
        lastDayPerformance: -0.01,
        lastWeekPerformance: 0.02,
        lastMonthPerformance: -0.04,
        totalProfit: -100,
        lastDayProfit: -10,
        lastWeekProfit: 20,
        lastMonthProfit: -40,
        breakEvenPoint: 100
    }
];

export const testTransactions: Array<Transaction> = [
    {
        id: 1,
        portfolioId: testPortfolios[0].id,
        positionId: testPositions[0].id,
        amount: 10,
        price: 50,
        time: '2022-01-01T00:00:00Z',
        instrument: testInstruments[0],
        note: 'Test transaction 1 note'
    },
    {
        id: 2,
        portfolioId: testPortfolios[1].id,
        positionId: testPositions[1].id,
        amount: 10,
        price: 50,
        time: '2022-01-01T00:00:00Z',
        instrument: testInstruments[1],
        note: 'Test transaction 2 note'
    }
];

// sorted by time descending, similar to the way API returns data
export const testPrices: Array<InstrumentPrice> = [
    {
        id: 2,
        instrumentId: testInstruments[0].id,
        time: '2022-01-01T00:00:00Z',
        price: 100
    },
    {
        id: 4,
        instrumentId: testInstruments[1].id,
        time: '2022-01-01T00:00:00Z',
        price: 80
    },
    {
        id: 1,
        instrumentId: testInstruments[0].id,
        time: '2010-01-01T00:00:00Z',
        price: 80
    },
    {
        id: 3,
        instrumentId: testInstruments[1].id,
        time: '2010-01-01T00:00:00Z',
        price: 100
    }    
];

export const testCharts: Array<Chart> = [
    {
        id: 1,
        name: 'Test chart 1',
        type: 'price',
        isToDate: true,
        toDateRange: {
            unit: 'month',
            value: 6
        },
        currencyCode: 'USD',
        lines: []
    }
];

export const testExchangeRates: Array<CurrencyExchangeRate> = [
    {
        id: 1,
        currencyFromCode: 'USD',
        currencyToCode: 'EUR',
        exchangeRate: 1,
        time: '2022-01-01'
    }
];

export const testDataImports: Array<ImportEntry> = [
    {
        importId: 'test-import',
        time: '2022-01-01T00:00:00Z',
        status: 'finished',
        statusDetails: '',
        templateType: 'instruments',
        errorLogAvailable: false,
        errorLogUrl: ''
    },
    {
        importId: 'test-import-2',
        time: '2022-06-01T00:00:00Z',
        status: 'finished',
        statusDetails: 'test error',
        templateType: 'portfolios',
        errorLogAvailable: true,
        errorLogUrl: 'http://localhost/api/imports/test-import-2/errorLog.'
    }
];

export const testDashboardLayout: DashboardLayout = {
    items: [
        {
            chartId: testCharts[0].id,
            dashboardPositionX: 0,
            dashboardPositionY: 0,
            dashboardHeight: 1,
            dashboardWidth: 1
        }
    ]
}

export const testInstrumentSplits: Array<InstrumentSplit> = [
    {
        id: 1,
        instrumentId: testInstruments[0].id,
        splitRatioNumerator: 2,
        splitRatioDenominator: 1,
        time: '2022-01-01T00:00:00Z',
        status: 'processed'
    }
];

export const testExchanges: Array<Exchange> = [
    {
        symbol: 'NASDAQ',
        name: 'NASDAQ'
    },
    {
        symbol: 'NYSE',
        name: 'NYSE'
    }
];

export type TestState = {
    portfolios: Array<Portfolio>;
    portfolioStatistics: Array<EntityStatistics>;
    positions: Array<Position>;
    positionStatistics: Array<PositionStatistics>;
    transactions: Array<Transaction>;
    instruments: Array<Instrument>;
    instrumentPrices: Array<InstrumentPrice>;
    instrumentSplits: Array<InstrumentSplit>;
    exchanges: Array<Exchange>;
    currencies: Array<Currency>;
    exchangeRates: Array<CurrencyExchangeRate>;
    dataImports: Array<ImportEntry>;
    dashboardLayout: DashboardLayout;
    charts: Array<Chart>;
};

export const DEFAULT_TEST_STATE: TestState = Object.freeze({
    portfolios: testPortfolios,
    portfolioStatistics: testPortfoliosStats,
    positions: testPositions,
    positionStatistics: testPositionStats,
    transactions: testTransactions,
    instruments: testInstruments,
    instrumentPrices: testPrices,
    instrumentSplits: testInstrumentSplits,
    exchanges: testExchanges,
    currencies: testCurrencies,
    exchangeRates: testExchangeRates,
    dataImports: testDataImports,
    dashboardLayout: testDashboardLayout,
    charts: testCharts
});