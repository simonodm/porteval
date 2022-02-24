import { DateTime } from 'luxon';

import { EntityPerformance, EntityProfit, EntityValue, Position } from '../../types';

import { CreatePositionParameters, DateRangeParameters } from './apiTypes';
import { portEvalApi } from './portEvalApi';
import { truncateEntityNote } from './apiUtils';

const positionApi = portEvalApi.injectEndpoints({
    endpoints: (build) => ({
        getPositions: build.query<Array<Position>, number>({
            query: (portfolioId) => `portfolios/${portfolioId}/positions`,
            providesTags: (result, error, arg) => 
                result
                    ? [
                        ...result.map(({ id }) => ({ type: 'Position' as const, id})),
                        { type: 'Positions', id: arg }
                      ]
                    : [{ type: 'Positions', id: arg }]
        }),
        getPosition: build.query<Position, { positionId: number }>({
            query: ({ positionId }) => `positions/${positionId}`,
            providesTags: (result, error, arg) => 
                result
                    ? [{ type: 'Position', id: arg.positionId }]
                    : []
        }),
        addPosition: build.mutation<Position, CreatePositionParameters>({
            query: (data) => ({
                url: 'positions',
                method: 'POST',
                body: truncateEntityNote(data)
            }),
            invalidatesTags: (result, error, arg) =>
                result 
                    ? [{ type: 'Positions', id: arg.portfolioId }]
                    : []
        }),
        updatePosition: build.mutation<Position, Position>({
            query: (data) => ({
                url: `positions/${data.id}`,
                method: 'PUT',
                body: truncateEntityNote(data)
            }),
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [{ type: 'Position', id: arg.id }]
                    : []
        }),
        deletePosition: build.mutation<void, Position>({
            query: ({ id }) => ({
                url: `positions/${id}`,
                method: 'DELETE'
            }),
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [
                        { type: 'Positions', id: arg.portfolioId },
                        { type: 'Position', id: arg.id },
                        { type: 'Transactions', id: arg.id },
                        { type: 'PortfolioCalculations', id: arg.portfolioId },
                        { type: 'PositionCalculations', id: arg.id },
                        'Charts'
                      ]
                    : []
        }),
        getPositionValue: build.query<EntityValue, { positionId: number } & DateRangeParameters>({
            query: ({ positionId, from, to }) =>
                `positions/${positionId}/value` + 
                    `?from=${encodeURIComponent(from ?? '')}&to=${encodeURIComponent(to ?? '')}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PositionCalculations', id: arg.positionId }]
                    : []
        }),
        getPositionCurrentValue: build.query<EntityValue, { positionId: number }>({
            query: ({ positionId }) =>
                `positions/${positionId}/value` + 
                    `?at=${encodeURIComponent(DateTime.now().toISO())}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PositionCalculations', id: arg.positionId }]
                    : []
        }),
        getPositionProfit: build.query<EntityProfit, { positionId: number } & DateRangeParameters>({
            query: ({ positionId, from, to }) =>
                `positions/${positionId}/profit` + 
                    `?from=${encodeURIComponent(from ?? '')}&to=${encodeURIComponent(to ?? '')}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PositionCalculations', id: arg.positionId }]
                    : []
        }),
        getPositionLastDayProfit: build.query<EntityProfit, { positionId: number }>({
            query: ({ positionId }) => {
                const to = DateTime.now();
                const from = to.minus({ days: 1 });
                return `positions/${positionId}/profit` + 
                    `?from=${encodeURIComponent(from.toISO())}&to=${encodeURIComponent(to.toISO())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PositionCalculations', id: arg.positionId }]
                    : []
        }),
        getPositionLastWeekProfit: build.query<EntityProfit, { positionId: number }>({
            query: ({ positionId }) => {
                const to = DateTime.now();
                const from = to.minus({ weeks: 1 });
                return `positions/${positionId}/profit` + 
                    `?from=${encodeURIComponent(from.toISO())}&to=${encodeURIComponent(to.toISO())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PositionCalculations', id: arg.positionId }]
                    : []
        }),
        getPositionLastMonthProfit: build.query<EntityProfit, { positionId: number }>({
            query: ({ positionId }) => {
                const to = DateTime.now();
                const from = to.minus({ months: 1 });
                return `positions/${positionId}/profit` + 
                    `?from=${encodeURIComponent(from.toISO())}&to=${encodeURIComponent(to.toISO())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PositionCalculations', id: arg.positionId }]
                    : []
        }),
        getPositionTotalProfit: build.query<EntityProfit, { positionId: number }>({
            query: ({ positionId }) =>
                `positions/${positionId}/profit?to=${encodeURIComponent(DateTime.now().toISO())}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PositionCalculations', id: arg.positionId }]
                    : []
        }),
        getPositionPerformance: build.query<EntityPerformance, { positionId: number } & DateRangeParameters>({
            query: ({ positionId, from, to }) =>
                `positions/${positionId}/performance` +
                `${from ? `&from=${encodeURIComponent(from)}` : ''}` +
                `${to ? `&to=${encodeURIComponent(to)}` : ''}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PositionCalculations', id: arg.positionId }]
                    : []
        }),
        getPositionLastDayPerformance: build.query<EntityPerformance, { positionId: number }>({
            query: ({ positionId }) => {
                const to = DateTime.now();
                const from = to.minus({ days: 1 });
                return `positions/${positionId}/performance` + 
                    `?from=${encodeURIComponent(from.toISO())}&to=${encodeURIComponent(to.toISO())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PositionCalculations', id: arg.positionId }]
                    : []
        }),
        getPositionLastWeekPerformance: build.query<EntityPerformance, { positionId: number }>({
            query: ({ positionId }) => {
                const to = DateTime.now();
                const from = to.minus({ weeks: 1 });
                return `positions/${positionId}/performance` + 
                    `?from=${encodeURIComponent(from.toISO())}&to=${encodeURIComponent(to.toISO())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PositionCalculations', id: arg.positionId }]
                    : []
        }),
        getPositionLastMonthPerformance: build.query<EntityPerformance, { positionId: number }>({
            query: ({ positionId }) => {
                const to = DateTime.now();
                const from = to.minus({ months: 1 });
                return `positions/${positionId}/performance` + 
                    `?from=${encodeURIComponent(from.toISO())}&to=${encodeURIComponent(to.toISO())}`
            },
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PositionCalculations', id: arg.positionId }]
                    : []
        }),
        getPositionTotalPerformance: build.query<EntityPerformance, { positionId: number }>({
            query: ({ positionId }) =>
                `positions/${positionId}/performance?to=${encodeURIComponent(DateTime.now().toISO())}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'PositionCalculations', id: arg.positionId }]
                    : []
        })
    })
});

export const {
    useGetPositionsQuery,
    useGetPositionQuery,
    useAddPositionMutation,
    useUpdatePositionMutation,
    useDeletePositionMutation,
    useGetPositionValueQuery,
    useGetPositionCurrentValueQuery,
    useGetPositionProfitQuery,
    useGetPositionLastDayProfitQuery,
    useGetPositionLastWeekProfitQuery,
    useGetPositionLastMonthProfitQuery,
    useGetPositionTotalProfitQuery,
    useGetPositionPerformanceQuery,
    useGetPositionLastDayPerformanceQuery,
    useGetPositionLastWeekPerformanceQuery,
    useGetPositionLastMonthPerformanceQuery,
    useGetPositionTotalPerformanceQuery
} = positionApi