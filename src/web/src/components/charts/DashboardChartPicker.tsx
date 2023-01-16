import React from 'react';
import { Chart } from '../../types';

type Props = {
    /**
     * A list of charts to render.
     */
    charts: Array<Chart>;

    /**
     * A callback which is invoked every time a chart is dragged.
     */
    onDrag: (id: number) => void;
}

/**
 * Renders a list of draggable charts.
 * 
 * @category Chart
 * @component
 */
function DashboardChartPicker({ charts, onDrag }: Props): JSX.Element {   
    const handleDrag = (id: number) => {
        onDrag(id);
    }
    
    return (
        <div role="picker" aria-label="Dashboard chart picker">
            {charts.map(chart => 
                <div
                    className="picker-item draggable"
                    data-grid={{i: chart.id}}
                    draggable
                    key={chart.id}
                    onDrag={() => handleDrag(chart.id)}
                >
                    <span className="picker-item-name">{chart.name}</span>
                </div>
        )}
        </div>
    )
}

export default DashboardChartPicker;