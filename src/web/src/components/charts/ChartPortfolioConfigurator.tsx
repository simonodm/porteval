import React from 'react';

import { Portfolio } from '../../types';
import PositionPicker from '../charts/PositionPicker';

type Props = {
    portfolio: Portfolio;
}

export default function ChartPortfolioConfiguratorModal({ portfolio }: Props): JSX.Element {
    return (
        <PositionPicker portfolio={portfolio} />
    )
}