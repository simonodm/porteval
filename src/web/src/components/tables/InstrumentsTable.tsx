import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import InstrumentRow from './InstrumentRow';

import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useGetInstrumentPageQuery, usePrefetch } from '../../redux/api/instrumentApi';
import PageSelector from '../ui/PageSelector';

export default function InstrumentsTable(): JSX.Element {
    const [page, setPage] = useState(1);
    const [pageLimit] = useState(30);

    const prefetchInstruments = usePrefetch('getInstrumentPage');
    const instruments = useGetInstrumentPageQuery({ page: page, limit: pageLimit});
    const isLoaded = checkIsLoaded(instruments);
    const isError = checkIsError(instruments);

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <div className="col-xs-12 container-fluid">
                <table className="entity-list w-100">
                    <thead>
                    <tr>
                        <th>Name</th>
                        <th>Symbol</th>
                        <th>Currency</th>
                        <th>Type</th>
                        <th>Current price</th>
                        <th>Note</th>
                        <th>Actions</th>
                    </tr>
                    </thead>
                    <tbody>
                        {instruments.data?.data.map(instrument => <InstrumentRow instrument={instrument} />)}
                    </tbody>
                </table>
                <div className="float-right">
                    <PageSelector
                        page={page}
                        totalPages={instruments.data ? instruments.data.totalCount / pageLimit : 1}
                        onPageChange={(p) => setPage(p)}
                        prefetch={(p) => prefetchInstruments({ page: p, limit: pageLimit })}
                        />
                </div>
            </div>
        </LoadingWrapper>
    );
}