import { FetchArgs, FetchBaseQueryError, FetchBaseQueryMeta } from '@reduxjs/toolkit/dist/query';
import { QueryReturnValue } from '@reduxjs/toolkit/dist/query/baseQueryTypes';
import { MaybePromise } from '@reduxjs/toolkit/dist/query/tsHelpers';

import { ChartConfig, PaginatedResponse, ChartLine } from '../../types';
import * as constants from '../../constants';

export function truncateEntityName<T extends { name: string }>(entity: T, length = constants.API_NAME_MAX_LENGTH): T {
    return {
        ...entity,
        name: entity.name.substring(0, length)
    };
}

export function truncateEntityNote<T extends { note: string }>(entity: T, length = constants.API_NOTE_MAX_LENGTH): T {
    return {
        ...entity,
        note: entity.note.substring(0, length)
    };
}

export async function getAllPaginated<T>(
    fetchWithBQ: (arg: string | FetchArgs) =>
        MaybePromise<QueryReturnValue<unknown, FetchBaseQueryError, FetchBaseQueryMeta>>,
    endpointUrl: string,
    params: Record<string, string> = {}
): Promise<QueryReturnValue<Array<T>, FetchBaseQueryError, FetchBaseQueryMeta>> {
    const limit = 300;
    const response: Array<T> = [];
    let currentPage = 1;
    let data: PaginatedResponse<T>;

    do {
        const urlParams = {
            ...params,
            page: currentPage,
            limit
        };

        const page = await fetchWithBQ(buildUrl(endpointUrl, urlParams));
        if (page.error) throw page.error;

        data = page.data as PaginatedResponse<T>;
        response.push(...data.data);

        currentPage++;
    } while (limit * (currentPage - 1) < data.totalCount);

    return {
        data: response
    };
}

export function buildChartLineDataBaseUrl(chart: ChartConfig, line: ChartLine): string {
    switch(chart.type) {
        case 'price':
            switch(line.type) {
                case 'portfolio':
                    return `portfolios/${line.portfolioId}/value/chart`;
                case 'position':
                    return `positions/${line.positionId}/value/chart`
                case 'instrument':
                    return `instruments/${line.instrumentId}/prices/chart`
            }
            break;
        case 'profit':
            switch(line.type) {
                case 'portfolio':
                    return `portfolios/${line.portfolioId}/profit/chart`;
                case 'position':
                    return `positions/${line.positionId}/profit/chart`
                case 'instrument':
                    return `instruments/${line.instrumentId}/profit/chart`
            }
            break;
        case 'performance':
            switch(line.type) {
                case 'portfolio':
                    return `portfolios/${line.portfolioId}/performance/chart`;
                case 'position':
                    return `positions/${line.positionId}/performance/chart`
                case 'instrument':
                    return `instruments/${line.instrumentId}/performance/chart`
            }
            break;
        case 'aggregatedProfit':
            switch(line.type) {
                case 'portfolio':
                    return `portfolios/${line.portfolioId}/profit/chart/aggregated`;
                case 'position':
                    return `positions/${line.positionId}/profit/chart/aggregated`
                case 'instrument':
                    return `instruments/${line.instrumentId}/profit/chart/aggregated`
            }
            break;
        case 'aggregatedPerformance':
            switch(line.type) {
                case 'portfolio':
                    return `portfolios/${line.portfolioId}/performance/chart/aggregated`;
                case 'position':
                    return `positions/${line.positionId}/performance/chart/aggregated`
                case 'instrument':
                    return `instruments/${line.instrumentId}/performance/chart/aggregated`
            }
    }
}

export function buildChartLineTransactionsUrl(line: ChartLine, from: string, to: string): string {
    let entityIdQueryParam = '';
    switch(line.type) {
        case 'portfolio':
            entityIdQueryParam = `portfolioId=${line.portfolioId}`;
            break;
        case 'instrument':
            entityIdQueryParam = `instrumentId=${line.instrumentId}`;
            break;
        case 'position':
            entityIdQueryParam = `positionId=${line.positionId}`;
            break;
        default:
            throw new Error('Unknown chart line type provided.');
            break;
    }

    return `transactions?${entityIdQueryParam}&from=${encodeURIComponent(from)}&to=${encodeURIComponent(to)}`;
}

type ChartLineDataTag = {
    type: 'ChartLinePortfolio' | 'ChartLinePosition' | 'ChartLineInstrument';
    id: string | number
};

type ChartLineTransactionTag = {
    type: 'ChartLinePortfolioTransactions' | 'ChartLinePositionTransactions' | 'ChartLineInstrumentTransactions';
    id: string | number
};

export const generateChartLinesTags = (lines: Array<ChartLine>): Array<ChartLineDataTag> => {
    return lines.map(line => {
            switch(line.type) {
                case 'portfolio':
                    return { type: 'ChartLinePortfolio', id: line.portfolioId };
                case 'position':
                    return { type: 'ChartLinePosition', id: line.positionId };
                case 'instrument':
                    return { type: 'ChartLineInstrument', id: line.instrumentId };
            }
        });
}

export const generateChartTransactionTags = (lines: Array<ChartLine>): Array<ChartLineTransactionTag> => {
    return lines.map(line => {
        switch(line.type) {
            case 'portfolio':
                return { type: 'ChartLinePortfolioTransactions', id: line.portfolioId };
            case 'position':
                return { type: 'ChartLinePositionTransactions', id: line.positionId };
            case 'instrument':
                return { type: 'ChartLineInstrumentTransactions', id: line.instrumentId };
        }
    })
}

function buildUrl(endpointUrl: string, params: Record<string, string | number>) {
    let resultUrl = endpointUrl;
    Object.entries(params).forEach(([param, value], index) => {
        resultUrl += index === 0 ? '?' : '&';
        resultUrl += `${encodeURIComponent(param)}=${encodeURIComponent(value)}`
    });

    return resultUrl;
}