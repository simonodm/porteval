import React from 'react';
import './PageHeading.css';
import usePageTitle from '../../hooks/usePageTitle';

type Props = {
    heading: string;
    children?: React.ReactNode;
}

export default function PageHeading({ heading, children }: Props): JSX.Element {
    usePageTitle(heading);

    return (
        <div className="heading-row">
            <h3 id="content-heading">{heading}</h3>
            {children}
        </div>
    )
}