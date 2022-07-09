import React, { useState } from 'react';

import { useGetAllInstrumentsQuery } from '../../redux/api/instrumentApi';

import { TemplateType } from '../../types';

import { checkIsError, checkIsLoaded } from '../../utils/queries';

import LoadingWrapper from '../ui/LoadingWrapper';

import InstrumentDropdown from './fields/InstrumentDropdown';
import TemplateTypeDropdown from './fields/TemplateTypeDropdown';

type Props = {
    onSuccess?: () => void;
}

export default function ExportDataForm({ onSuccess }: Props): JSX.Element {
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
        <div className="form-group">
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
            <button className="btn btn-primary" onClick={onSubmit}>Export</button>
        </div>
    )
}