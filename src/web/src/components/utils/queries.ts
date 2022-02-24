import { SerializedError } from '@reduxjs/toolkit';
import { FetchBaseQueryError } from '@reduxjs/toolkit/dist/query';

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

export function checkIsLoaded(...args: Array<Query>): boolean {
    return !args.map(arg => arg.isFetching || arg.isLoading).reduce((prev, curr) => prev || curr);
}

export function checkIsError(...args: Array<Query>): boolean {
    return args.map(arg => !!arg.error).reduce((prev, curr) => prev || curr);
}

export function onSuccessfulResponse<T>(val: PromiseReturnType<T>, callback?: (data: T) => void): void {
    if(!Object.hasOwnProperty.call(val, 'error')) {
        callback && callback((<SuccessfulResponse<T>>val).data);
    }
}