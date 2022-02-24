import { skipToken } from '@reduxjs/toolkit/dist/query';

import { checkIsLoaded } from '../components/utils/queries';
import {
    useGetPortfolioLastDayProfitQuery,
    useGetPortfolioLastMonthProfitQuery,
    useGetPortfolioLastWeekProfitQuery,
    useGetPortfolioTotalProfitQuery
} from '../redux/api/portfolioApi';
import { ToDateFinancialDataQueryResponse } from '../types';
import * as constants from '../constants';

export default function useGetPortfolioToDateProfitsQueryWrapper(
    portfolioId: number, skip?: boolean
): ToDateFinancialDataQueryResponse {
    const options = { pollingInterval: constants.REFRESH_INTERVAL };

    const profitDaily = useGetPortfolioLastDayProfitQuery(skip ? skipToken : portfolioId, options);
    const profitWeekly = useGetPortfolioLastWeekProfitQuery(skip ? skipToken : portfolioId, options);
    const profitMonthly = useGetPortfolioLastMonthProfitQuery(skip ? skipToken : portfolioId, options);
    const profitTotal = useGetPortfolioTotalProfitQuery(skip ? skipToken : portfolioId, options);

    const isLoaded = checkIsLoaded(profitDaily, profitWeekly, profitMonthly, profitTotal);

    return {
        lastDay: profitDaily.data?.profit ?? 0,
        lastWeek: profitWeekly.data?.profit ?? 0,
        lastMonth: profitMonthly.data?.profit ?? 0,
        total: profitTotal.data?.profit ?? 0,
        isLoading: !isLoaded
    }
}