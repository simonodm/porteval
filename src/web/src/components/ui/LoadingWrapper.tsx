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
                <ReactLoading type="spin" color={constants.MAIN_COLOR} height={'32px'} width={'32px'} />
            </div>
        );
    }
    if(isError) {
        return <div className="error-wrapper">An error has occured.</div>;
    }

    return <>{ children }</>;
}