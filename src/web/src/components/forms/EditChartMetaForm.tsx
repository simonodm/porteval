import React, { useState } from 'react';

import { ChartConfig } from '../../types';

import TextInput from './fields/TextInput';

type Props = {
    chart: ChartConfig;
    onSave: (updatedChart: ChartConfig) => void;
}

export default function EditChartMetaForm({ chart, onSave }: Props): JSX.Element {
    const [name, setName] = useState(chart.name);

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        onSave({
            ...chart,
            name
        });
        e.preventDefault();
    }

    return (
        <form onSubmit={handleSubmit}>
            <TextInput label='Chart name' onChange={(val) => setName(val)} value={chart.name} />
            <button className="btn btn-primary" role="button">Save</button>
        </form>
    )
}