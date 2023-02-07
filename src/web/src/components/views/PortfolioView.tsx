import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import PageHeading from '../ui/PageHeading';
import ModalWrapper from '../modals/ModalWrapper';
import OpenPositionForm from '../forms/OpenPositionForm';
import ChartPreview from '../charts/ChartPreview';
import PortfolioInformation from '../ui/PortfolioInformation';
import PortfolioPositionOverview from '../ui/PortfolioPositionOverview';

import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

import { useParams } from 'react-router-dom';
import { generateDefaultPortfolioChart } from '../../utils/chart';
import {
    useGetPortfolioByIdQuery,
    useGetPortfolioCurrentValueQuery,
    useGetPortfolioStatisticsQuery
} from '../../redux/api/portfolioApi';
import { checkIsLoaded, checkIsError } from '../../utils/queries';

type Params = {
    /**
     * ID of portfolio to display.
     */
    portfolioId?: string;
}

/**
 * Renders a portfolio view based on query parameters.
 * 
 * @category Views
 * @component
 */
function PortfolioView(): JSX.Element {
    const params = useParams<Params>();
    const portfolioId = params.portfolioId ? parseInt(params.portfolioId) : 0;
    const portfolio = useGetPortfolioByIdQuery(portfolioId);

    const [modalIsOpen, setModalIsOpen] = useState(false);
    
    const value = useGetPortfolioCurrentValueQuery(portfolioId);
    const stats = useGetPortfolioStatisticsQuery(portfolioId);

    const isLoaded = checkIsLoaded(portfolio, stats, value);
    const isError = checkIsError(portfolio, stats, value);

    const chart = portfolio.data ? generateDefaultPortfolioChart(portfolio.data) : undefined;

    return (
        <>
            <PageHeading heading={portfolio.data?.name ?? 'Portfolio'} />
            <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                <Container fluid>
                    <Row className="mb-5 gy-5">
                        <Col xs={{ span: 12, order: 2}} lg={{ span: 6, order: 1}}>
                            {
                                portfolio.data && value.data && stats.data &&
                                    <PortfolioInformation
                                        portfolio={portfolio.data}
                                        value={value.data.value}
                                        stats={stats.data}
                                    />
                            }
                        </Col>
                        <Col xs={{ span: 12, order: 1}} lg={{span: 6, order: 2}}>
                            { chart && <ChartPreview chart={chart} /> }
                        </Col>
                    </Row>
                    <Row>
                        { portfolio.data && <PortfolioPositionOverview portfolio={portfolio.data} />}         
                    </Row>
                </Container>
                <ModalWrapper closeModal={() => setModalIsOpen(false)} heading="Open position" isOpen={modalIsOpen}>
                    <OpenPositionForm onSuccess={() => setModalIsOpen(false)} portfolioId={portfolioId} />
                </ModalWrapper>
            </LoadingWrapper>
        </>
    )
}

export default PortfolioView;