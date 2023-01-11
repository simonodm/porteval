import React from 'react';
import { NavLink } from 'react-router-dom';
import { ChartConfig } from '../../types';
import PortEvalChart from './PortEvalChart';

import './ChartPreview.css';

type Props = {
    chart: ChartConfig;
}

export default function ChartPreview({ chart }: Props): JSX.Element {
    return (
        <div className="chart-preview-wrapper">
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