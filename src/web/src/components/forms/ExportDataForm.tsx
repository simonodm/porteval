import React, { useState } from 'react';
import InstrumentDropdown from './fields/InstrumentDropdown';
import TemplateTypeDropdown from './fields/TemplateTypeDropdown';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { Instrument, TemplateType } from '../../types';

type Props = {
    /**
     * A list of instruments to select from for price export.
     */
    instruments: Instrument[];

    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess?: () => void;
}

/**
 * Renders a CSV data export form.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function ExportDataForm({ instruments, onSuccess }: Props): JSX.Element {
    const [templateType, setTemplateType] = useState<TemplateType>('portfolios');
    const [instrumentId, setInstrumentId] = useState<number | undefined>(undefined);
    
    const getExportUrl = () => {
        const baseUrl = '/api/export/';

        switch(templateType) {
            case 'prices':
                return baseUrl + 'instruments/' + instrumentId + '/prices';
            default:
                return baseUrl + templateType;
        }
    }

    const handleSubmit = () => {
        const fetchUrl = getExportUrl();

        fetch(fetchUrl)
            .then(res => res.blob())
            .then(blob => {
                const file = URL.createObjectURL(blob);
                location.assign(file);
            });

        onSuccess && onSuccess();
    }

    return (
        <Form aria-label="Export CSV data form">
            <TemplateTypeDropdown className="mb-2" label='Export data type' onChange={setTemplateType} />
            {
                templateType === 'prices'
                    ?
                        <InstrumentDropdown
                            className="mb-2"
                            instruments={instruments}
                            onChange={setInstrumentId}
                            value={instrumentId}
                        />
                    : null
            }
            <Button variant="primary" onClick={handleSubmit}>Export</Button>
        </Form>
    )
}

export default ExportDataForm;