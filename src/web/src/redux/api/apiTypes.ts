import { FetchBaseQueryError } from '@reduxjs/toolkit/dist/query';
import { SerializedError } from '@reduxjs/toolkit';

import { Instrument, Portfolio, Position, Transaction } from '../../types'

export type CreateInstrumentParameters = Omit<Instrument, 'id'>;
export type CreatePortfolioParameters = Omit<Portfolio, 'id'>;
export type CreatePositionParameters = Omit<Position, 'id' | 'instrument'> & {
    initialTransaction: Omit<CreateTransactionParameters, 'positionId'>;
}
export type CreateTransactionParameters = Omit<Transaction, 'id' | 'instrument' | 'portfolioId'>;

export type PaginationParameters = {
    page?: number;
    limit?: number;
}

export type DateRangeParameters = {
    from?: string;
    to?: string;
}

export type RequestErrorResponse = {
    statusCode: number;
    errorMessage: string;
}

export function isRequestErrorResponse(value: unknown): value is RequestErrorResponse {
    const valueAsResponse = value as RequestErrorResponse;
    return valueAsResponse !== undefined
            && valueAsResponse.statusCode !== undefined
            && valueAsResponse.errorMessage !== undefined;            
}

export type ValidationErrorResponse = {
    errors: {
        [key: string]: Array<string>
    }
}

export function isValidationErrorResponse(value: unknown): value is ValidationErrorResponse {
    const valueAsResponse = value as ValidationErrorResponse;
    return valueAsResponse !== undefined && valueAsResponse.errors !== undefined;
}

export function isSuccessfulResponse<T>(
    res: { data: T } | { error: FetchBaseQueryError | SerializedError }
): res is { data: T } {
    const resAsData = res as { data: T };
    return resAsData !== undefined && resAsData.data !== undefined;
}

