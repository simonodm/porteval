import { FetchArgs, FetchBaseQueryError, FetchBaseQueryMeta } from '@reduxjs/toolkit/dist/query';
import { QueryReturnValue } from '@reduxjs/toolkit/dist/query/baseQueryTypes';
import { MaybePromise } from '@reduxjs/toolkit/dist/query/tsHelpers';
import { ChartConfig, PaginatedResponse, ChartLine } from '../../types';
import * as constants from '../../constants';

/**
 * Truncates an object's `name` property to API name length limit.
 * 
 * @category API
 * @subcategory Utilities
 * @param entity Object, the name of which is to be truncated.
 * @param length Length to truncate name to.
 * @returns The original object with truncated name.
 */
function truncateEntityName<T extends { name: string }>(entity: T, length = constants.API_NAME_MAX_LENGTH): T {
    return {
        ...entity,
        name: entity.name.substring(0, length)
    };
}

/**
 * Truncates an object's `note` property to API note length limit.
 * 
 * @category API
 * @subcategory Utilities
 * @param entity Object, the note of which is to be truncated.
 * @param length Length to truncate note to.
 * @returns The original object with truncated note.
 */
function truncateEntityNote<T extends { note: string }>(entity: T, length = constants.API_NOTE_MAX_LENGTH): T {
    return {
        ...entity,
        note: entity.note.substring(0, length)
    };
}

/**
 * Fetches all data from a paginated endpoint.
 * 
 * @category API
 * @subcategory Utilities
 * @param fetchWithBQ RTK base query function.
 * @param endpointUrl Endpoint URL.
 * @param params Endpoint query parameters.
 * @returns An array of all data available from the specified paginated endpoint.
 */
async function getAllPaginated<T>(
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

/**
 * Builds a URL for the chart's line data endpoint based on chart and line configuration.
 * 
 * @category API
 * @subcategory Utilities
 * @param chart Chart to get data for.
 * @param line Line to get data for.
 * @returns URL of the endpoint to call to get the data for the specified chart and line.
 */
function buildChartLineDataBaseUrl(chart: ChartConfig, line: ChartLine): string {
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

/**
 * Builds a URL for the chart line's transactions endpoint.
 * 
 * @category API
 * @subcategory Utilities
 * @param line Chart line to get transactions for.
 * @param from Date to get transactions from.
 * @param to Date to get transactions until.
 * @returns URL of the endpoint to call to get all transactions for the specified chart line.
 */
function buildChartLineTransactionsUrl(line: ChartLine, from: string, to: string): string {
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

/**
 * An RTK query tag for a fetched chart line.
 * @category API
 * @subcategory Utilities
 */
type ChartLineEntityTag = {
    type: 'Portfolio' | 'Position' | 'Instrument',
    id: string | number
};

/**
 * An RTK query tag for fetched chart line data.
 * @category API
 * @subcategory Utilities
 */
type ChartLineDataTag = {
    type: 'PortfolioCalculations' | 'PositionCalculations' | 'InstrumentCalculations';
    id: string | number
};

/**
 * An RTK query tag for fetched chart line transactions.
 * @category API
 * @subcategory Utilities
 */
type ChartLineTransactionTag = {
    type: 'PortfolioTransactions' | 'PositionTransactions' | 'InstrumentTransactions';
    id: string | number
};

/**
 * Generates RTK query tags for fetched chart lines.
 * 
 * @category API
 * @subcategory Utilities
 * @param lines Fetched chart lines.
 * @returns An array of RTK query tags.
 */
const generateChartLineReferencedEntityTags = (lines: Array<ChartLine>): Array<ChartLineEntityTag> => {
    return lines.map(line => {
        switch(line.type) {
            case 'portfolio':
                return { type: 'Portfolio', id: line.portfolioId };
            case 'position':
                return { type: 'Position', id: line.positionId };
            case 'instrument':
                return { type: 'Instrument', id: line.instrumentId };
        }
    })
}

/**
 * Generates RTK query tags for fetched chart lines' data.
 * 
 * @category API
 * @subcategory Utilities
 * @param lines Fetched chart lines.
 * @returns An array of RTK query tags.
 */
const generateChartLineCalcTags = (lines: Array<ChartLine>): Array<ChartLineDataTag> => {
    return lines.map(line => {
            switch(line.type) {
                case 'portfolio':
                    return { type: 'PortfolioCalculations', id: line.portfolioId };
                case 'position':
                    return { type: 'PositionCalculations', id: line.positionId };
                case 'instrument':
                    return { type: 'InstrumentCalculations', id: line.instrumentId };
            }
        });
}

/**
 * Generates RTK query tags for fetched chart lines' transactions.
 * 
 * @category API
 * @subcategory Utilities
 * @param lines Fetched chart lines.
 * @returns An array of RTK query tags.
 */
const generateChartTransactionTags = (lines: Array<ChartLine>): Array<ChartLineTransactionTag> => {
    return lines.map(line => {
        switch(line.type) {
            case 'portfolio':
                return { type: 'PortfolioTransactions', id: line.portfolioId };
            case 'position':
                return { type: 'PositionTransactions', id: line.positionId };
            case 'instrument':
                return { type: 'InstrumentTransactions', id: line.instrumentId };
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

export {
    truncateEntityName,
    truncateEntityNote,
    getAllPaginated,
    buildChartLineDataBaseUrl,
    buildChartLineTransactionsUrl,
    generateChartLineCalcTags,
    generateChartLineReferencedEntityTags,
    generateChartTransactionTags
}
export type { ChartLineEntityTag }
export type { ChartLineDataTag }
export type { ChartLineTransactionTag }