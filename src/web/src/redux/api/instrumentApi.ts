import { portEvalApi } from './portEvalApi';
import { Instrument, InstrumentPrice, PaginatedResponse, EntityProfit, EntityPerformance, InstrumentPriceConfig } from '../../types';
import { CreateInstrumentParameters, DateRangeParameters, PaginationParameters } from './apiTypes';
import { getAllPaginated, truncateEntityName, truncateEntityNote } from './apiUtils';

const instrumentApi = portEvalApi.injectEndpoints({
    endpoints: (build) => ({
        getAllInstruments: build.query<Array<Instrument>, void>({
            async queryFn(_arg, _queryApi, _extraOptions, fetchWithBQ) {
                return await getAllPaginated<Instrument>(fetchWithBQ, 'instruments');
            },
            providesTags: (result) =>
                result
                    ? [
                        ...result.map(({ id }) => ({ type: 'Instrument' as const, id })),
                        'Instruments'
                      ]
                    : []
        }),
        getInstrumentPage: build.query<PaginatedResponse<Instrument>, PaginationParameters>({
            query: ({ page = 1, limit = 100 }) => `instruments?page=${page}&limit=${limit}`,
            providesTags: (result) =>
                result
                    ? [
                        ...result.data.map(({ id }) => ({ type: 'Instrument' as const, id })),
                        'Instruments'
                    ]
                    : []
        }),
        getInstrumentById: build.query<Instrument, number>({
            query: (id) => `instruments/${id}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'Instrument', id: arg }]
                    : []
        }),
        createInstrument: build.mutation<Instrument, CreateInstrumentParameters>({
            query: (data) => ({
                url: 'instruments',
                method: 'POST',
                body: truncateEntityName(truncateEntityNote(data))
            }),
            invalidatesTags: (result, error) =>
                !error
                    ? ['Instruments']
                    : []
        }),
        updateInstrument: build.mutation<Instrument, Instrument>({
            query: (data) => ({
                url: `instruments/${data.id}`,
                method: 'PUT',
                body: truncateEntityName(truncateEntityNote(data))
            }),
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [{ type: 'Instrument', id: arg.id }, 'Instruments']
                    : []
        }),
        deleteInstrument: build.mutation<void, number>({
            query: (id) => ({
                url: `instruments/${id}`,
                method: 'DELETE'
            }),
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [
                        'Instruments',
                        'PortfolioCalculations',
                        'PositionCalculations',
                        'Charts',
                        { type: 'Instrument', id: arg },
                        { type: 'InstrumentCalculations', id: arg }
                    ]
                    : []
        }),
        getAllInstrumentPrices: build.query<Array<InstrumentPrice>, { instrumentId: number } & DateRangeParameters>({
            async queryFn(arg, _queryApi, _extraOptions, fetchWithBQ) {
                return await getAllPaginated<InstrumentPrice>(
                    fetchWithBQ,
                    `instruments/${arg.instrumentId}/prices`,
                    {
                        ...(arg.from && {from: arg.from}),
                        ...(arg.to && {to: arg.to})
                    });
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'InstrumentPrices', id: arg.instrumentId }]
                    : []
        }),
        getInstrumentPricePage: build.query<PaginatedResponse<InstrumentPrice>, { instrumentId: number } & PaginationParameters & DateRangeParameters>({
            query: ({ instrumentId, from, to, page = 1, limit = 100 }) => ({
                url: `instruments/${instrumentId}/prices` +
                    `?page=${page}` +
                    `&limit=${limit}` +
                    `${from ? `&from=${encodeURIComponent(from)}` : ''}` +
                    `${to ? `&to=${encodeURIComponent(to)}` : ''}`
            }),
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'InstrumentPrices', id: arg.instrumentId }]
                    : []
        }),
        getInstrumentPriceAt: build.query<InstrumentPrice, { instrumentId: number, time: string }>({
            query: ({ instrumentId, time }) => ({
                url: `instruments/${instrumentId}/prices/at?time=${encodeURIComponent(time)}`
            }),
            providesTags: (result) =>
                result
                    ? [{ type: 'InstrumentPrices', id: result.id }]
                    : []
        }),
        getInstrumentCurrentPrice: build.query<InstrumentPrice, number>({
            query: (instrumentId) => ({
                url: `instruments/${instrumentId}/prices/latest`
            }),
            providesTags: (result) =>
                result
                    ? [{ type: 'InstrumentPrices', id: result.id }]
                    : []
        }),
        addInstrumentPrice: build.mutation<InstrumentPrice, InstrumentPriceConfig>({
            query: (data) => ({
                url: `instruments/${data.instrumentId}/prices`,
                method: 'POST',
                body: data
            }),
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [{type: 'InstrumentPrices', id: arg.instrumentId}, 'PortfolioCalculations', 'PositionCalculations']
                    : []
        }),
        deleteInstrumentPrice: build.mutation<void, { instrumentId: number, priceId: number }>({
            query: ({ instrumentId, priceId }) => ({
                url: `instruments/${instrumentId}/prices/${priceId}`,
                method: 'DELETE'
            }),
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [
                        'PortfolioCalculations', 'PositionCalculations',
                        { type: 'InstrumentCalculations', id: arg.instrumentId },
                        { type: 'InstrumentPrices', id: arg.instrumentId }
                    ]
                    : []
        }),
        getInstrumentProfit: build.query<EntityProfit, { instrumentId: number } & DateRangeParameters>({
            query: ({ instrumentId, from, to }) =>
                `instruments/${instrumentId}/profit` +
                `${from ? `from=${encodeURIComponent(from)}` : ''}` +
                `${to ? `&to=${encodeURIComponent(to)}` : ''}`,
            providesTags: (result, error, arg) =>
                !error
                    ? [{ type: 'InstrumentCalculations', id: arg.instrumentId }]
                    : []
        }),
        getInstrumentPerformance: build.query<EntityPerformance, { instrumentId: number } & DateRangeParameters>({
            query: ({ instrumentId, from, to }) =>
                `instruments/${instrumentId}/performance` +
                `${from ? `from=${encodeURIComponent(from)}` : ''}` +
                `${to ? `&to=${encodeURIComponent(to)}` : ''}`,
            providesTags: (result, error, arg) =>
                !error
                    ? [{ type: 'InstrumentCalculations', id: arg.instrumentId }]
                    : []
        })
    })
});

export const {
    useGetAllInstrumentsQuery,
    useGetInstrumentPageQuery,
    useGetInstrumentByIdQuery,
    useCreateInstrumentMutation,
    useUpdateInstrumentMutation,
    useDeleteInstrumentMutation,
    useGetAllInstrumentPricesQuery,
    useGetInstrumentPricePageQuery,
    useGetInstrumentPriceAtQuery,
    useGetInstrumentCurrentPriceQuery,
    useAddInstrumentPriceMutation,
    useDeleteInstrumentPriceMutation,
    useGetInstrumentProfitQuery,
    useGetInstrumentPerformanceQuery,
    usePrefetch
} = instrumentApi;
