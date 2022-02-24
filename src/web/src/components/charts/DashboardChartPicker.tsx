import React from 'react';

import { Chart } from '../../types';

type Props = {
    charts: Array<Chart>;
    onDrag: (id: number) => void;
}

export default function DashboardChartPicker({ charts, onDrag }: Props): JSX.Element {   
    const handleDrag = (id: number) => {
        onDrag(id);
    }
    
    return (
        <div>
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