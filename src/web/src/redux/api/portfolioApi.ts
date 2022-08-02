import { subDays, subMonths, subWeeks } from 'date-fns';
import { EntityPerformance, EntityProfit, EntityStatistics, EntityValue, Portfolio } from '../../types';

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
                        'Portfolios',
                        ...result.map(portfolio => ({ type: 'Portfolio' as const, id: portfolio.id }))
                    ]
                    : []
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
            invalidatesTags: (result, error) => 
                !error
                    ? ['Portfolios']
                    : []
        }),
        updatePortfolio: build.mutation<Portfolio, Portfolio>({
            query: (data) => ({
                url: `portfolios/${data.id}`,
                method: 'PUT',
                body: truncateEntityName(truncateEntityNote(data))
            }),
            invalidatesTags: (result, error, arg) => 
                !error
                    ? [
                        { type: 'Portfolio', id: arg.id },
                        { type: 'PortfolioCalculations', id: arg.id }
                    ]
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
                        'Position',
                        'PositionCalculations',
                        'InstrumentTransactions',
                        { type: 'Portfolio', id: arg },
                        { type: 'Positions', id: arg },
                        { type: 'PortfolioTransactions', id: arg },
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
                `portfolios/${id}/value?at=${encodeURIComponent(new Date().toISOString())}`,
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
        getAllPortfoliosStatistics: build.query<Array<EntityStatistics>, void>({
            query: () => 'portfolios/stats',
            providesTags: (result) =>
                result
                    ? ['PortfolioCalculations']
                    : []
        }),
        getPortfolioStatistics: build.query<EntityStatistics, number>({
            query: (id) => `portfolios/${id}/stats`,
            providesTags: (result, error, arg) =>
                result 
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioLastDayProfit: build.query<EntityProfit, number>({
            query: (id) => {
                const to = new Date();
                const from = subDays(to, 1);
                return `portfolios/${id}/profit` +
                    `?from=${encodeURIComponent(from.toISOString())}&to=${encodeURIComponent(to.toISOString())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioLastWeekProfit: build.query<EntityProfit, number>({
            query: (id) => {
                const to = new Date();
                const from = subWeeks(to, 1);
                return `portfolios/${id}/profit` + 
                    `?from=${encodeURIComponent(from.toISOString())}&to=${encodeURIComponent(to.toISOString())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioLastMonthProfit: build.query<EntityProfit, number>({
            query: (id) => {
                const to = new Date();
                const from = subMonths(to, 1);
                return `portfolios/${id}/profit` + 
                    `?from=${encodeURIComponent(from.toISOString())}&to=${encodeURIComponent(to.toISOString())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioTotalProfit: build.query<EntityProfit, number>({
            query: (id) => `portfolios/${id}/profit?to=${encodeURIComponent(new Date().toISOString())}`,
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
                const to = new Date();
                const from = subDays(to, 1);
                return `portfolios/${id}/performance` + 
                    `?from=${encodeURIComponent(from.toISOString())}&to=${encodeURIComponent(to.toISOString())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioLastWeekPerformance: build.query<EntityPerformance, number>({
            query: (id) => {
                const to = new Date();
                const from = subWeeks(to, 1);
                return `portfolios/${id}/performance` + 
                    `?from=${encodeURIComponent(from.toISOString())}&to=${encodeURIComponent(to.toISOString())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioLastMonthPerformance: build.query<EntityPerformance, number>({
            query: (id) => {
                const to = new Date();
                const from = subMonths(to, 1);
                return `portfolios/${id}/performance` + 
                    `?from=${encodeURIComponent(from.toISOString())}&to=${encodeURIComponent(to.toISOString())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PortfolioCalculations', id: arg }]
                    : []
        }),
        getPortfolioTotalPerformance: build.query<EntityPerformance, number>({
            query: (id) => `portfolios/${id}/performance?to=${encodeURIComponent(new Date().toISOString())}`,
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
    useGetPortfolioTotalPerformanceQuery,
    useGetAllPortfoliosStatisticsQuery,
    useGetPortfolioStatisticsQuery
} = portfolioApi;
