import React from 'react';
import { Link } from 'react-router-dom';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useDeleteChartMutation } from '../../redux/api/chartApi';
import { Chart } from '../../types';
import LoadingWrapper from '../ui/LoadingWrapper';

type Props = {
    chart: Chart;
}

export default function ChartRow({ chart }: Props): JSX.Element {
    const [deleteChart, mutationStatus] = useDeleteChartMutation();

    const isLoaded = checkIsLoaded(mutationStatus);
    const isError = checkIsError(mutationStatus);

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <tr>
                <td>
                    <Link to={`/charts/view/${chart.id}`}>{chart.name}</Link>
                </td>
                <td>
                    <button role="button" className="btn btn-danger btn-extra-sm" onClick={() => deleteChart(chart.id)}>Remove</button>
                </td>
            </tr>
        </LoadingWrapper>
    )
}