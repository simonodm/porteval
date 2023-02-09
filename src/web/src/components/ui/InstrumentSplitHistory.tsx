import React, { useState } from 'react';
import CreateInstrumentSplitForm from '../forms/CreateInstrumentSplitForm';
import ModalWrapper from '../modals/ModalWrapper';
import InstrumentSplitsTable from '../tables/InstrumentSplitsTable';

import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';

import { Instrument } from '../../types';

type Props = {
    /**
     * Instrument to display split history of.
     */
    instrument?: Instrument;
}

/**
 * Renders the instrument's split history with a button allowing to add a new split.
 * 
 * @category UI
 * @component
 */
function InstrumentSplitHistory({ instrument }: Props): JSX.Element {
    const [modalIsOpen, setModalIsOpen] = useState(false);

    return (
        <>
            <Container fluid>
                <Row className="mb-2">
                    <Col xs={6}>
                        <h5>Split history</h5>
                    </Col>
                    <Col xs={6}>
                        <Button
                            variant="success"
                            size="sm"
                            className="float-right"
                            onClick={() => setModalIsOpen(true)}
                        >
                            Add a split
                        </Button>
                    </Col>
                </Row>
                <Row>
                    <Col xs={12}>
                        {instrument && 
                            <InstrumentSplitsTable
                                instrumentId={instrument.id}
                            />
                        }
                    </Col>
                </Row>
            </Container>
            <ModalWrapper closeModal={() => setModalIsOpen(false)} heading="Add a split" isOpen={modalIsOpen}>
                { instrument &&
                    <CreateInstrumentSplitForm
                        instrumentId={instrument.id}
                        onSuccess={() => setModalIsOpen(false)}
                    />
                }
            </ModalWrapper>
        </>
    )
}

export default InstrumentSplitHistory;