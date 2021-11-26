import React from 'react';
import { ChartConfig, ModalCallbacks } from '../../types';
import ChartForm from '../forms/ChartForm'

type Props = {
    chart: ChartConfig;
    onSave: (chart: ChartConfig) => void;
} & ModalCallbacks

export default function EditChartMetaModal({ chart, onSave, closeModal }: Props): JSX.Element {
    const handleSubmit = (name: string) => {
        onSave({
            ...chart,
            name
        });
        closeModal();
    }

    return (
        <ChartForm onSubmit={handleSubmit} defaultName={chart.name} />
    )
}