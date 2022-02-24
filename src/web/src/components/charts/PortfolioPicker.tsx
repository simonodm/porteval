import React, { useContext, useState } from 'react';
import { useGetAllPortfoliosQuery } from '../../redux/api/portfolioApi';
import { Portfolio } from '../../types';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import LoadingWrapper from '../ui/LoadingWrapper';
import PortfolioPickerItem from '../ui/PortfolioPickerItem';
import ModalWrapper from '../modals/ModalWrapper';
import ChartPortfolioConfigurator from './ChartPortfolioConfigurator';
import ChartLineConfigurationContext from '../../context/ChartLineConfigurationContext';

export default function PortfolioPicker(): JSX.Element {
    const context = useContext(ChartLineConfigurationContext);
    const portfolios = useGetAllPortfoliosQuery();

    const isLoaded = checkIsLoaded(portfolios);
    const isError = checkIsError(portfolios);
    const [modalIsOpen, setModalIsOpen] = useState(false);
    const [modalPortfolio, setModalPortfolio] = useState<Portfolio | undefined>(undefined);

    const handleConfigure = (portfolio: Portfolio) => {
        setModalPortfolio(portfolio);
        setModalIsOpen(true);
    }

    return (
        <div className="chart-picker">
            <h6>Portfolios</h6>
            <LoadingWrapper isLoaded={isLoaded} isError={isError}>
                {portfolios.data?.map(portfolio => {
                    const line = context.chart?.lines.find(existingLine => existingLine.type === 'portfolio' && existingLine.portfolioId === portfolio.id)

                    return (
                        <PortfolioPickerItem
                            portfolio={portfolio}
                            line={line}
                            onLineAdd={() => context.addPortfolioLine(portfolio)}
                            onLineRemove={() => line ? context.removeLine(line) : undefined}
                            onLineConfigure={() => line ? context.configureLine(line) : undefined}
                            onConfigurePositions={() => handleConfigure(portfolio)}/>
                    )
                })}
            </LoadingWrapper>
            <ModalWrapper isOpen={modalIsOpen} closeModal={() => setModalIsOpen(false)}>
                {
                    modalPortfolio && <ChartPortfolioConfigurator portfolio={modalPortfolio} />
                }
            </ModalWrapper>
        </div>
    )
}