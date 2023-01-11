import React, { useState } from 'react';
import { Instrument } from '../../types';
import CreateInstrumentPriceForm from '../forms/CreateInstrumentPriceForm';
import ModalWrapper from '../modals/ModalWrapper';
import InstrumentPricesTable from '../tables/InstrumentPricesTable';

type Props = {
    instrument?: Instrument;
}

function InstrumentPriceHistory({ instrument }: Props): JSX.Element {
    const [modalIsOpen, setModalIsOpen] = useState(false);

    return (
        <>
            <div className="action-buttons">
                <button
                    className="btn btn-success btn-sm float-right"
                    onClick={() => setModalIsOpen(true)} role="button"
                >
                    Add price
                </button>
            </div>
            <div className="row">
                <div className="col-xs-12 container-fluid">
                    <div className="content-heading">
                        <h5>Price history</h5>
                    </div>
                    {instrument && 
                        <InstrumentPricesTable currencyCode={instrument.currencyCode}
                            instrumentId={instrument.id}
                        />
                    }
                </div>
            </div>
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