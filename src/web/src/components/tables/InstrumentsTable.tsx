import React, { useMemo, useState } from 'react';
import PageSelector from '../ui/PageSelector';
import DataTable, { ColumnDefinition } from './DataTable';
import EditInstrumentForm from '../forms/EditInstrumentForm';
import ModalWrapper from '../modals/ModalWrapper';
import InstrumentCurrentPriceText from '../ui/InstrumentCurrentPriceText';

import { Link, NavLink } from 'react-router-dom';
import { generateDefaultInstrumentChart } from '../../utils/chart';
import { Instrument } from '../../types';
import { INSTRUMENT_TYPE_TO_STRING } from '../../constants';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { useDeleteInstrumentMutation, useGetInstrumentPageQuery } from '../../redux/api/instrumentApi';

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

    const instruments = useGetInstrumentPageQuery({ page: page, limit: pageLimit});
    const [deleteInstrument, mutationStatus] = useDeleteInstrumentMutation();

    const columnsCompact: Array<ColumnDefinition<Instrument>> = useMemo(() => [
        {
            id: 'name',
            header: 'Name',
            accessor: i => i.name,
            render: i => <Link to={`/instruments/${i.id}`}>{i.name}</Link>
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
            id: 'currentPrice',
            header: 'Current price',
            accessor: i => i.currentPrice ?? 0,
            render: i => <InstrumentCurrentPriceText instrument={i} />
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
                        role='button'
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

    const columnsFull: Array<ColumnDefinition<Instrument>> = useMemo(() => [
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
            accessor: i => i.currentPrice ?? 0,
            render: i => <InstrumentCurrentPriceText instrument={i} />
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
                        role='button'
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
                columnDefinitions={{
                    lg: columnsFull,
                    xs: columnsCompact
                }}
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
                    totalPages={instruments.data ? instruments.data.totalCount / pageLimit : 1}
                />
            </div>
            <ModalWrapper closeModal={() => setIsModalOpen(false)}
                heading={`Edit ${instrumentBeingEdited?.symbol ?? ''}`} isOpen={isModalOpen}
            >
                {
                    instrumentBeingEdited !== undefined
                        ?
                            <EditInstrumentForm
                                instrument={instrumentBeingEdited}
                                onSuccess={() => setIsModalOpen(false)}
                            />
                        : null
                }
            </ModalWrapper>
        </>
    );
}

export default InstrumentsTable;