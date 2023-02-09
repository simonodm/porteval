import React from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import PageHeading from '../ui/PageHeading';
import InstrumentPriceHistory from '../ui/InstrumentPriceHistory';
import InstrumentSplitHistory from '../ui/InstrumentSplitHistory';
import ChartPreview from '../charts/ChartPreview';
import InstrumentInformation from '../ui/InstrumentInformation';

import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

import { useGetInstrumentByIdQuery } from '../../redux/api/instrumentApi';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { generateDefaultInstrumentChart } from '../../utils/chart';
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

    const instrumentLoaded = checkIsLoaded(instrument);
    const instrumentError = checkIsError(instrument);

    const chart = instrument.data ? generateDefaultInstrumentChart(instrument.data) : undefined;

    return (
        <>
            <PageHeading heading={instrument.data?.name ?? 'Instrument'}>
                { instrument.data?.trackingStatus === 'tracked'  && instrument.data.lastPriceUpdate &&
                    <span className="d-none d-lg-inline float-right last-updated">
                        Prices updated: {
                            new Date(instrument.data.lastPriceUpdate).toLocaleString()
                        }
                    </span>
                }
            </PageHeading>
            <LoadingWrapper isError={instrumentError} isLoaded={instrumentLoaded}>
                <Container fluid className="g-0">
                    <Row className="mb-5 gy-5">
                        <Col xs={{ span: 12, order: 2 }} lg={{ span: 6, order: 1 }}>
                            { instrument.data && <InstrumentInformation instrument={instrument.data} /> }
                        </Col>
                        <Col xs={{ span: 12, order: 1 }} lg={{ span: 6, order: 1 }}>
                            { chart && <ChartPreview chart={chart} /> }
                        </Col>
                    </Row>
                    <Row className="mb-5">
                        <Col xs={12}>
                            <InstrumentSplitHistory instrument={instrument.data} />
                        </Col>
                    </Row>
                    <Row className="mb-5">
                        <Col xs={12}>
                            <InstrumentPriceHistory instrument={instrument.data} />
                        </Col>
                    </Row>
                </Container>
            </LoadingWrapper>
        </>
    )
}

export default InstrumentView;