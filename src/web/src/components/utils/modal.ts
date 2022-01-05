import { SerializedError } from '@reduxjs/toolkit';
import { FetchBaseQueryError } from '@reduxjs/toolkit/dist/query';

type PromiseReturnType<T> = {
    data: T
} | {
    error: FetchBaseQueryError | SerializedError
};

export function onSuccessfulResponse<T>(val: PromiseReturnType<T>, callback: () => void): void {
    if(!Object.hasOwnProperty.call(val, 'error')) {
        callback();
    }
}