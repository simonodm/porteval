import React from 'react';
import Button from 'react-bootstrap/Button';

import { COLLAPSE_ALL_ROWS_EVENT_NAME, EXPAND_ALL_ROWS_EVENT_NAME } from '../../constants';

/**
 * Renders "expand all" and "collapse all" buttons. These buttons emit an event
 * which all {@link DataTable} tables react to by expanding or collapsing all their rows.
 * 
 * @category Tables
 * @subcategory Utilities
 * @component
 */
function ExpandAllButtons(): JSX.Element {
    const handleExpandAllClick = () => {
        window.dispatchEvent(new Event(EXPAND_ALL_ROWS_EVENT_NAME));
    }

    const handleCollapseAllClick = () => {
        window.dispatchEvent(new Event(COLLAPSE_ALL_ROWS_EVENT_NAME));
    }

    return (
        <div>
            <Button variant="primary" size="sm" onClick={handleExpandAllClick}>Expand all</Button>
            <Button variant="primary" size="sm" onClick={handleCollapseAllClick}>Collapse all</Button>
        </div>                    
    )
}

export default ExpandAllButtons;