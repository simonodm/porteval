import { DashboardLayout } from '../../types';
import { portEvalApi } from './portEvalApi';

/**
 * PortEval's dashboard API definition.
 * @category API
 */
const dashboardApi = portEvalApi.injectEndpoints({
    endpoints: (build) => ({
        getDashboardLayout: build.query<DashboardLayout, void>({
            query: () => 'dashboard',
            providesTags: (result) => 
                result
                    ? ['DashboardLayout']
                    : []

        }),
        updateDashboardLayout: build.mutation<DashboardLayout, DashboardLayout>({
            query: (data) => ({
                url: 'dashboard',
                method: 'POST',
                body: data
            }),
            invalidatesTags: (result, error) =>
                !error
                    ? ['DashboardLayout']
                    : []
        })
    })
});

export const {
    useGetDashboardLayoutQuery,
    useUpdateDashboardLayoutMutation
} = dashboardApi;