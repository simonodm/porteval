import { Currency, CurrencyExchangeRate } from '../../types';
import { portEvalApi } from './portEvalApi';

/**
 * PortEval's currency API definition.
 * @category API
 */
const currencyApi = portEvalApi.injectEndpoints({
    endpoints: (build) => ({
        getAllKnownCurrencies: build.query<Array<Currency>, void>({
            query: () => 'currencies',
            providesTags: (result) =>
                result
                    ? ['Currencies']
                    : []
        }),
        getCurrency: build.query<Currency, string>({
            query: (code) => `currencies/${code}`,
            providesTags: (result) =>
                result
                    ? ['Currencies']
                    : []
        }),
        getExchangeRates: build.query<Array<CurrencyExchangeRate>, { codeFrom: string, time: string }>({
            query: ({ codeFrom, time}) =>
                `currencies/${codeFrom}/exchange_rates?time=${encodeURIComponent(time)}`,
            providesTags: (result, error, args) =>
                result
                    ? [{type: 'CurrencyExchangeRates', id: args.codeFrom}]
                    : []
        }),
        getLatestExchangeRates: build.query<Array<CurrencyExchangeRate>, string>({
            query: (codeFrom) =>
                `currencies/${codeFrom}/exchange_rates/latest`,
            providesTags: (result, error, arg) =>
            result
                ? [{type: 'CurrencyExchangeRates', id: arg}]
                : []
        }),
        getExchangeRateAt: build.query<CurrencyExchangeRate, { codeFrom: string, codeTo: string, time: string }>({
            query: ({ codeFrom, codeTo, time }) =>
                `currencies/${codeFrom}/exchange_rates/${codeTo}/at?time=${encodeURIComponent(time)}`,
            providesTags: (result, error, args) =>
            result
                ? [{type: 'CurrencyExchangeRates', id: args.codeFrom}]
                : []
        }),
        getLatestExchangeRate: build.query<CurrencyExchangeRate, { codeFrom: string, codeTo: string }>({
            query: ({ codeFrom, codeTo }) => `currencies/${codeFrom}/exchange_rates/${codeTo}/latest`,
            providesTags: (result, error, args) =>
            result
                ? [{type: 'CurrencyExchangeRates', id: args.codeFrom}]
                : []
        }),
        updateCurrency: build.mutation<Currency, Currency>({
            query: (data) => ({
                url: `currencies/${data.code}`,
                method: 'PUT',
                body: data
            }),
            invalidatesTags: (result, error) =>
                !error
                    ? ['Currencies']
                    : []
        })
    })
});

export const {
    useGetAllKnownCurrenciesQuery,
    useGetCurrencyQuery,
    useGetExchangeRatesQuery,
    useGetLatestExchangeRatesQuery,
    useGetExchangeRateAtQuery,
    useGetLatestExchangeRateQuery,
    useUpdateCurrencyMutation
} = currencyApi