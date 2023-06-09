import React, { useMemo } from 'react';
import useUserSettings from '../../hooks/useUserSettings';
import DataTable, { ColumnDefinition } from './DataTable';

import Button from 'react-bootstrap/Button';

import { useGetAllImportsQuery } from '../../redux/api/importApi';
import { checkIsError, checkIsLoaded } from '../../utils/queries';
import { formatDateTimeString } from '../../utils/string';
import { ImportEntry } from '../../types';

/**
 * Renders data imports' table.
 * 
 * @category Tables
 * @component
 */
function ImportsTable(): JSX.Element {
    const imports = useGetAllImportsQuery();

    const [userSettings] = useUserSettings();

    const handleLogDownload = (importEntry: ImportEntry) => {
        fetch(`/api${importEntry.errorLogUrl}`)
            .then(res => res.blob())
            .then(blob => {
                const file = URL.createObjectURL(blob);
                location.assign(file);
            })
    }

    const columnsCompact: Array<ColumnDefinition<ImportEntry>> = useMemo(() => [
        {
            id: 'time',
            header: 'Time',
            accessor: i => i.time,
            render: i => formatDateTimeString(i.time, userSettings.dateFormat + ' ' + userSettings.timeFormat)
        },
        {
            id: 'template',
            header: 'Template',
            accessor: i => i.templateType
        },
        {
            id: 'status',
            header: 'Status',
            accessor: i => i.status
        },
        {
            id: 'errorLog',
            header: 'Error log',
            render: i => (
                i.errorLogAvailable
                    ? (
                        <Button variant="primary" size="sm"
                            onClick={() => handleLogDownload(i)}
                        >Download
                        </Button>
                    )
                    : 'No error log available.'
            )
        }
    ], []);

    const columnsFull: Array<ColumnDefinition<ImportEntry>> = useMemo(() => [
        {
            id: 'time',
            header: 'Time',
            accessor: i => i.time,
            render: i => formatDateTimeString(i.time, userSettings.dateFormat + ' ' + userSettings.timeFormat)
        },
        {
            id: 'template',
            header: 'Template',
            accessor: i => i.templateType
        },
        {
            id: 'status',
            header: 'Status',
            accessor: i => i.status
        },
        {
            id: 'statusDetails',
            header: 'Status message',
            accessor: i => i.statusDetails
        },
        {
            id: 'errorLog',
            header: 'Error log',
            render: i => (
                i.errorLogAvailable
                    ? (
                        <button className="btn btn-primary btn-sm"
                            onClick={() => handleLogDownload(i)}
                        >Download
                        </button>
                    )
                    : 'No error log available.'
            )
        }
    ], []);

    const isLoaded = checkIsLoaded(imports);
    const isError = checkIsError(imports);

    return (
        <DataTable
            className="w-100 entity-list"
            sortable
            columnDefinitions={{
                lg: columnsFull,
                xs: columnsCompact
            }}
            ariaLabel="Imports table"
            data={{
                data: imports.data ?? [],
                isLoading: !isLoaded,
                isError: isError
            }}
        />
    )
}

export default ImportsTable;