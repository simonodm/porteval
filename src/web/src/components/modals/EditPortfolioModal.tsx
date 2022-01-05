import React from 'react';
import { useUpdatePortfolioMutation } from '../../redux/api/portfolioApi';
import { ModalCallbacks, Portfolio } from '../../types';
import PortfolioForm from '../forms/PortfolioForm';
import { onSuccessfulResponse } from '../utils/modal';

type Props = {
    portfolio: Portfolio;
} & ModalCallbacks

export default function EditPortfolioModal({ portfolio, closeModal }: Props): JSX.Element {
    const [updatePortfolio] = useUpdatePortfolioMutation();

    const handleSubmit = (name: string, currencyCode: string, note: string) => {
        const updatedPortfolio = {
            ...portfolio,
            name,
            currencyCode,
            note
        };

        updatePortfolio(updatedPortfolio).then((val) => {
            onSuccessfulResponse(val, closeModal);
        });
    }

    return (
        <PortfolioForm name={portfolio.name} currencyCode={portfolio.currencyCode} note={portfolio.note} onSubmit={handleSubmit} />
    )
}