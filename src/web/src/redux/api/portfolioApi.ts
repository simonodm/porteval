import { DateTime } from 'luxon';

import { EntityPerformance, EntityProfit, EntityValue, Portfolio } from '../../types';

import { CreatePortfolioParameters, DateRangeParameters } from './apiTypes';
import { truncateEntityName, truncateEntityNote } from './apiUtils';
import { portEvalApi } from './portEvalApi';

const portfolioApi = portEvalApi.injectEndpoints({
    endpoints: (build) => ({
        getAllPortfolios: build.query<Array<Portfolio>, void>({
            query: () => 'portfolios',
            providesTags: (result) => 
                result
                    ? [
                        ...result.map(({ id }) => ({ type: 'Portfolio' as const, id})),
                         'Portfolios'
                      ]
                    : ['Portfolios']
        }),
        getPortfolioById: build.query<Portfolio, number>({
            query: (id) => `portfolios/${id}`,
            providesTags: (result, error, arg) => 
                result
                    ? [{ type: 'Portfolio', id: arg }]
                    : []
        }),
        createPortfolio: build.mutation<Portfolio, CreatePortfolioParameters>({
            query: (data) => ({
                url: 'portfolios',
                method: 'POST',
                body: truncateEntityName(truncateEntityNote(data))
            }),
            invalidatesTags: () => ['Portfolios']
        }),
        updatePortfolio: build.mutation<Portfolio, Portfolio>({
            query: (data) => ({
                url: `portfolios/${data.id}`,
                method: 'PUT',
                body: truncateEntityName(truncateEntityNote(data))
            }),
            invalidatesTags: (result, error, arg) => 
                !error
                    ? ['Portfolios', { type: 'Portfolio', id: arg.id }]
                    : []
        }),
        deletePortfolio: build.mutation<void, number>({
            query: (id) => ({
                url: `portfolios/${id}`,
                method: 'DELETE'
            }),
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [
                        'Portfolios',
                        'Charts',
                        { type: 'Portfolio', id: arg },
                        { type: 'Positions', id: arg },
                        { type: 'PortfolioCalculations', id: arg },
                      ]
                    : []
        }),
        getPortfolioValue: build.query<EntityValue, { id: number, time: string }>({
            query: ({ id, time }) =>
                `portfolios/${id}/value?at=${encodeURIComponent(time)}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg.id }]
                    : []
        }),
        getPortfolioCurrentValue: build.query<EntityValue, number>({
            query: (id) =>
                `portfolios/${id}/value?at=${encodeURIComponent(DateTime.now().toISO())}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioProfit: build.query<EntityProfit, { id: number } & DateRangeParameters>({
            query: ({ id, from, to }) =>
                `portfolios/${id}/profit` +
                `?${from ? `&from=${encodeURIComponent(from)}` : ''}` +
                `${to ? `&to=${encodeURIComponent(to)}` : ''}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg.id }]
                    : []
            
        }),
        getPortfolioLastDayProfit: build.query<EntityProfit, number>({
            query: (id) => {
                const to = DateTime.now();
                const from = to.minus({ days: 1 });
                return `portfolios/${id}/profit` +
                    `?from=${encodeURIComponent(from.toISO())}&to=${encodeURIComponent(to.toISO())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioLastWeekProfit: build.query<EntityProfit, number>({
            query: (id) => {
                const to = DateTime.now();
                const from = to.minus({ weeks: 1 });
                return `portfolios/${id}/profit` + 
                    `?from=${encodeURIComponent(from.toISO())}&to=${encodeURIComponent(to.toISO())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioLastMonthProfit: build.query<EntityProfit, number>({
            query: (id) => {
                const to = DateTime.now();
                const from = to.minus({ months: 1 });
                return `portfolios/${id}/profit` + 
                    `?from=${encodeURIComponent(from.toISO())}&to=${encodeURIComponent(to.toISO())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioTotalProfit: build.query<EntityProfit, number>({
            query: (id) => `portfolios/${id}/profit?to=${encodeURIComponent(DateTime.now().toISO())}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioPerformance: build.query<EntityPerformance, { id: number } & DateRangeParameters>({
            query: ({ id, from, to }) =>
                `portfolios/${id}/performance?` +
                `${from ? `&from=${encodeURIComponent(from)}` : ''}` +
                `${to ? `&to=${encodeURIComponent(to)}` : ''}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg.id }]
                    : []
        }),
        getPortfolioLastDayPerformance: build.query<EntityPerformance, number>({
            query: (id) => {
                const to = DateTime.now();
                const from = to.minus({ days: 1 });
                return `portfolios/${id}/performance` + 
                    `?from=${encodeURIComponent(from.toISO())}&to=${encodeURIComponent(to.toISO())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioLastWeekPerformance: build.query<EntityPerformance, number>({
            query: (id) => {
                const to = DateTime.now();
                const from = to.minus({ weeks: 1 });
                return `portfolios/${id}/performance` + 
                    `?from=${encodeURIComponent(from.toISO())}&to=${encodeURIComponent(to.toISO())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioLastMonthPerformance: build.query<EntityPerformance, number>({
            query: (id) => {
                const to = DateTime.now();
                const from = to.minus({ months: 1 });
                return `portfolios/${id}/performance` + 
                    `?from=${encodeURIComponent(from.toISO())}&to=${encodeURIComponent(to.toISO())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioTotalPerformance: build.query<EntityPerformance, number>({
            query: (id) => `portfolios/${id}/performance?to=${encodeURIComponent(DateTime.now().toISO())}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        })
    })
});

export const { 
    useGetAllPortfoliosQuery,
    useGetPortfolioByIdQuery,
    useCreatePortfolioMutation,
    useUpdatePortfolioMutation,
    useDeletePortfolioMutation,
    useGetPortfolioValueQuery,
    useGetPortfolioCurrentValueQuery,
    useGetPortfolioProfitQuery,
    useGetPortfolioLastDayProfitQuery,
    useGetPortfolioLastWeekProfitQuery,
    useGetPortfolioLastMonthProfitQuery,
    useGetPortfolioTotalProfitQuery,
    useGetPortfolioPerformanceQuery,
    useGetPortfolioLastDayPerformanceQuery,
    useGetPortfolioLastWeekPerformanceQuery,
    useGetPortfolioLastMonthPerformanceQuery,
    useGetPortfolioTotalPerformanceQuery
} = portfolioApi;
