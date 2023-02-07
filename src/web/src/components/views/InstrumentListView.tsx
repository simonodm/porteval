import React, { useState, Fragment } from 'react';
import InstrumentsTable from '../tables/InstrumentsTable';
import ModalWrapper from '../modals/ModalWrapper';
import PageHeading from '../ui/PageHeading';
import CreateInstrumentForm from '../forms/CreateInstrumentForm';

import Container from 'react-bootstrap/Container';
import Button from 'react-bootstrap/Button';

/**
 * Renders the instrument list view.
 * 
 * @category Views
 * @component
 */
function InstrumentListView(): JSX.Element {
    const [modalIsOpen, setModalIsOpen] = useState(false);

    return (
        <Fragment>
            <PageHeading heading="Instruments">
                <Button
                    variant="success"
                    size="sm"
                    className="float-right"
                    onClick={() => setModalIsOpen(true)}
                >
                    Create new instrument
                </Button>
            </PageHeading>
            <Container fluid className="g-0">
                <InstrumentsTable />
            </Container>
            <ModalWrapper closeModal={() => setModalIsOpen(false)} heading="Create new instrument" isOpen={modalIsOpen}>
                <CreateInstrumentForm onSuccess={() => setModalIsOpen(false)} />
            </ModalWrapper>
        </Fragment>
    )
}

export default InstrumentListView;