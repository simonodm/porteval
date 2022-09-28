import { Exchange } from '../../types';
import { portEvalApi } from './portEvalApi';

/**
 * PortEval's stock exchanges API definition.
 * @category API
 */
const exchangeApi = portEvalApi.injectEndpoints({
    endpoints: (build) => ({
        getAllKnownExchanges: build.query<Array<Exchange>, void>({
            query: () => 'exchanges',
            providesTags: (result) =>
                result
                    ? ['Exchanges']
                    : []
        })
    })
});

export const { useGetAllKnownExchangesQuery } = exchangeApi;