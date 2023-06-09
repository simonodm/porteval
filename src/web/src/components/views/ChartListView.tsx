import React from 'react';
import ChartsTable from '../tables/ChartsTable';
import PageHeading from '../ui/PageHeading';
import * as constants from '../../constants';

import Container from 'react-bootstrap/Container';

import { NavLink } from 'react-router-dom';

/**
 * Renders the list of created charts view.
 * 
 * @category Views
 * @component
 */
function ChartListView(): JSX.Element {
    return (
        <>
            <PageHeading heading="Charts">
                <NavLink
                    className="btn btn-success btn-sm float-right"
                    role="button"
                    state={{chart: constants.DEFAULT_CHART}}
                    to="/charts/view"
                >Create new chart
                </NavLink>
            </PageHeading>
            <Container fluid className="g-0">
                <ChartsTable />
            </Container>            
        </>
    )
}

export default ChartListView;