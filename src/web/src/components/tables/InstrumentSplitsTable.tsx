import React, { useMemo } from 'react';
import useUserSettings from '../../hooks/useUserSettings';
import { useGetInstrumentSplitsQuery, useUpdateInstrumentSplitMutation } from '../../redux/api/instrumentApi';
import { InstrumentSplit } from '../../types';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { formatDateTimeString } from '../../utils/string';
import DataTable, { ColumnDefinition } from './DataTable';

type Props = {
    instrumentId: number;
}

function InstrumentSplitsTable({ instrumentId }: Props): JSX.Element {
    const splits = useGetInstrumentSplitsQuery(instrumentId);
    const [updateSplit, mutationStatus] = useUpdateInstrumentSplitMutation();

    const [userSettings] = useUserSettings();

    const handleSplitRollback = (split: InstrumentSplit) => {
        if(split.status !== 'processed') {
            return;
        }

        updateSplit({
            ...split,
            status: 'rollbackRequested'
        });
    }

    const isLoaded = checkIsLoaded(splits, mutationStatus);
    const isError = checkIsError(splits, mutationStatus);

    const columns: Array<ColumnDefinition<InstrumentSplit>> = useMemo(() => [
        {
            id: 'date',
            header: 'Date',
            accessor: s => s.time,
            render: s => formatDateTimeString(s.time, userSettings.dateFormat + ' ' + userSettings.timeFormat)
        },
        {
            id: 'ratio',
            header: 'Ratio',
            accessor: s => s.splitRatioNumerator / s.splitRatioDenominator,
            render: s => `${s.splitRatioNumerator}:${s.splitRatioDenominator}`
        },
        {
            id: 'status',
            header: 'Status',
            accessor: s => s.status
        },
        {
            id: 'actions',
            header: 'Actions',
            render: s => (
                <>
                    {s.status === 'processed'
                        && <button
                            className="btn btn-primary btn-extra-sm mr-1"
                            onClick={() => {
                                handleSplitRollback(s)
                            }}
                            role="button"
                        >Rollback
                        </button>
                    }
                </>
            )
        },
    ], []);

    return (
        <DataTable
            className="w-100 entity-list"
            sortable
            columns={columns}
            data={{
                data: splits.data ?? [],
                isLoading: !isLoaded,
                isError: isError
            }}
        />
    );
}

export default InstrumentSplitsTable;