import React from 'react';
import './Header.css';

export default function Header(): JSX.Element {
    return (
    <nav className="navbar navbar-dark bg-dark p-0">
        <span id="app-logo">
            <a className="navbar-brand" href="/">PortEval</a>
        </span>
    </nav>
    )
}