import React from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import InstrumentRow from './InstrumentRow';

import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useGetInstrumentPageQuery } from '../../redux/api/instrumentApi';

export default function InstrumentsTable(): JSX.Element {
    const instruments = useGetInstrumentPageQuery({ page: 1, limit: 100});
    const isLoaded = checkIsLoaded(instruments);
    const isError = checkIsError(instruments);

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <table className="entity-list w-100">
                <thead>
                <tr>
                    <th>Name</th>
                    <th>Symbol</th>
                    <th>Currency</th>
                    <th>Type</th>
                    <th>Current price</th>
                    <th>Actions</th>
                </tr>
                </thead>
                <tbody>
                    {instruments.data?.data.map(instrument => <InstrumentRow instrument={instrument} />)}
                </tbody>
            </table>
        </LoadingWrapper>
    );
}