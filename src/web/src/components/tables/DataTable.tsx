/* eslint-disable react/jsx-key */

import React, { useEffect, useMemo } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import DataTableExpandableComponent from './DataTableExpandableComponent';

import { Column, Row, useExpanded, useSortBy, useTable } from "react-table"
import { COLLAPSE_ALL_ROWS_EVENT_NAME, EXPAND_ALL_ROWS_EVENT_NAME } from '../../constants';

/**
 * Represents a definition of a single table column.
 * @template T Table data type
 */
export type ColumnDefinition<T extends Record<string, unknown>> = {
    /**
     * Column ID.
     */
    id: string;

    /**
     * Column header.
     */
    header: string;

    /**
     * Callback to access the specific property value displayed in this column. If this property is `undefined`, then sorting is disabled for this column.
     */
    accessor?: (data: T) => any;

    /**
     * Nested columns to be displayed. This converts this column to a grouping column, after which {@link accessor} and {@link render} properties are ignored.
     */
    columns?: Array<ColumnDefinition<T>>;

    /**
     * Callback to render the cell value.
     */
    render?: (data: T) => JSX.Element | string | null;
}

/**
 * Represents data to be displayed in the table.
 * @template T Table data type
 */
export type TableData<T extends Record<string, unknown>> = {
    /**
     * Data rows.
     */
    data: Array<T>;

    /**
     * Determines whether the data is still being loaded.
     */
    isLoading?: boolean;

    /**
     * Determines whether the data failed to load.
     */
    isError?: boolean;
}

type Props<T extends Record<string, unknown>> = {
    /**
     * Custom class name to use for the table.
     */
    className?: string;

    /**
     * An array of column definitions.
     */
    columns: ColumnDefinition<T>[];

    /**
     * Table data.
     */
    data: TableData<T>;

    /**
     * Row ID selector to map to react's `key` prop.
     */
    idSelector?: (entry: T) => string | number;

    /**
     * Determines whether sorting is enabled for this table.
     */
    sortable?: boolean;

    /**
     * Determines whether table rows can be expanded.
     */
    expandable?: boolean;

    /**
     * A callback which is invoked whenever a table row is expanded.
     * @return {JSX.Element | null} An element to render when the row is expanded.
     */
    expandElement?: (data: T) => JSX.Element | null;
};

/**
 * Converts {@link ColumnDefinition} to format accepted by `react-table`
 * 
 * @param colDef Column definition
 * @returns `react-table`'s column definition
 * @ignore
 */
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

/**
 * Generates the row expander column.
 * 
 * @returns `react-table`'s row expander column definition
 * @ignore
 */
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

/**
 * Counts the total number of data columns in the provided array of column definitions.
 * Includes grouped columns, but excludes grouping columns, ergo only the columns which map to data are counted.
 * 
 * @param columns Array of column definitions.
 * @returns Number of data columns.
 * @ignore
 */
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

/**
 * Renders a data table based on the provided data and configuration.
 * 
 * @category Tables
 * @component
 */
function DataTable<T extends Record<string, unknown>>(
    { className, columns, data, idSelector, sortable, expandable, expandElement }: Props<T>
) {
    // Column definitions need to be converted, expander column needs to be added and the result needs to be memoized
    // to work correctly with `react-table`.
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

    // add/remove expand all and collapse all event listeners on `expandable` prop change
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

export default DataTable;