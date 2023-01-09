import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import InstrumentDropdown from './fields/InstrumentDropdown';
import TemplateTypeDropdown from './fields/TemplateTypeDropdown';

import { useGetAllInstrumentsQuery } from '../../redux/api/instrumentApi';
import { TemplateType } from '../../types';
import { checkIsError, checkIsLoaded } from '../../utils/queries';

type Props = {
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
function ExportDataForm({ onSuccess }: Props): JSX.Element {
    const [templateType, setTemplateType] = useState<TemplateType>('portfolios');
    
    const [instrumentId, setInstrumentId] = useState<number | undefined>(undefined);

    const instruments = useGetAllInstrumentsQuery();

    const isLoaded = checkIsLoaded(instruments);
    const isError = checkIsError(instruments);
    
    const getExportUrl = () => {
        const baseUrl = '/api/export/';

        switch(templateType) {
            case 'prices':
                return baseUrl + 'instruments/' + instrumentId + '/prices';
            default:
                return baseUrl + templateType;
        }
    }

    const onSubmit = () => {
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
        <form onSubmit={onSubmit} aria-label="Export CSV data form">
            <TemplateTypeDropdown onChange={setTemplateType} />
            <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                {
                    templateType === 'prices'
                        ?
                            <InstrumentDropdown
                                instruments={instruments.data ?? []}
                                onChange={setInstrumentId}
                                value={instrumentId}
                            />
                        : null
                }
            </LoadingWrapper>
            <input type="submit" className="btn btn-primary" value="Export" />
        </form>
    )
}

export default ExportDataForm;