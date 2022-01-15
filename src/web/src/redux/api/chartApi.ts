import { Chart, ChartConfig, ChartFrequency, EntityChartDataPoint, isPriceDataChart } from '../../types';
import { buildChartLineDataBaseUrl, generateChartLinesTags, truncateEntityName } from './apiUtils';
import { portEvalApi } from './portEvalApi';
import { FetchBaseQueryError, FetchBaseQueryMeta } from '@reduxjs/toolkit/dist/query';
import { QueryReturnValue } from '@reduxjs/toolkit/dist/query/baseQueryTypes';

const chartApi = portEvalApi.injectEndpoints({
    endpoints: (build) => ({
        getAllCharts: build.query<Array<Chart>, void>({
            query: () => 'charts',
            providesTags: (result) => 
                result
                    ? [
                        ...result.map(({ id }) => ({ type: 'Chart' as const, id })),
                        ...generateChartLinesTags(result.map(({ lines }) => lines).reduce((prevLines, lines) => [...prevLines, ...lines], [])),
                        'Charts'
                      ]
                    : []
        }),
        getChart: build.query<Chart, number>({
            query: (id) => `charts/${id}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'Chart', id: arg }]
                    : []
        }),
        createChart: build.mutation<ChartConfig, ChartConfig>({
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
                    ? ['Charts', { type: 'Chart', id: arg }]
                    : []
        }),
        getChartData: build.query<Array<Array<EntityChartDataPoint>>, { chart: ChartConfig, from: string, to: string, frequency: ChartFrequency }>({
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
                if(dataPromises.length > 0 && !dataPromises.find(promise => !(promise as QueryReturnValue<unknown, FetchBaseQueryError, FetchBaseQueryMeta>).error)) {
                    throw new Error('Chart line data could not be fecthed.');
                }

                return {
                    data: data.map(promise => (promise as QueryReturnValue<Array<EntityChartDataPoint>, FetchBaseQueryError, FetchBaseQueryMeta>).data ?? [])
                }
            },
            providesTags: (result, error, arg) =>
                result
                    ? generateChartLinesTags(arg.chart.lines)
                    : []
        }),
    })
});

export const {
    useGetAllChartsQuery,
    useGetChartQuery,
    useCreateChartMutation,
    useUpdateChartMutation,
    useDeleteChartMutation,
    useGetChartDataQuery
} = chartApi