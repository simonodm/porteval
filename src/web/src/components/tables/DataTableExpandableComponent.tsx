import React from 'react';

type Props = {
    /**
     * Number of data columns in the parent row.
     * This property is required to properly align the expandable component to the width of the parent row.
     */
    originalRowColumnCount: number;

    /**
     * Determines whether the expanded content is hidden.
     */
    hidden?: boolean;

    /**
     * Callback to render the expanded content.
     */
    render: () => JSX.Element | null;
}

/**
 * A wrapper for the expandable row content.
 * 
 * @category Tables
 * @component
 */
function DataTableExpandableComponent({ originalRowColumnCount, hidden, render }: Props): JSX.Element {
    return (
        <tr style={{display: hidden ? 'none' : 'table-row'}}>
            <td/>
            <td colSpan={originalRowColumnCount - 1}>
                {render()}
            </td>
        </tr>
    )
}

export default DataTableExpandableComponent;