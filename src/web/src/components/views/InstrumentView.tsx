import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import PortEvalChart from '../charts/PortEvalChart';
import ModalWrapper from '../modals/ModalWrapper';
import PageHeading from '../ui/PageHeading';
import CreateInstrumentPriceForm from '../forms/CreateInstrumentPriceForm';
import useUserSettings from '../../hooks/useUserSettings';
import InstrumentPricesTable from '../tables/InstrumentPricesTable';

import { useGetInstrumentByIdQuery } from '../../redux/api/instrumentApi';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { generateDefaultInstrumentChart } from '../../utils/chart';
import { getPriceString } from '../../utils/string';
import { useParams } from 'react-router-dom';

import './InstrumentView.css';

type Params = {
    /**
     * ID of instrument to display.
     */
    instrumentId?: string;
}

/**
 * Renders an instrument view based on query parameters.
 * 
 * @category Views
 * @component
 */
function InstrumentView(): JSX.Element {
    const params = useParams<Params>();
    const instrumentId = params.instrumentId ? parseInt(params.instrumentId) : 0;

    const instrument = useGetInstrumentByIdQuery(instrumentId);

    const [userSettings] = useUserSettings();

    const instrumentLoaded = checkIsLoaded(instrument);
    const instrumentError = checkIsError(instrument);

    const [modalIsOpen, setModalIsOpen] = useState(false);

    const chart = instrument.data ? generateDefaultInstrumentChart(instrument.data) : undefined;

    return (
        <LoadingWrapper isError={instrumentError} isLoaded={instrumentLoaded}>
            <PageHeading heading={instrument.data?.name ?? 'Instrument'}>
                { instrument.data?.isTracked  && instrument.data.lastPriceUpdate &&
                    <span className="float-right last-updated">
                        Prices updated: {
                            new Date(instrument.data.lastPriceUpdate).toLocaleString()
                        }
                    </span>
                }
            </PageHeading>
            <div className="row mb-5">
                <div className="col-xs-12 col-md-6">
                    <h5>Data</h5>
                    <table className="entity-data w-100">
                        <tbody>
                            <tr>
                                <td>Name:</td>
                                <td>{ instrument.data?.name }</td>
                            </tr>
                            <tr>
                                <td>Symbol:</td>
                                <td>{ instrument.data?.symbol }</td>
                            </tr> 
                            <tr>
                                <td>Exchange:</td>
                                <td>{ instrument.data?.exchange }</td>
                            </tr>
                            <tr>
                                <td>Currency:</td>
                                <td>{ instrument.data?.currencyCode }</td>
                            </tr>
                            <tr>
                                <td>Current price:</td>
                                <td>
                                    { 
                                        getPriceString(
                                            instrument.data?.currentPrice,
                                            instrument.data?.currencyCode,
                                            userSettings
                                        )
                                    }
                                </td>
                            </tr>
                            <tr>
                                <td>Note:</td>
                                <td>{ instrument.data?.note }</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div className="col-xs-12 col-md-6">
                    { chart && <PortEvalChart chart={chart} /> }
                </div>
            </div>
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
                    {instrument.data && 
                        <InstrumentPricesTable currencyCode={instrument.data.currencyCode}
                            instrumentId={instrument.data.id}
                        />
                    }
                </div>
            </div>
            <ModalWrapper closeModal={() => setModalIsOpen(false)} heading="Add new price" isOpen={modalIsOpen}>
                { instrument.data &&
                    <CreateInstrumentPriceForm
                        instrumentId={instrument.data.id}
                        onSuccess={() => setModalIsOpen(false)}
                    />
                }
            </ModalWrapper>
        </LoadingWrapper>
    )
}

export default InstrumentView;