import React from 'react';
import { useDispatch } from 'react-redux';
import { portEvalApi } from '../redux/api/portEvalApi';
import './Header.css';

/**
 * Renders the application header.
 * 
 * @component
 */
function Header(): JSX.Element {
    const dispatch = useDispatch();

    return (
        <nav className="navbar navbar-dark bg-dark p-0">
            <span id="app-logo">
                <a className="navbar-brand" href="/">PortEval</a>
            </span>
            <span className="float-right mr-1">
                <button className="btn btn-primary btn-sm" onClick={() => dispatch(portEvalApi.util.resetApiState())}>
                    <i className="bi bi-arrow-clockwise"></i>
                </button>
            </span>
        </nav>
    )
}

export default Header;