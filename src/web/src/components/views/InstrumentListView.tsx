import React, { useState, Fragment } from 'react';

import InstrumentsTable from '../tables/InstrumentsTable';
import ModalWrapper from '../modals/ModalWrapper';
import PageHeading from '../ui/PageHeading';
import CreateInstrumentForm from '../forms/CreateInstrumentForm';

export default function InstrumentListView(): JSX.Element {
    const [modalIsOpen, setModalIsOpen] = useState(false);

    return (
        <Fragment>
            <PageHeading heading="Instruments">
                <button
                    className="btn btn-success btn-sm float-right"
                    onClick={() => setModalIsOpen(true)}
                >
                    Create new instrument
                </button>
            </PageHeading>
            <div className="col-xs-12 container-fluid">
                <InstrumentsTable />
            </div>
            <ModalWrapper closeModal={() => setModalIsOpen(false)} isOpen={modalIsOpen}>
                <CreateInstrumentForm onSuccess={() => setModalIsOpen(false)} />
            </ModalWrapper>
        </Fragment>
    )
}