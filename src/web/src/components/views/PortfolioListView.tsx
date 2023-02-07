import React, { Fragment, useState } from 'react';
import PortfoliosTable from '../tables/PortfoliosTable';
import ModalWrapper from '../modals/ModalWrapper';
import PageHeading from '../ui/PageHeading';
import CreatePortfolioForm from '../forms/CreatePortfolioForm';

import Container from 'react-bootstrap/Container';
import Button from 'react-bootstrap/Button';

/**
 * Renders the portfolio list view.
 * 
 * @category Views
 * @component
 */
function PortfolioListView(): JSX.Element {
    const [modalIsOpen, setModalIsOpen] = useState(false);

    return (
        <Fragment>
            <PageHeading heading="Portfolios">
                <Button
                    variant="success"
                    size="sm"
                    className="float-right"
                    onClick={() => setModalIsOpen(true)}
                >
                    Create new portfolio
                </Button>
            </PageHeading>
            <Container fluid className="g-0">
                <PortfoliosTable />
            </Container>
            <ModalWrapper closeModal={() => setModalIsOpen(false)} heading="Create new portfolio" isOpen={modalIsOpen}>
                <CreatePortfolioForm onSuccess={() => setModalIsOpen(false)} />
            </ModalWrapper>
        </Fragment>
    )
}

export default PortfolioListView;