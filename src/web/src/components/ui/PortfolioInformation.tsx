import React from 'react';
import useUserSettings from '../../hooks/useUserSettings';

import { EntityStatistics, Portfolio } from '../../types';
import { getPerformanceString, getPriceString } from '../../utils/string';

type Props = {
    /**
     * Portfolio to render information of.
     */
    portfolio: Portfolio;

    /**
     * Statistics of the portfolio to render information of.
     */
    stats: EntityStatistics;

    /**
     * Portfolio's current value.
     */
    value: number;
}

/**
 * Renders the portfolio's key information in a table format.
 * 
 * @category UI
 * @component
 */
function PortfolioInformation({ portfolio, stats, value }: Props): JSX.Element {
    const [userSettings] = useUserSettings();

    return (
        <table className="entity-data w-100">
            <tbody>
                <tr>
                    <td>Name:</td>
                    <td>{portfolio.name}</td>
                </tr>
                <tr>
                    <td>Current value:</td>
                    <td>
                        {
                            getPriceString(
                                value,
                                portfolio.currencyCode,
                                userSettings)
                        }
                    </td>
                </tr>
                <tr>
                    <td>Total profit:</td>
                    <td>
                        {
                                getPriceString(
                                    stats.totalProfit,
                                    portfolio.currencyCode,
                                    userSettings
                                )
                            }
                    </td>
                </tr>
                <tr>
                    <td>Total performance:</td>
                    <td>
                        {
                                getPerformanceString(stats.totalPerformance, userSettings)
                            }
                    </td>
                </tr>
                <tr>
                    <td>Daily/weekly/monthly profit:</td>
                    <td>
                        {
                                getPriceString(
                                    stats.lastDayProfit,
                                    portfolio.currencyCode,
                                    userSettings
                                ) + ' / '
                            }
                        {
                                getPriceString(
                                    stats.lastWeekProfit,
                                    portfolio.currencyCode,
                                    userSettings
                                ) + ' / '
                            }
                        {
                                getPriceString(
                                    stats.lastMonthProfit,
                                    portfolio.currencyCode,
                                    userSettings
                                )
                            }
                    </td>
                </tr>
                <tr>
                    <td>Daily/weekly/monthly performance:</td>
                    <td>
                        {
                                getPerformanceString(
                                    stats.lastDayPerformance,
                                    userSettings
                                ) + ' / '
                            }
                        {
                                getPerformanceString(
                                    stats.lastWeekPerformance,
                                    userSettings
                                ) + ' / '
                            }
                        {
                                getPerformanceString(
                                    stats.lastMonthPerformance,
                                    userSettings
                                )
                            }
                    </td>
                </tr>
                <tr>
                    <td>Note:</td>
                    <td>{portfolio.note}</td>
                </tr>
            </tbody>
        </table>   
    );
}

export default PortfolioInformation;