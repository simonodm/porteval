import React, { useState } from 'react';
import { Instrument } from '../../types';
import CreateInstrumentSplitForm from '../forms/CreateInstrumentSplitForm';
import ModalWrapper from '../modals/ModalWrapper';
import InstrumentSplitsTable from '../tables/InstrumentSplitsTable';

type Props = {
    instrument?: Instrument;
}

function InstrumentSplitHistory({ instrument }: Props): JSX.Element {
    const [modalIsOpen, setModalIsOpen] = useState(false);

    return (
        <>
            <div className="action-buttons">
                <button
                    className="btn btn-success btn-sm float-right"
                    onClick={() => setModalIsOpen(true)} role="button"
                >
                    Add a split
                </button>
            </div>
            <div className="row">
                <div className="col-xs-12 container-fluid">
                    <div className="content-heading">
                        <h5>Split history</h5>
                    </div>
                    {instrument && 
                        <InstrumentSplitsTable
                            instrumentId={instrument.id}
                        />
                    }
                </div>
            </div>
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