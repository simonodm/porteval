import React from 'react';
import { ChartLineConfigurationContextType } from '../types';

/**
 * A shared context for chart line addition, removal and configuration.
 * 
 * @category Contexts
 */
const ChartLineConfigurationContext = React.createContext<ChartLineConfigurationContextType>({
    chart: undefined,
    addInstrumentLine: () => { /* do nothing */ },
    addPositionLine: () => { /* do nothing */ },
    addPortfolioPositionLines: () => { /* do nothing */ },
    addPortfolioLine: () => { /* do nothing */ },
    configureLine: () => { /* do nothing */ },
    removeLine: () => { /* do nothing */ }
});

export default ChartLineConfigurationContext;