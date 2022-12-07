import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import PortfolioDropdown from './fields/PortfolioDropdown';
import InstrumentDropdown from './fields/InstrumentDropdown';
import TextInput from './fields/TextInput';

import { useGetAllInstrumentsQuery } from '../../redux/api/instrumentApi';
import { useGetAllPortfoliosQuery } from '../../redux/api/portfolioApi';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../../utils/queries';
import { useUpdatePositionMutation } from '../../redux/api/positionApi';
import { Position } from '../../types';

type Props = {
    /**
     * Position to edit.
     */
    position: Position;

    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess?: () => void;
}

/**
 * Renders a position edit form.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function EditPositionForm({ position, onSuccess }: Props): JSX.Element {
    const [updatePosition, mutationStatus] = useUpdatePositionMutation();
    const [note, setNote] = useState(position.note);

    const instruments = useGetAllInstrumentsQuery();
    const portfolios = useGetAllPortfoliosQuery();

    const isLoaded = checkIsLoaded(instruments, portfolios, mutationStatus);
    const isError = checkIsError(instruments, portfolios);

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        updatePosition({
            ...position,
            note
        }).then(res => onSuccessfulResponse(res, onSuccess));

        e.preventDefault();
    }

    return (
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <form onSubmit={handleSubmit}>
                <PortfolioDropdown disabled portfolios={portfolios.data ?? []} value={position.portfolioId} />
                <InstrumentDropdown disabled instruments={instruments.data ?? []} value={position.instrumentId} />
                <TextInput label='Note' onChange={(val) => setNote(val)} value={note} />
                <button 
                    className="btn btn-primary"
                    role="button"
                >Save
                </button>
            </form>
        </LoadingWrapper>
    )
}

export default EditPositionForm;