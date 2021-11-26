import './Sidebar.css';
import { NavLink } from 'react-router-dom';
import React from 'react';


export default function Sidebar(): JSX.Element {
    return (
        <nav id="sidebar" className="bg-dark">
            <ul className="list-unstyled">
                <li>
                    <NavLink to="/dashboard" activeClassName="active">Dashboard</NavLink>
                </li>
                <li>
                    <NavLink to="/portfolios" activeClassName="active">Portfolios</NavLink>
                </li>
                <li>
                    <NavLink to="/instruments" activeClassName="active">Instruments</NavLink>
                </li>
                <li>
                    <NavLink to="/charts" activeClassName="active">Charts</NavLink>
                </li>
                <li>
                    <NavLink to="/settings" activeClassName="active">Settings</NavLink>
                </li>
            </ul>
        </nav>
    )
}