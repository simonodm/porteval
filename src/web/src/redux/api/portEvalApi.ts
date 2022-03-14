import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const portEvalApi = createApi({
    baseQuery: fetchBaseQuery({ baseUrl: 'http://localhost:4680/api' }),
    endpoints: () => ({}),
    tagTypes: [
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
        'Charts',
        'Chart',
        'DashboardLayout',
        'Exchanges',
        'Currencies'
    ]
});