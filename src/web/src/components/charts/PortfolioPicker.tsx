import React, { useContext, useState } from 'react';

import { useGetAllPortfoliosQuery } from '../../redux/api/portfolioApi';
import { Portfolio } from '../../types';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import LoadingWrapper from '../ui/LoadingWrapper';
import PortfolioPickerItem from '../ui/PortfolioPickerItem';
import ModalWrapper from '../modals/ModalWrapper';

import ChartLineConfigurationContext from '../../context/ChartLineConfigurationContext';

import ChartPortfolioConfigurator from './ChartPortfolioConfigurator';

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
            <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                {portfolios.data?.map(portfolio => {
                    const line = context.chart?.lines.find(
                        existingLine => existingLine.type === 'portfolio' && existingLine.portfolioId === portfolio.id
                    );

                    return (
                        <PortfolioPickerItem
                            key={portfolio.id}
                            line={line}
                            onConfigurePositions={() => handleConfigure(portfolio)}
                            onLineAdd={() => context.addPortfolioLine(portfolio)}
                            onLineConfigure={() => line ? context.configureLine(line) : undefined}
                            onLineRemove={() => line ? context.removeLine(line) : undefined}
                            portfolio={portfolio}
                        />
                    )
                })}
            </LoadingWrapper>
            <ModalWrapper closeModal={() => setModalIsOpen(false)} isOpen={modalIsOpen}>
                {
                    modalPortfolio && <ChartPortfolioConfigurator portfolio={modalPortfolio} />
                }
            </ModalWrapper>
        </div>
    )
}