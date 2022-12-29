import { SerializedError } from '@reduxjs/toolkit';
import { FetchBaseQueryError, TagDescription } from '@reduxjs/toolkit/dist/query';
import { portEvalApi } from '../redux/api/portEvalApi';

type Query = {
    isFetching?: boolean;
    isLoading?: boolean;
    error?: undefined | unknown;
}

type SuccessfulResponse<T> = {
    data: T;
}

type ErrorResponse = {
    error: FetchBaseQueryError | SerializedError;
}

type PromiseReturnType<T> = SuccessfulResponse<T> | ErrorResponse;

/**
 * Checks whether all of the provided queries have loaded.
 * 
 * @category Utilities
 * @subcategory Queries
 * @param args RTK queries to check.
 * @returns `true` if all of the queries have loaded, `false` otherwise.
 */
export function checkIsLoaded(...args: Array<Query>): boolean {
    return !args.map(arg => arg.isFetching || arg.isLoading).reduce((prev, curr) => prev || curr);
}

/**
 * Checks whether any of the provided queries have returned an error.
 * 
 * @category Utilities
 * @subcategory Queries
 * @param args RTK queries to check.
 * @returns `true` if any of the queries returned an error, `false` otherwise.
 */
export function checkIsError(...args: Array<Query>): boolean {
    return args.map(arg => !!arg.error).reduce((prev, curr) => prev || curr);
}

/**
 * Calls the provided callback if the provided RTK query response was successful. 
 * 
 * @category Utilities
 * @subcategory Queries
 * @param val RTK query promise.
 * @param callback Callback to call.
 */
export function onSuccessfulResponse<T>(val: PromiseReturnType<T>, callback?: (data: T) => void): void {
    if(!Object.hasOwnProperty.call(val, 'error')) {
        callback && callback((<SuccessfulResponse<T>>val).data);
    }
}

/**
 * Invalidates RTK Query tags related to price data, including relevant calculations. 
 * 
 * @returns A PayloadAction.
 */
export function invalidateFinancialData(): { payload: TagDescription<string>[], type: string } {
    return portEvalApi.util.invalidateTags([
        'PortfolioCalculations',
        'PositionCalculations',
        'InstrumentCalculations',
        'InstrumentPrices',
        'InstrumentPrice',
        'Instrument',
        'Instruments',
        'InstrumentSplits',
        'PortfolioTransactions',
        'PositionTransactions',
        'InstrumentTransactions',
        'Transaction'
    ]);
}