import React from 'react';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useAddPositionMutation } from '../../redux/api/positionApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { ModalCallbacks } from '../../types';
import PositionForm from '../forms/PositionForm';

type Props = {
    portfolioId?: number;
} & ModalCallbacks;

export default function CreatePositionModal({ portfolioId, closeModal }: Props): JSX.Element {
    const [createPosition, mutationStatus] = useAddPositionMutation();

    const isLoaded = checkIsLoaded(mutationStatus);
    const isError = checkIsError(mutationStatus);

    const handleSubmit = (portfolioId: number, instrumentId: number, note: string) => {
        const position = {
            portfolioId,
            instrumentId,
            note
        };

        createPosition(position).then(() => closeModal());
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <PositionForm portfolioId={portfolioId} onSubmit={handleSubmit} />
        </LoadingWrapper>
    )

}