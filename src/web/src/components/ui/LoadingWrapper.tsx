import React from 'react';
import LoadingSpinner from './LoadingSpinner';
import './LoadingWrapper.css';

type Props = {
    /**
     * Wrapped content.
     */
    children?: React.ReactNode;

    /**
     * Determines whether loading was successfully finished.
     */
    isLoaded: boolean;

    /**
     * Determines whether loading failed.
     */
    isError?: boolean;
}

/**
 * A wrapper for content being asynchronously loaded. Displays a spinning wheel when loading.
 * 
 * @category UI
 * @component
 */
function LoadingWrapper({ children, isLoaded, isError = false}: Props): JSX.Element {   
    if(!isLoaded) {
        return (
            <div className="loading-wrapper">
                <LoadingSpinner />
            </div>
        );
    }
    if(isError) {
        return <div className="error-wrapper">An error has occured.</div>;
    }

    return (
        // eslint-disable-next-line react/jsx-no-useless-fragment
        <>
            { children }
        </>
    );
}

export default LoadingWrapper;
