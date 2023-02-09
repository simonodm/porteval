import React from 'react';
import PortEvalChart from './PortEvalChart';

import { NavLink } from 'react-router-dom';
import { ChartConfig } from '../../types';

import './ChartPreview.css';

type Props = {
    /**
     * The chart to render.
     */
    chart: ChartConfig;
}

/**
 * Renders a preview of a chart with a button to navigate to full-sized view.
 * 
 * @category Charts
 * @component
 */
export default function ChartPreview({ chart }: Props): JSX.Element {
    return (
        <div className="chart-preview-wrapper" aria-label="Chart preview">
            <div className="action-buttons clearfix">
                <NavLink
                    className="btn btn-primary float-right btn-sm"
                    to={{ pathname: '/charts/view', state: { chart }}}
                >Go to chart
                </NavLink>
            </div>            
            <PortEvalChart chart={chart} />
        </div>
    )
}