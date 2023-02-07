import React from 'react';
import PageHeading from '../ui/PageHeading';
import ImportDataForm from '../forms/ImportDataForm';
import ImportsTable from '../tables/ImportsTable';
import ExportDataForm from '../forms/ExportDataForm';
import LoadingWrapper from '../ui/LoadingWrapper';

import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

import { useGetAllInstrumentsQuery } from '../../redux/api/instrumentApi';
import { checkIsError, checkIsLoaded } from '../../utils/queries';

/**
 * Renders the data import/export view.
 * 
 * @category Views
 * @component
 */
function ImportExportView(): JSX.Element {
    const instruments = useGetAllInstrumentsQuery();

    const isLoaded = checkIsLoaded(instruments);
    const isError = checkIsError(instruments);

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <PageHeading heading="Data import and export" />
            <Container fluid>
                <Row className="mb-5">
                    <Col>
                        <h5>Export</h5>
                        <ExportDataForm instruments={instruments.data ?? []}/>
                    </Col>
                </Row>
                <Row className="mb-5">
                    <Col>
                        <h5>Import</h5>
                        <ImportDataForm />
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <ImportsTable />
                    </Col>
                </Row>
            </Container>
        </LoadingWrapper>
    )
}

export default ImportExportView;