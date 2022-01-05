import React from 'react';
import { useCreatePortfolioMutation } from '../../redux/api/portfolioApi';
import { ModalCallbacks } from '../../types';
import PortfolioForm from '../forms/PortfolioForm';
import { onSuccessfulResponse } from '../utils/modal';

export default function CreatePortfolioModal({ closeModal }: ModalCallbacks): JSX.Element {
    const [createPortfolio] = useCreatePortfolioMutation();

    const handleSubmit = (name: string, currencyCode: string, note: string) => {
        const portfolio = {
            name,
            currencyCode,
            note
        };

        createPortfolio(portfolio).then((val) => {
            onSuccessfulResponse(val, closeModal);
        });
    }

    return (
        <PortfolioForm onSubmit={handleSubmit} />
    )
}