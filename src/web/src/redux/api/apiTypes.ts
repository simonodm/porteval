import { FetchBaseQueryError } from '@reduxjs/toolkit/dist/query';
import { SerializedError } from '@reduxjs/toolkit';
import { Instrument, InstrumentSplit, Portfolio, Position, Transaction } from '../../types'

/**
 * Parameters required to create an instrument using PortEval's API.
 * @category API
 */
export type CreateInstrumentParameters = Omit<Instrument, 'id'>;

/**
 * Parameters required to create a portfolio using PortEval's API.
 * @category API
 */
export type CreatePortfolioParameters = Omit<Portfolio, 'id'>;

/**
 * Parameters required to create a position using PortEval's API.
 * @category API
 */
export type CreatePositionParameters = Omit<Position, 'id' | 'instrument' | 'positionSize'> 
    & Omit<CreateTransactionParameters, 'positionId' | 'note'>

/**
 * Parameters required to create a transaction using PortEval's API.
 * @category API
 */
export type CreateTransactionParameters = Omit<Transaction, 'id' | 'instrument' | 'portfolioId'>;

/**
 * Parameters required to update an instrument split using PortEval's API.
 */
export type CreateInstrumentSplitParameters = Omit<InstrumentSplit, 'id' | 'status'>;

/**
 * PortEval's API pagination query parameters
 * @category API
 */
export type PaginationParameters = {
    page?: number;
    limit?: number;
}

/**
 * PortEval's API date range query parameters.
 * @category API
 */
export type DateRangeParameters = {
    from?: string;
    to?: string;
}

/**
 * PortEval's API generic error response.
 * @category API
 */
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

/**
 * PortEval's API data validation error response.
 * @category API
 */
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

