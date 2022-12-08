import React, { useMemo, useState } from 'react';
import PageSelector from '../ui/PageSelector';
import DataTable, { ColumnDefinition } from './DataTable';
import useUserSettings from '../../hooks/useUserSettings';
import EditInstrumentForm from '../forms/EditInstrumentForm';
import ModalWrapper from '../modals/ModalWrapper';

import { Link, NavLink } from 'react-router-dom';
import { generateDefaultInstrumentChart } from '../../utils/chart';
import { Instrument } from '../../types';
import { INSTRUMENT_TYPE_TO_STRING } from '../../constants';
import { getPriceString } from '../../utils/string';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { useDeleteInstrumentMutation, useGetInstrumentPageQuery, usePrefetch } from '../../redux/api/instrumentApi';

/**
 * Loads instruments and renders a paginated instruments table.
 * 
 * @category Tables
 * @component
 */
function InstrumentsTable(): JSX.Element {
    const [page, setPage] = useState(1);
    const [pageLimit] = useState(30);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [instrumentBeingEdited, setInstrumentBeingEdited] = useState<Instrument | undefined>(undefined);

    const [userSettings] = useUserSettings();

    const prefetchInstruments = usePrefetch('getInstrumentPage');
    const instruments = useGetInstrumentPageQuery({ page: page, limit: pageLimit});
    const [deleteInstrument, mutationStatus] = useDeleteInstrumentMutation();

    const columns: Array<ColumnDefinition<Instrument>> = useMemo(() => [
        {
            id: 'name',
            header: 'Name',
            accessor: i => i.name,
            render: i => <Link to={`/instruments/${i.id}`}>{i.name}</Link>
        },
        {
            id: 'exchange',
            header: 'Exchange',
            accessor: i => i.exchange
        },
        {
            id: 'symbol',
            header: 'Symbol',
            accessor: i => i.symbol
        },
        {
            id: 'currency',
            header: 'Currency',
            accessor: i => i.currencyCode
        },
        {
            id: 'type',
            header: 'Type',
            accessor: i => i.type,
            render: i => INSTRUMENT_TYPE_TO_STRING[i.type]
        },
        {
            id: 'currentPrice',
            header: 'Current price',
            accessor: i => getPriceString(i.currentPrice, i.currencyCode, userSettings)
        },
        {
            id: 'note',
            header: 'Note',
            accessor: i => i.note
        },
        {
            id: 'actions',
            header: 'Actions',
            render: i => (
                <>
                    <NavLink
                        className="btn btn-primary btn-extra-sm mr-1"
                        to={{
                            pathname: '/charts/view', state: { chart: generateDefaultInstrumentChart(i) 
                        }}}
                    >Chart
                    </NavLink>
                    <button
                        className="btn btn-primary btn-extra-sm mr-1"
                        onClick={() => {
                            setInstrumentBeingEdited(i);
                            setIsModalOpen(true);
                        }}
                        role="button"
                    >Edit
                    </button>
                    <button
                        className="btn btn-danger btn-extra-sm"
                        onClick={() => deleteInstrument(i.id)}
                        role="button"
                    >Remove
                    </button>
                </>
            )
        },
    ], []);

    const isLoaded = checkIsLoaded(instruments, mutationStatus);
    const isError = checkIsError(instruments, mutationStatus);

    return (
        <>
            <DataTable
                className="w-100 entity-list"
                sortable
                columns={columns}
                idSelector={i => i.id}
                ariaLabel="Instruments table"
                data={{
                    data: instruments.data?.data ?? [],
                    isLoading: !isLoaded,
                    isError: isError
                }} 
            />
            <div className="float-right">
                <PageSelector
                    onPageChange={(p) => setPage(p)}
                    page={page}
                    prefetch={(p) => prefetchInstruments({ page: p, limit: pageLimit })}
                    totalPages={instruments.data ? instruments.data.totalCount / pageLimit : 1}
                />
            </div>
            <ModalWrapper closeModal={() => setIsModalOpen(false)}
                heading={`Edit ${instrumentBeingEdited?.symbol ?? ''}`} isOpen={isModalOpen}
            >
                {
                    instrumentBeingEdited !== undefined
                        ? <EditInstrumentForm instrument={instrumentBeingEdited} onSuccess={() => setIsModalOpen(false)} />
                        : null
                }
            </ModalWrapper>
        </>
    );
}

export default InstrumentsTable;