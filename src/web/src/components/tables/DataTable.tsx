/* eslint-disable react/jsx-key */

import React, { useEffect, useMemo } from 'react';
import { Column, Row, useExpanded, useSortBy, useTable } from "react-table"
import { COLLAPSE_ALL_ROWS_EVENT_NAME, EXPAND_ALL_ROWS_EVENT_NAME } from '../../constants';
import LoadingWrapper from '../ui/LoadingWrapper';
import DataTableExpandableComponent from './DataTableExpandableComponent';

export type ColumnDefinition<T extends Record<string, unknown>> = {
    id: string;
    header: string;
    accessor?: (data: T) => any;
    columns?: Array<ColumnDefinition<T>>;
    render?: (data: T) => JSX.Element | string | null;
}

export type TableData<T extends Record<string, unknown>> = {
    data: Array<T>;
    isLoading?: boolean;
    isError?: boolean;
}

type Props<T extends Record<string, unknown>> = {
    className?: string;
    columns: ColumnDefinition<T>[];
    data: TableData<T>;
    idSelector?: (entry: T) => string | number;
    sortable?: boolean;
    expandable?: boolean;
    expandElement?: (data: T) => JSX.Element;
};

function convertColumnDefinition<T extends Record<string, unknown>>(colDef: ColumnDefinition<T>): Column<T> {
    const result: Column<T> = {
        id: colDef.id,
        Header: colDef.header,
        columns: colDef.columns?.map(c => convertColumnDefinition(c)),
        ...(colDef.render ? { Cell: ({ row }: { row: Row<T> }) => colDef.render!(row.original) } : undefined),
        ...(colDef.accessor ? { accessor: colDef.accessor } : undefined)
    };

    return result;
}

function getExpanderColumn<T extends Record<string, unknown>>(): Column<T> {
    return {
        Header: () => null,
        id: 'expander',
        Cell: ({ row }: { row: Row<T> }) => (
          <span {...row.getToggleRowExpandedProps()}>
            {
                row.isExpanded
                    ? <i className="bi bi-arrow-down-short"></i>
                    : <i className="bi bi-arrow-right-short"></i>
            }            
          </span>
        )
    }
}

function getColumnCount<T extends Record<string, unknown>>(columns: Array<ColumnDefinition<T>>): number {
    let result = 0;
    columns.forEach(column => {
        if(column.columns) {
            result += getColumnCount(column.columns);
        }
        else {
            result++;
        }
    });

    return result;
}

export default function DataTable<T extends Record<string, unknown>>(
    { className, columns, data, idSelector, sortable, expandable, expandElement }: Props<T>
) {
    const convertedColumns = useMemo<Array<Column<T>>>(() => {
        return [
            ...(expandable ? [getExpanderColumn<T>()] : []),
            ...columns.map(c => convertColumnDefinition(c))
        ]
    }, [columns, expandable]);

    const {
      getTableProps,
      getTableBodyProps,
      headerGroups,
      rows,
      prepareRow,
      toggleAllRowsExpanded
    } = useTable(
      {
        columns: convertedColumns,
        data: data.data,
        autoResetExpanded: false,
        collapseOnDataChange: false,
        ...(idSelector !== undefined && { getRowId: (originalRow: T) => idSelector(originalRow).toString() })
      },
      useSortBy,
      useExpanded
    );

    useEffect(() => {
        if(expandable) {
            const expandListener = () => toggleAllRowsExpanded(true);
            const collapseListener = () => toggleAllRowsExpanded(false);

            window.addEventListener(EXPAND_ALL_ROWS_EVENT_NAME, expandListener);
            window.addEventListener(COLLAPSE_ALL_ROWS_EVENT_NAME, collapseListener);

            return () => {
                window.removeEventListener(EXPAND_ALL_ROWS_EVENT_NAME, expandListener);
                window.removeEventListener(COLLAPSE_ALL_ROWS_EVENT_NAME, collapseListener);
            }
        }
    }, [expandable]);

    return (
        <table className={className ?? ''} {...getTableProps()}>
            <thead>
            {headerGroups.map(headerGroup => (
                <tr {...headerGroup.getHeaderGroupProps()}>
                {headerGroup.headers.map(column => (
                    <th {...column.getHeaderProps(sortable ? column.getSortByToggleProps() : undefined)}>
                    {column.render('Header')}
                    { sortable &&
                        <span>
                            {column.isSorted
                            ? column.isSortedDesc
                                ? ' 🔽'
                                : ' 🔼'
                            : ''}
                        </span>
                    }
                    </th>
                ))}
                </tr>
            ))}
            </thead>
            <LoadingWrapper isLoaded={!data.isLoading} isError={!!data.isError}>
                <tbody {...getTableBodyProps()}>
                    {rows.map(
                        (row) => {
                        prepareRow(row);
                        return (
                            <>
                                <tr {...row.getRowProps()}>
                                {row.cells.map(cell => {
                                    return (
                                        <td {...cell.getCellProps()}>{cell.render('Cell')}</td>
                                    )
                                })}
                                </tr>
                                {expandable && expandElement &&
                                    <DataTableExpandableComponent 
                                        originalRowColumnCount={getColumnCount(columns) + 1}
                                        render={() => expandElement(row.original)}
                                        hidden={!row.isExpanded} 
                                    />
                                }
                            </>
                        )}
                    )}
                </tbody>
            </LoadingWrapper>
        </table>
    )
  }