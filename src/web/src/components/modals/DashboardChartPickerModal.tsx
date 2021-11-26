import React from 'react';
import { Chart, ModalCallbacks } from '../../types';

type Props = {
    charts: Array<Chart>;
    onDrag: (id: number) => void;
} & ModalCallbacks

export default function DashboardChartPickerModal({ charts, onDrag, closeModal }: Props): JSX.Element {   
    const handleDrag = (id: number) => {
        onDrag(id);
        closeModal();
    }
    
    return (
        <div>
        {charts.map(chart => 
            <div
                key={chart.id}
                draggable
                className="picker-item draggable"
                onDrag={() => handleDrag(chart.id)}
                data-grid={{i: chart.id}}>
                <span className="picker-item-name">{chart.name}</span>
            </div>
        )}
        </div>
    )
}