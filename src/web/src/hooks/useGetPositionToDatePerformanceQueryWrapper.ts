import { checkIsLoaded } from '../components/utils/queries';
import { 
    useGetPositionLastDayPerformanceQuery,
    useGetPositionLastMonthPerformanceQuery,
    useGetPositionLastWeekPerformanceQuery,
    useGetPositionTotalPerformanceQuery
} from '../redux/api/positionApi';
import { ToDateFinancialDataQueryResponse } from '../types';
import * as constants from '../constants';
import { skipToken } from '@reduxjs/toolkit/dist/query';

export default function useGetPositionToDatePerformanceQueryWrapper(positionId: number, skip?: boolean): ToDateFinancialDataQueryResponse {
    const options = { pollingInterval: constants.REFRESH_INTERVAL };

    const performanceDaily = useGetPositionLastDayPerformanceQuery(
        skip ? skipToken : {
            positionId: positionId
        }, options);

    const performanceWeekly = useGetPositionLastWeekPerformanceQuery(
        skip ? skipToken : {
            positionId: positionId
        }, options);

    const performanceMonthly = useGetPositionLastMonthPerformanceQuery(
        skip ? skipToken : {
            positionId: positionId
        }, options);
    const performanceTotal = useGetPositionTotalPerformanceQuery(
        skip ? skipToken : {
            positionId: positionId
        }, options);

    const isLoaded = checkIsLoaded(performanceDaily, performanceWeekly, performanceMonthly, performanceTotal);

    return {
        lastDay: performanceDaily.data?.performance ?? 0,
        lastWeek: performanceWeekly.data?.performance ?? 0,
        lastMonth: performanceMonthly.data?.performance ?? 0,
        total: performanceTotal.data?.performance ?? 0,
        isLoading: !isLoaded
    }
}