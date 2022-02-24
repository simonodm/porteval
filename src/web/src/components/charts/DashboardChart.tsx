import React from 'react'

import { Chart } from '../../types'

import PortEvalChart from './PortEvalChart';
import './DashboardChart.css';

type Props = {
    chart: Chart;
}

export default function DashboardChart({ chart }: Props): JSX.Element {
    return (
        <div className="dashboard-chart-item-wrapper">
            <h6 className="mb-0 text-center">{chart.name}</h6>
            <PortEvalChart chart={chart} />
        </div>
    )
}