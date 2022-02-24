import React, { useState } from 'react';
import { useGetAllInstrumentsQuery } from '../../redux/api/instrumentApi';
import { useGetAllPortfoliosQuery } from '../../redux/api/portfolioApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError, onSuccessfulResponse } from '../utils/queries';
import PortfolioDropdown from './fields/PortfolioDropdown';
import InstrumentDropdown from './fields/InstrumentDropdown';
import TextInput from './fields/TextInput';
import { useUpdatePositionMutation } from '../../redux/api/positionApi';
import { Position } from '../../types';

type Props = {
    position: Position;
    onSuccess?: () => void;
}

export default function EditPositionForm({ position, onSuccess }: Props): JSX.Element {
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
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={handleSubmit}>
                <PortfolioDropdown portfolios={portfolios.data ?? []} disabled value={position.portfolioId} />
                <InstrumentDropdown instruments={instruments.data ?? []} disabled value={position.instrumentId} />
                <TextInput label='Note' value={note} onChange={(val) => setNote(val)} />
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
    )
}