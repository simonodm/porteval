import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const portEvalApi = createApi({
    baseQuery: fetchBaseQuery({ baseUrl: 'https://localhost:4681/api' }),
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
        'Chart',
        'DashboardLayout'
    ]
});