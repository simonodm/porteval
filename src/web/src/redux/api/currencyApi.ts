import { Currency, CurrencyExchangeRate } from '../../types';

import { portEvalApi } from './portEvalApi';

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
                `currencies/${codeFrom}/exchange_rates?time=${encodeURIComponent(time)}`
        }),
        getLatestExchangeRates: build.query<Array<CurrencyExchangeRate>, string>({
            query: (codeFrom) =>
                `currencies/${codeFrom}/exchange_rates/latest`
        }),
        getExchangeRateAt: build.query<CurrencyExchangeRate, { codeFrom: string, codeTo: string, time: string }>({
            query: ({ codeFrom, codeTo, time }) =>
                `currencies/${codeFrom}/exchange_rates/${codeTo}/at?time=${encodeURIComponent(time)}`
        }),
        getLatestExchangeRate: build.query<CurrencyExchangeRate, { codeFrom: string, codeTo: string }>({
            query: ({ codeFrom, codeTo }) => `currencies/${codeFrom}/exchange_rates/${codeTo}/latest`
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