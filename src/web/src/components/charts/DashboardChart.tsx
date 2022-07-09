import React from 'react'

import { Link } from 'react-router-dom';

import { Chart } from '../../types'

import PortEvalChart from './PortEvalChart';

import './DashboardChart.css';

type Props = {
    chart: Chart;
    disabled?: boolean;
}

export default function DashboardChart({ chart, disabled }: Props): JSX.Element {
    return (
        <div className="dashboard-chart-item-wrapper">
            {
                disabled
                    ? <h6 className="mb-0 text-center">{chart.name}</h6>
                    : 
                    <Link to={`/charts/view/${chart.id}`}>
                        <h6 className="mb-0 text-center">{chart.name}</h6>
                    </Link>
            }
            <PortEvalChart chart={chart} />
        </div>
    )
}