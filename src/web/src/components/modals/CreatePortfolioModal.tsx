import React from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useCreatePortfolioMutation } from '../../redux/api/portfolioApi';
import { ModalCallbacks } from '../../types';
import PortfolioForm from '../forms/PortfolioForm';

export default function CreatePortfolioModal({ closeModal }: ModalCallbacks): JSX.Element {
    const [createPortfolio, mutationStatus] = useCreatePortfolioMutation();

    const isLoaded = checkIsLoaded(mutationStatus);
    const isError = checkIsError(mutationStatus);

    const handleSubmit = (name: string, currencyCode: string, note: string) => {
        const portfolio = {
            name,
            currencyCode,
            note
        };

        createPortfolio(portfolio).then(() => closeModal());
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <PortfolioForm onSubmit={handleSubmit} />
        </LoadingWrapper>
    )
}