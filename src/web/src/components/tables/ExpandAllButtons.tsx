import React from 'react';
import { COLLAPSE_ALL_ROWS_EVENT_NAME, EXPAND_ALL_ROWS_EVENT_NAME } from '../../constants';

export default function ExpandAllButtons(): JSX.Element {
    const handleExpandAllClick = () => {
        window.dispatchEvent(new Event(EXPAND_ALL_ROWS_EVENT_NAME));
    }

    const handleCollapseAllClick = () => {
        window.dispatchEvent(new Event(COLLAPSE_ALL_ROWS_EVENT_NAME));
    }

    return (
        <div className="expand-all-rows-buttons">
            <button className="btn btn-primary btn-sm mr-1" onClick={handleExpandAllClick}>Expand all</button>
            <button className="btn btn-primary btn-sm" onClick={handleCollapseAllClick}>Collapse all</button>
        </div>
    )
}