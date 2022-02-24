import React from 'react';

export default function PortfoliosTableHeaders(): JSX.Element {
    return (
        <thead>
            <tr>
                <th className="w-10">Name</th>
                <th className="w-5">Exchange</th>
                <th className="w-5">Currency</th>
                <th className="w-25" colSpan={4}>Profit</th>
                <th className="w-20" colSpan={4}>Performance</th>
                <th className="w-5">BEP</th>
                <th className="w-10">Note</th>
                <th className="w-20">Actions</th>
            </tr>
            <tr>
                <th colSpan={3}></th>
                <th>Daily</th>
                <th>Weekly</th>
                <th>Monthly</th>
                <th>Total</th>
                <th>Daily</th>
                <th>Weekly</th>
                <th>Monthly</th>
                <th>Total</th>
                <th colSpan={3}></th>
            </tr>
        </thead>
    )
}