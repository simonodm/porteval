import React, { useState } from 'react';
import CreateInstrumentPriceForm from '../forms/CreateInstrumentPriceForm';
import ModalWrapper from '../modals/ModalWrapper';
import InstrumentPricesTable from '../tables/InstrumentPricesTable';

import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';

import { Instrument } from '../../types';

type Props = {
    instrument?: Instrument;
}

function InstrumentPriceHistory({ instrument }: Props): JSX.Element {
    const [modalIsOpen, setModalIsOpen] = useState(false);

    return (
        <>
            <Container fluid>
                <Row className="mb-2">
                    <Col xs={6}>
                        <h5>Price history</h5>
                    </Col>  
                    <Col xs={6}>
                        <Button
                            variant="success"
                            size="sm"
                            className="float-right"
                            onClick={() => setModalIsOpen(true)}
                        >
                            Add a price
                        </Button>
                    </Col>
                </Row>
                <Row>
                    <Col xs={12}>
                        {instrument && 
                            <InstrumentPricesTable currencyCode={instrument.currencyCode}
                                instrumentId={instrument.id}
                            />
                        }
                    </Col>
                </Row>
            </Container>
            <ModalWrapper closeModal={() => setModalIsOpen(false)} heading="Add new price" isOpen={modalIsOpen}>
                { instrument &&
                    <CreateInstrumentPriceForm
                        instrumentId={instrument.id}
                        onSuccess={() => setModalIsOpen(false)}
                    />
                }
            </ModalWrapper>
        </>
    )
}

export default InstrumentPriceHistory;