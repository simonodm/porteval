import React from 'react';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useUpdatePositionMutation } from '../../redux/api/positionApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { ModalCallbacks, Position } from '../../types';
import PositionForm from '../forms/PositionForm';

type Props = {
    position: Position;
} & ModalCallbacks;

export default function EditPositionModal({ position, closeModal }: Props): JSX.Element {
    const [updatePosition, mutationStatus] = useUpdatePositionMutation();

    const isLoaded = checkIsLoaded(mutationStatus);
    const isError = checkIsError(mutationStatus);

    const handleSubmit = (portfolioId: number, instrumentId: number, note: string) => {
        const updatedPosition = {
            ...position,
            note
        }

        updatePosition(updatedPosition).then(() => closeModal());
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <PositionForm portfolioId={position.portfolioId} instrumentId={position.instrumentId} note={position.note} onSubmit={handleSubmit} />
        </LoadingWrapper>
    )

}