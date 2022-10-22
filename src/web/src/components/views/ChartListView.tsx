import React from 'react';
import ChartsTable from '../tables/ChartsTable';
import PageHeading from '../ui/PageHeading';
import * as constants from '../../constants';

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
                    className="btn btn-success btn-sm float-right mr-1"
                    role="button"
                    to={{pathname: '/charts/view', state: {chart: constants.DEFAULT_CHART}}}
                >Create new chart
                </NavLink>
            </PageHeading>
            <ChartsTable />
        </>
    )
}

export default ChartListView;