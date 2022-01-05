import React from 'react';
import { useUpdatePositionMutation } from '../../redux/api/positionApi';
import { ModalCallbacks, Position } from '../../types';
import PositionForm from '../forms/PositionForm';
import { onSuccessfulResponse } from '../utils/modal';

type Props = {
    position: Position;
} & ModalCallbacks;

export default function EditPositionModal({ position, closeModal }: Props): JSX.Element {
    const [updatePosition] = useUpdatePositionMutation();

    const handleSubmit = (portfolioId: number, instrumentId: number, note: string) => {
        const updatedPosition = {
            ...position,
            note
        }

        updatePosition(updatedPosition).then((val) => {
            onSuccessfulResponse(val, closeModal);
        });
    }

    return (
        <PositionForm portfolioId={position.portfolioId} instrumentId={position.instrumentId} note={position.note} onSubmit={handleSubmit} />
    )

}