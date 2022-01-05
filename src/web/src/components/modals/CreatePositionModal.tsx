import React from 'react';
import { useAddPositionMutation } from '../../redux/api/positionApi';
import { ModalCallbacks } from '../../types';
import PositionForm from '../forms/PositionForm';
import { onSuccessfulResponse } from '../utils/modal';

type Props = {
    portfolioId?: number;
} & ModalCallbacks;

export default function CreatePositionModal({ portfolioId, closeModal }: Props): JSX.Element {
    const [createPosition] = useAddPositionMutation();

    const handleSubmit = (portfolioId: number, instrumentId: number, note: string) => {
        const position = {
            portfolioId,
            instrumentId,
            note
        };

        createPosition(position).then((val) => {
            onSuccessfulResponse(val, closeModal);
        });
    }

    return (
        <PositionForm portfolioId={portfolioId} onSubmit={handleSubmit} />
    )

}