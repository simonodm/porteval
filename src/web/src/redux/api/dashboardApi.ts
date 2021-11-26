import { portEvalApi } from './portEvalApi';
import { DashboardLayout } from '../../types';

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