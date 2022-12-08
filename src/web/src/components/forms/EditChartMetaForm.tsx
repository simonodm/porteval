import React, { useState } from 'react';
import { ChartConfig } from '../../types';
import TextInput from './fields/TextInput';

type Props = {
    /**
     * Chart to edit.
     */
    chart: ChartConfig;

    /**
     * A callback which is invoked whenever the form is submitted.
     */
    onSave: (updatedChart: ChartConfig) => void;
}

/**
 * Renders a chart metadata edit form.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function EditChartMetaForm({ chart, onSave }: Props): JSX.Element {
    const [name, setName] = useState(chart.name);

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        onSave({
            ...chart,
            name
        });
        e.preventDefault();
    }

    return (
        <form onSubmit={handleSubmit} aria-label="Edit chart information form">
            <TextInput label='Chart name' onChange={(val) => setName(val)} value={chart.name} />
            <button className="btn btn-primary" role="button">Save</button>
        </form>
    )
}

export default EditChartMetaForm;