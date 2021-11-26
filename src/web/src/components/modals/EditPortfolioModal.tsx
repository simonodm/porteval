import React from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useUpdatePortfolioMutation } from '../../redux/api/portfolioApi';
import { ModalCallbacks, Portfolio } from '../../types';
import PortfolioForm from '../forms/PortfolioForm';

type Props = {
    portfolio: Portfolio;
} & ModalCallbacks

export default function EditPortfolioModal({ portfolio, closeModal }: Props): JSX.Element {
    const [updatePortfolio, mutationStatus] = useUpdatePortfolioMutation();

    const isLoaded = checkIsLoaded(mutationStatus);
    const isError = checkIsError(mutationStatus);

    const handleSubmit = (name: string, currencyCode: string, note: string) => {
        const updatedPortfolio = {
            ...portfolio,
            name,
            currencyCode,
            note
        };

        updatePortfolio(updatedPortfolio).then(() => closeModal());
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <PortfolioForm name={portfolio.name} currencyCode={portfolio.currencyCode} note={portfolio.note} onSubmit={handleSubmit} />
        </LoadingWrapper>
    )
}