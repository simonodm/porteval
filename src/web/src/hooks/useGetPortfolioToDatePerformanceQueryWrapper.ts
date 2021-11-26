import { checkIsLoaded } from '../components/utils/queries';
import { 
    useGetPortfolioLastDayPerformanceQuery,
    useGetPortfolioLastMonthPerformanceQuery,
    useGetPortfolioLastWeekPerformanceQuery,
    useGetPortfolioTotalPerformanceQuery
} from '../redux/api/portfolioApi';
import { ToDateFinancialDataQueryResponse } from '../types';
import * as constants from '../constants';
import { skipToken } from '@reduxjs/toolkit/dist/query';

export default function useGetPortfolioToDatePerformanceQueryWrapper(portfolioId: number, skip?: boolean): ToDateFinancialDataQueryResponse {
    const options = { pollingInterval: constants.REFRESH_INTERVAL };

    const performanceDaily = useGetPortfolioLastDayPerformanceQuery(skip ? skipToken : portfolioId, options);
    const performanceWeekly = useGetPortfolioLastWeekPerformanceQuery(skip ? skipToken : portfolioId, options);
    const performanceMonthly = useGetPortfolioLastMonthPerformanceQuery(skip ? skipToken : portfolioId, options);
    const performanceTotal = useGetPortfolioTotalPerformanceQuery(skip ? skipToken : portfolioId, options);

    const isLoaded = checkIsLoaded(performanceDaily, performanceWeekly, performanceMonthly, performanceTotal);

    return {
        lastDay: performanceDaily.data?.performance ?? 0,
        lastWeek: performanceWeekly.data?.performance ?? 0,
        lastMonth: performanceMonthly.data?.performance ?? 0,
        total: performanceTotal.data?.performance ?? 0,
        isLoading: !isLoaded
    }
}