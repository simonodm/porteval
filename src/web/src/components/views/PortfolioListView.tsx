import React, { Fragment, useState } from 'react';
import PortfoliosTable from '../tables/PortfoliosTable';
import ModalWrapper from '../modals/ModalWrapper';
import PageHeading from '../ui/PageHeading';
import CreatePortfolioForm from '../forms/CreatePortfolioForm';

function PortfolioListView(): JSX.Element {
    const [modalIsOpen, setModalIsOpen] = useState(false);

    return (
        <Fragment>
            <PageHeading heading="Portfolios">
                <button role="button" className="btn btn-success btn-sm float-right" onClick={() => setModalIsOpen(true)}>Create new portfolio</button>
            </PageHeading>
            <div className="col-xs-12 container-fluid">
                <PortfoliosTable />
            </div>
            <ModalWrapper isOpen={modalIsOpen} closeModal={() => setModalIsOpen(false)}>
                <CreatePortfolioForm onSuccess={() => setModalIsOpen(false)} />
            </ModalWrapper>
        </Fragment>
    )
}

export default PortfolioListView;