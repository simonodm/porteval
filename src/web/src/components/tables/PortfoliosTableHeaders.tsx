import React from 'react';
import { EntityStatistics, Portfolio } from '../../types';

export type PortfolioSortableHeader = keyof (Portfolio & EntityStatistics);

type Props = {
    onSort?: (header: PortfolioSortableHeader) => void;
}

export default function PortfoliosTableHeaders({ onSort }: Props): JSX.Element {
    const handleSort = (header: PortfolioSortableHeader) => {
        onSort && onSort(header);
    }
    
    return (
        <thead>
            <tr>
                <th className="w-10" onClick={() => handleSort('name')}>Name</th>
                <th className="w-5">Exchange</th>
                <th className="w-5" onClick={() => handleSort('currencyCode')}>Currency</th>
                <th className="w-25" colSpan={4}>Profit</th>
                <th className="w-20" colSpan={4}>Performance</th>
                <th className="w-5">BEP</th>
                <th className="w-10" onClick={() => handleSort('note')}>Note</th>
                <th className="w-20">Actions</th>
            </tr>
            <tr>
                <th colSpan={3}></th>
                <th onClick={() => handleSort('lastDayProfit')}>Daily</th>
                <th onClick={() => handleSort('lastWeekProfit')}>Weekly</th>
                <th onClick={() => handleSort('lastMonthProfit')}>Monthly</th>
                <th onClick={() => handleSort('totalProfit')}>Total</th>
                <th onClick={() => handleSort('lastDayPerformance')}>Daily</th>
                <th onClick={() => handleSort('lastWeekPerformance')}>Weekly</th>
                <th onClick={() => handleSort('lastMonthPerformance')}>Monthly</th>
                <th onClick={() => handleSort('totalPerformance')}>Total</th>
                <th colSpan={3}></th>
            </tr>
        </thead>
    )
}