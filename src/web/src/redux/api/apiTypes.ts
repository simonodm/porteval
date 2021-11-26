import { Instrument, Portfolio, Position, Transaction } from '../../types'

export type CreateInstrumentParameters = Omit<Instrument, 'id'>;
export type CreatePortfolioParameters = Omit<Portfolio, 'id'>;
export type CreatePositionParameters = Omit<Position, 'id' | 'instrument'>;
export type CreateTransactionParameters = Omit<Transaction, 'id'>;

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

