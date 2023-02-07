/* eslint-disable react/jsx-key */

import React, { useEffect, useMemo } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import DataTableExpandableComponent from './DataTableExpandableComponent';

import { Column, Row, useExpanded, useSortBy, useTable } from 'react-table'
import { COLLAPSE_ALL_ROWS_EVENT_NAME, EXPAND_ALL_ROWS_EVENT_NAME, RESPONSIVE_BREAKPOINTS } from '../../constants';
import { useBreakpoint } from 'use-breakpoint';

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
     * Callback to access the specific property value displayed in this column.
     * If this property is `undefined`, then sorting is disabled for this column.
     */
    accessor?: (data: T) => unknown;

    /**
     * Nested columns to be displayed.
     * 
     * This converts this column to a grouping column,
     * after which {@link accessor} and {@link render} properties are ignored.
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

/**
 * Available breakpoints.
 */
type Breakpoint = keyof typeof RESPONSIVE_BREAKPOINTS;

/**
 * Represents a set of column definitions to be applied on specific breakpoints.
 * @template T Table data type
 */
type ResponsiveColumnDefinitions<T extends Record<string, unknown>>
    = Partial<Record<Breakpoint, ColumnDefinition<T>[]>>;

type Props<T extends Record<string, unknown>> = {
    /**
     * Custom class name to use for the table.
     */
    className?: string;

    /**
     * Columns to be displayed in the table.
     */
    columnDefinitions: ColumnDefinition<T>[] | ResponsiveColumnDefinitions<T>;

    /**
     * Table data.
     */
    data: TableData<T>;

    /**
     * Row ID selector to map to react's `key` prop.
     */
    idSelector?: (entry: T) => string | number;

    /**
     * Table's aria-label attribute.
     */
    ariaLabel?: string;

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
 * A type guard which determines whether the provided parameter is of type {@link ResponsiveColumnDefinitions<T>}.
 * @param obj Object to check type of.
 */
function isResponsiveColumnDefinitions<T extends Record<string, unknown>>(
    obj: unknown
): obj is ResponsiveColumnDefinitions<T> {
    return obj !== null
        && obj !== undefined
        && typeof obj === 'object'
        && Object.keys(obj)
            .every(breakpoint => Object.keys(RESPONSIVE_BREAKPOINTS).includes(breakpoint));
}

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
        ...(colDef.accessor ? { accessor: colDef.accessor } : undefined),
        sortType: 'basic'
    };

    return result;
}

function findColumnDefinitionsForBreakpoint<T extends Record<string, unknown>>(
    responsiveDefinitions: ResponsiveColumnDefinitions<T>,
    currentBreakpoint: Breakpoint
): ColumnDefinition<T>[] {
    // Object.keys() will keep the order of key definitions, so it's safe to use here
    const breakpointArray = Object.keys(RESPONSIVE_BREAKPOINTS);
    let currentBreakpointSeen = false;
    let result: ColumnDefinition<T>[] = [];

    breakpointArray.forEach(breakpoint => {
        if(!currentBreakpointSeen && Object.prototype.hasOwnProperty.call(responsiveDefinitions, breakpoint)) {
            const currentBreakpointColumnDefinitions
                = responsiveDefinitions[breakpoint as Breakpoint];
            if(currentBreakpointColumnDefinitions !== undefined) {
                result = currentBreakpointColumnDefinitions;
            }
        }

        currentBreakpointSeen = currentBreakpointSeen || breakpoint === currentBreakpoint;
    });

    return result;
}   

/**
 * Determines column definitions to use for the specified breakpoint.
 * 
 * @param definitions Column definitions provided as a prop to {@link DataTable}
 * @param currentBreakpoint Current screen breakpoint
 * @returns Definitions of columns for the provided breakpoint if the provided definitions are responsive,
 * original definitions otherwise.
 */
function preprocessColumnDefinitions<T extends Record<string, unknown>>(
    definitions: ColumnDefinition<T>[] | ResponsiveColumnDefinitions<T>,
    currentBreakpoint: Breakpoint
): ColumnDefinition<T>[] {
    if(isResponsiveColumnDefinitions<T>(definitions)) {
        return findColumnDefinitionsForBreakpoint(definitions, currentBreakpoint);        
    }

    return definitions;
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
            <span {...row.getToggleRowExpandedProps()} data-testid="expander">
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
        } else {
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
    { className, columnDefinitions, data, idSelector, ariaLabel, sortable, expandable, expandElement }: Props<T>
) {
    const { breakpoint } = useBreakpoint(RESPONSIVE_BREAKPOINTS, 'lg');

    const columns = preprocessColumnDefinitions(columnDefinitions, breakpoint);

    // Column definitions need to be converted, expander column needs to be added and the result needs to be memoized
    // to work correctly with `react-table`.
    const convertedColumns = useMemo<Array<Column<T>>>(() => {
        return [
            ...(expandable ? [getExpanderColumn<T>()] : []),
            ...columns.map(convertColumnDefinition)
        ]
    }, [breakpoint, columnDefinitions, expandable]);

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
        <table className={className ?? ''} aria-label={ariaLabel} {...getTableProps()}>
            <thead>
                {headerGroups.map(headerGroup => (
                    <tr {...headerGroup.getHeaderGroupProps()}>
                        {headerGroup.headers.map(column => (
                            <th {...column.getHeaderProps(
                                    sortable
                                        ? column.getSortByToggleProps({ title: undefined })
                                        : undefined
                                )}
                            >
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
                                    <tr {...row.getRowProps()} data-testid="datarow">
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
                            )
                    })}
                </tbody>
            </LoadingWrapper>
        </table>
    )
}

export default DataTable;