import React, { useMemo } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import DataTable, { ColumnDefinition } from './DataTable';

import Button from 'react-bootstrap/Button';

import { Chart } from '../../types';
import { Link } from 'react-router-dom';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { useDeleteChartMutation, useGetAllChartsQuery } from '../../redux/api/chartApi';

/**
 * Loads and renders a table of charts.
 * 
 * @category Tables
 * @component
 */
function ChartsTable(): JSX.Element {
    const charts = useGetAllChartsQuery();
    const [deleteChart, deletionStatus] = useDeleteChartMutation();

    const isLoaded = checkIsLoaded(charts, deletionStatus);
    const isError = checkIsError(charts, deletionStatus);

    const columns: Array<ColumnDefinition<Chart>> = useMemo(() => [
        {
            id: 'name',
            header: 'Name',
            accessor: c => c.name,
            render: c => <Link to={`/charts/view/${c.id}`}>{c.name}</Link>
        },
        {
            id: 'actions',
            header: 'Actions',
            render: c => (
                <Button variant="danger" className="btn-xs" onClick={() => deleteChart(c.id)}>
                    Remove
                </Button>
            )
        }
    ], []);

    return (
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <DataTable
                className="w-100 entity-list"
                sortable
                columnDefinitions={columns}
                idSelector={c => c.id}
                ariaLabel="Charts table"
                data={{
                    data: charts.data ?? [],
                    isLoading: !isLoaded,
                    isError: isError
                }}
            />
        </LoadingWrapper>
    )
}

export default ChartsTable;