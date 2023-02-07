import React, { useState } from 'react';
import ModalWrapper from '../modals/ModalWrapper';
import ExpandAllButtons from '../tables/ExpandAllButtons';
import PositionsTable from '../tables/PositionsTable';
import OpenPositionForm from '../forms/OpenPositionForm';

import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';

import { Portfolio } from '../../types';

type Props = {
    portfolio: Portfolio;
}

function PortfolioPositionOverview({ portfolio }: Props): JSX.Element {
    const [modalIsOpen, setModalIsOpen] = useState(false);

    return (
        <>
            <Container fluid>
                <Row className="mb-2">
                    <Col xs={6}>
                        <h5>Positions</h5>
                    </Col>
                    <Col xs={6}>
                        <Button
                            variant="success"
                            size="sm"
                            className="float-right"
                            onClick={() => setModalIsOpen(true)}
                        >
                            Open a position
                        </Button>
                    </Col>
                </Row>
                <Row>
                    <Col xs={12}>
                        <ExpandAllButtons />
                    </Col>
                </Row>
                <Row>
                    <Col xs={12}>
                        <PositionsTable className="w-100 entity-list" portfolioId={portfolio.id} />
                    </Col>
                </Row>
            </Container>
            <ModalWrapper closeModal={() => setModalIsOpen(false)} heading="Open position" isOpen={modalIsOpen}>
                <OpenPositionForm onSuccess={() => setModalIsOpen(false)} portfolioId={portfolio.id} />
            </ModalWrapper>
        </>
    )
}

export default PortfolioPositionOverview;