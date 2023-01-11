import React, { useEffect, useState } from 'react';
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

    /**
     * Determines whether the loading animation should be displayed when the wrapper gets reloaded.
     */
    displayLoadingOnQueryRefresh?: boolean;
}

/**
 * A wrapper for content being asynchronously loaded. Displays a spinning wheel when loading.
 * 
 * @category UI
 * @component
 */
function LoadingWrapper({ children, isLoaded, isError = false, displayLoadingOnQueryRefresh = false}: Props): JSX.Element {   
    const [isFirstLoad, setIsFirstLoad] = useState(true);

    useEffect(() => {
        if(isLoaded && isFirstLoad) {
            setIsFirstLoad(false);
        }
    }, [isLoaded])

    if(!isLoaded && (isFirstLoad || displayLoadingOnQueryRefresh)) {
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
