import { skipToken } from '@reduxjs/toolkit/dist/query';

import { checkIsLoaded } from '../components/utils/queries';
import {
    useGetPositionLastDayProfitQuery,
    useGetPositionLastMonthProfitQuery,
    useGetPositionLastWeekProfitQuery,
    useGetPositionTotalProfitQuery
} from '../redux/api/positionApi';
import { ToDateFinancialDataQueryResponse } from '../types';
import * as constants from '../constants';

export default function useGetPositionToDateProfitsQueryWrapper(
    positionId: number, skip?: boolean
): ToDateFinancialDataQueryResponse {
    const options = { pollingInterval: constants.REFRESH_INTERVAL };

    const profitDaily = useGetPositionLastDayProfitQuery(
        skip ? skipToken : {
            positionId: positionId
        }, options);

    const profitWeekly = useGetPositionLastWeekProfitQuery(
        skip ? skipToken : {
            positionId: positionId
        }, options);

    const profitMonthly = useGetPositionLastMonthProfitQuery(
        skip ? skipToken : {
            positionId: positionId
        }, options);

    const profitTotal = useGetPositionTotalProfitQuery(
        skip ? skipToken : {
            positionId: positionId
        }, options);

    const isLoaded = checkIsLoaded(profitDaily, profitWeekly, profitMonthly, profitTotal);

    return {
        lastDay: profitDaily.data?.profit ?? 0,
        lastWeek: profitWeekly.data?.profit ?? 0,
        lastMonth: profitMonthly.data?.profit ?? 0,
        total: profitTotal.data?.profit ?? 0,
        isLoading: !isLoaded
    }
}