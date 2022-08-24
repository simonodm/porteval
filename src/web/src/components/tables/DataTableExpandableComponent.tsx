import React from 'react';

type Props = {
    originalRowColumnCount: number;
    hidden?: boolean;
    render: () => JSX.Element;
}

export default function DataTableExpandableComponent({ originalRowColumnCount, hidden, render }: Props): JSX.Element {
    return (
        <tr style={{display: hidden ? 'none' : 'table-row'}}>
            <td/>
            <td colSpan={originalRowColumnCount - 1}>
                {render()}
            </td>
        </tr>
    )
}