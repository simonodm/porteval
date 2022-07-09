import './Sidebar.css';
import { NavLink } from 'react-router-dom';
import React from 'react';


export default function Sidebar(): JSX.Element {
    return (
        <nav className="bg-dark" id="sidebar">
            <ul className="list-unstyled">
                <li>
                    <NavLink activeClassName="active" to="/dashboard">Dashboard</NavLink>
                </li>
                <li>
                    <NavLink activeClassName="active" to="/portfolios">Portfolios</NavLink>
                </li>
                <li>
                    <NavLink activeClassName="active" to="/instruments">Instruments</NavLink>
                </li>
                <li>
                    <NavLink activeClassName="active" to="/charts">Charts</NavLink>
                </li>
                <li>
                    <NavLink activeClassName="active" to="/currencies">Currencies</NavLink>
                </li>
                <li>
                    <NavLink activeClassName="active" to="/import">Import and export</NavLink>
                </li>
                <li>
                    <NavLink activeClassName="active" to="/settings">Settings</NavLink>
                </li>
            </ul>
        </nav>
    )
}