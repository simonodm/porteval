import { Currency, EntityStatistics, Instrument, InstrumentPrice, Portfolio, Position, PositionStatistics, Transaction } from "../../types";

export const testCurrencies: Array<Currency> = [
    {
        name: "US Dollar",
        code: "USD",
        symbol: "US$",
        isDefault: true
    },
    {
        name: "European Euro",
        code: "EUR",
        symbol: "€",
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

export const testPortfolioStats: Array<EntityStatistics> = [
    {
        id: 1,
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
        id: 2,
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
        name: "Apple Inc.",
        symbol: "AAPL",
        currencyCode: "USD",
        type: "stock",
        exchange: "NASDAQ",
        note: "Test note AAPL"
    },
    {
        id: 2,
        name: "Bitcoin USD",
        symbol: "BTC",
        currencyCode: "USD",
        type: "cryptoCurrency",
        note: "Test note BTC"
    }
];

export const testPositions: Array<Position> = [
    {
        id: 1,
        portfolioId: testPortfolios[0].id,
        instrumentId: testInstruments[0].id,
        instrument: testInstruments[0],
        positionSize: 1,
        note: "Test note AAPL position"
    },
    {
        id: 2,
        portfolioId: testPortfolios[1].id,
        instrumentId: testInstruments[1].id,
        instrument: testInstruments[1],
        positionSize: 0.5,
        note: "Test note BTC position"
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
        time: "2022-01-01T00:00:00Z",
        instrument: testInstruments[0],
        note: "Test transaction 1 note"
    },
    {
        id: 2,
        portfolioId: testPortfolios[1].id,
        positionId: testPositions[1].id,
        amount: 10,
        price: 50,
        time: "2022-01-01T00:00:00Z",
        instrument: testInstruments[1],
        note: "Test transaction 2 note"
    }
];

export const testPrices: Array<InstrumentPrice> = [
    {
        id: 1,
        instrumentId: testInstruments[0].id,
        time: "2010-01-01T00:00:00Z",
        price: 80
    },
    {
        id: 2,
        instrumentId: testInstruments[0].id,
        time: "2022-01-01T00:00:00Z",
        price: 100
    },
    {
        id: 3,
        instrumentId: testInstruments[1].id,
        time: "2010-01-01T00:00:00Z",
        price: 100
    },
    {
        id: 4,
        instrumentId: testInstruments[1].id,
        time: "2010-01-01T00:00:00Z",
        price: 80
    },
];