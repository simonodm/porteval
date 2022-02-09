import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const portEvalApi = createApi({
    baseQuery: fetchBaseQuery({ baseUrl: 'http://localhost:4680/api' }),
    endpoints: () => ({}),
    tagTypes: [
        'Portfolios',
        'Portfolio',
        'PortfolioCalculations',
        'Positions',
        'Position',
        'PositionInstrument',
        'PositionCalculations',
        'Transactions',
        'Transaction',
        'Instruments',
        'Instrument',
        'InstrumentCalculations',
        'InstrumentPrices',
        'Charts',
        'ChartLineInstrument',
        'ChartLinePortfolio',
        'ChartLinePosition',
        'ChartLineInstrumentTransactions',
        'ChartLinePortfolioTransactions',
        'ChartLinePositionTransactions',
        'Chart',
        'DashboardLayout'
    ]
});