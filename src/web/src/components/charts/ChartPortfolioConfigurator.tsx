import React from 'react';
import PositionPicker from '../charts/PositionPicker';
import { Portfolio } from '../../types';

type Props = {
    /**
     * Portfolio to load and display positions for.
     */
    portfolio: Portfolio;
}

/**
 * Renders a portfolio's list of positions to be added to a chart.
 * 
 * @category Chart
 * @component
 */
function ChartPortfolioConfiguratorModal({ portfolio }: Props): JSX.Element {
    return (
        <PositionPicker portfolio={portfolio} />
    )
}

export default ChartPortfolioConfiguratorModal;