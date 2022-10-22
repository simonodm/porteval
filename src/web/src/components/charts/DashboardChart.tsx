import React from 'react'
import PortEvalChart from './PortEvalChart';

import { Link } from 'react-router-dom';
import { Chart } from '../../types'

import './DashboardChart.css';

type Props = {
    /**
     * Chart to render.
     */
    chart: Chart;

    /**
     * Determines whether the header link to full-page chart view is disabled.
     */
    disabled?: boolean;
}

/**
 * Renders a single chart for the dashboard grid.
 * 
 * @category Chart
 * @component
 */
function DashboardChart({ chart, disabled }: Props): JSX.Element {
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

export default DashboardChart;