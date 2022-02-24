import React from 'react';
import ReactLoading from 'react-loading';

import * as constants from '../../constants';
import './LoadingWrapper.css';

type Props = {
    children?: React.ReactNode;
    isLoaded: boolean;
    isError?: boolean;
}

export default function LoadingWrapper({ children, isLoaded, isError = false}: Props): JSX.Element {   
    if(!isLoaded) {
        return (
            <div className="loading-wrapper">
                <ReactLoading color={constants.MAIN_COLOR} height="32px" type="spin"
                    width="32px"
                />
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