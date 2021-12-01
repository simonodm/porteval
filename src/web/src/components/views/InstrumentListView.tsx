import React, { useState, Fragment } from 'react';
import InstrumentsTable from '../tables/InstrumentsTable';
import CreateInstrumentModal from '../modals/CreateInstrumentModal';
import ModalWrapper from '../modals/ModalWrapper';
import PageHeading from '../ui/PageHeading';

export default function InstrumentListView(): JSX.Element {
    const [modalIsOpen, setModalIsOpen] = useState(false);

    return (
        <Fragment>
            <PageHeading heading="Instruments">
                <button className="btn btn-success btn-sm float-right" onClick={() => setModalIsOpen(true)}>Create new instrument</button>
            </PageHeading>
            <div className="col-xs-12 container-fluid">
                <InstrumentsTable />
            </div>
            <ModalWrapper isOpen={modalIsOpen} closeModal={() => setModalIsOpen(false)}>
                <CreateInstrumentModal closeModal={() => setModalIsOpen(false)} />
            </ModalWrapper>
        </Fragment>
    )
}