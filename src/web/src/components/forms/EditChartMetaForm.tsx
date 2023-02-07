import React, { useState } from 'react';
import TextInput from './fields/TextInput';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { ChartConfig } from '../../types';

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
        <Form onSubmit={handleSubmit} aria-label="Edit chart information form">
            <TextInput className="mb-3" label='Chart name' onChange={(val) => setName(val)}
                value={chart.name}
            />
            <Button variant="primary" type="submit">Save</Button>
        </Form>
    )
}

export default EditChartMetaForm;