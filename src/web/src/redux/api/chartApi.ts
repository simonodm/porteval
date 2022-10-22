import { FetchBaseQueryError, FetchBaseQueryMeta } from '@reduxjs/toolkit/dist/query';
import { QueryReturnValue } from '@reduxjs/toolkit/dist/query/baseQueryTypes';
import { Chart, ChartConfig, AggregationFrequency,
    EntityChartDataPoint, isPriceDataChart, Transaction } from '../../types';
import { buildChartLineDataBaseUrl, buildChartLineTransactionsUrl,
    generateChartLineCalcTags, truncateEntityName, generateChartTransactionTags, ChartLineEntityTag,
    generateChartLineReferencedEntityTags } from './apiUtils';
import { portEvalApi } from './portEvalApi';

/**
 * PortEval's chart API definition.
 * @category API
 */
const chartApi = portEvalApi.injectEndpoints({
    endpoints: (build) => ({
        getAllCharts: build.query<Array<Chart>, void>({
            query: () => 'charts',
            providesTags: (result) => 
                result
                    ? [
                        ...result.reduce<Array<ChartLineEntityTag>>(
                            (prev, curr) => [...prev, ...generateChartLineReferencedEntityTags(curr.lines)], []
                        ),
                        ...result.map(chart => ({ type: 'Chart' as const, id: chart.id })),
                        'Charts'
                    ]
                    : []
        }),
        getChart: build.query<Chart, number>({
            query: (id) => `charts/${id}`,
            providesTags: (result, error, arg) =>
                result
                    ? [
                        ...generateChartLineReferencedEntityTags(result.lines),
                        { type: 'Chart', id: arg }
                    ]
                    : []
        }),
        createChart: build.mutation<Chart, ChartConfig>({
            query: (data) => ({
                url: 'charts',
                method: 'POST',
                body: truncateEntityName(data)
            }),
            invalidatesTags: (result, error) =>
                !error
                    ? ['Charts']
                    : []
        }),
        updateChart: build.mutation<Chart, Chart>({
            query: (data) => ({
                url: `charts/${data.id}`,
                method: 'PUT',
                body: truncateEntityName(data)
            }),
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [{ type: 'Chart', id: arg.id }]
                    : []
        }),
        deleteChart: build.mutation<void, number>({
            query: (id) => ({
                url: `charts/${id}`,
                method: 'DELETE'
            }),
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [{ type: 'Chart', id: arg }]
                    : []
        }),
        getChartData: build.query<
            Array<Array<EntityChartDataPoint>>,
            { chart: ChartConfig, from: string, to: string, frequency: AggregationFrequency }
        >({
            queryFn: async (args, api, extraOptions, fetchWithBQ) => {
                const currency = isPriceDataChart(args.chart) ? args.chart.currencyCode : undefined;
                const linesDataUrls = args.chart.lines.map(
                    line => `${buildChartLineDataBaseUrl(args.chart, line)}` +
                        `?from=${encodeURIComponent(args.from)}` +
                        `&to=${encodeURIComponent(args.to)}` +
                        `&frequency=${args.frequency}` +
                        `${currency ? `&currency=${currency}` : ''}`
                );
                
                const dataPromises = linesDataUrls.map(url => fetchWithBQ(url));
                const data = await Promise.all(dataPromises);

                // if all queries failed
                if(dataPromises.length > 0 &&
                    !dataPromises.find(promise =>
                        !(promise as QueryReturnValue<unknown, FetchBaseQueryError, FetchBaseQueryMeta>).error)) {
                    throw new Error('Chart line data could not be fetched.');
                }

                return {
                    data: data.map(
                        promise =>
                            (promise as QueryReturnValue<
                                Array<EntityChartDataPoint>,
                                FetchBaseQueryError,
                                FetchBaseQueryMeta
                            >).data ?? [])
                }
            },
            providesTags: (result, error, arg) =>
                result
                    ? generateChartLineCalcTags(arg.chart.lines)
                    : []
        }),
        getChartTransactions: build.query<Array<Array<Transaction>>, { chart: ChartConfig, from: string, to: string }>({
            queryFn: async (args, api, extraOptions, fetchWithBQ) => {
                const transactionDataUrls = args.chart.lines.map(
                    line => buildChartLineTransactionsUrl(line, args.from, args.to)
                );

                const dataPromises = transactionDataUrls.map(url => fetchWithBQ(url));
                const data = await Promise.all(dataPromises);

                // if all queries failed
                if(dataPromises.length > 0 &&
                    !dataPromises.find(promise =>
                        !(promise as QueryReturnValue<unknown, FetchBaseQueryError, FetchBaseQueryMeta>).error)) {
                    throw new Error('Chart line transaction data could not be fetched.');
                }

                return {
                    data: data.map(
                        promise =>
                            (promise as QueryReturnValue<
                                Array<Transaction>,
                                FetchBaseQueryError,
                                FetchBaseQueryMeta
                            >).data ?? [])
                };
            },
            providesTags: (result, error, arg) =>
                result
                    ? generateChartTransactionTags(arg.chart.lines)
                    : []
        })
    })
});

export const {
    useGetAllChartsQuery,
    useGetChartQuery,
    useCreateChartMutation,
    useUpdateChartMutation,
    useDeleteChartMutation,
    useGetChartDataQuery,
    useGetChartTransactionsQuery,
} = chartApi