import React from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import ChartRow from './ChartRow';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useGetAllChartsQuery } from '../../redux/api/chartApi';

export default function ChartsTable(): JSX.Element {
    const charts = useGetAllChartsQuery();

    const isLoaded = checkIsLoaded(charts);
    const isError = checkIsError(charts);

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <table className="w-100 entity-list">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {charts.data?.map(chart => <ChartRow chart={chart} />)}
                </tbody>
            </table>
        </LoadingWrapper>
    )
}