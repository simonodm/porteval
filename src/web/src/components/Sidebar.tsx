import React from 'react';
import { NavLink } from 'react-router-dom';
import './Sidebar.css';

type Route = {
    url: string;
    name: string;
}

type Props = {
    onRouteChange?: (newRoute: Route) => void;
}

/**
 * Renders the application sidebar.
 * 
 * @component
 */
function Sidebar({ onRouteChange }: Props): JSX.Element {
    const routes: Route[] = [
        {
            url: '/dashboard',
            name: 'Dashboard'
        },
        {
            url: '/portfolios',
            name: 'Portfolios'
        },
        {
            url: '/instruments',
            name: 'Instruments'
        },
        {
            url: '/charts',
            name: 'Charts'
        },
        {
            url: '/currencies',
            name: 'Currencies'
        },
        {
            url: '/import',
            name: 'Import and export'
        },
        {
            url: '/settings',
            name: 'Settings'
        }
    ];

    return (
        <nav className="bg-dark" id="sidebar">
            <ul className="list-unstyled">
                {
                    routes.map(route =>
                        <li key={route.url}>
                            <NavLink
                                className={({ isActive }) => isActive ? "active" : ""}
                                to={route.url}
                                onClick={() => onRouteChange && onRouteChange(route)}
                            >
                                {route.name}
                            </NavLink>
                        </li>
                    )
                }
            </ul>
        </nav>
    )
}

export default Sidebar;