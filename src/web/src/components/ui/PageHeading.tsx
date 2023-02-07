import React from 'react';
import usePageTitle from '../../hooks/usePageTitle';
import './PageHeading.css';

type Props = {
    /**
     * Header to display.
     */
    heading: string;

    /**
     * Additional content to display as part of the page header.
     */
    children?: React.ReactNode;
}

/**
 * Renders a page header and changes page title to the specified header.
 * 
 * @category UI
 * @component
 */
function PageHeading({ heading, children }: Props): JSX.Element {
    usePageTitle(heading);

    return (
        <div className="heading-row">
            <h3 id="content-heading">{heading}</h3>
            <div>
                {children}
            </div>
        </div>
    )
}

export default PageHeading;