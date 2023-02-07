import React from 'react';
import Navbar from 'react-bootstrap/Navbar';
import Button from 'react-bootstrap/Button';

import { useDispatch } from 'react-redux';
import { RTK_API_TAGS } from '../constants';
import { portEvalApi } from '../redux/api/portEvalApi';
import './Header.css';

type Props = Record<string, never> | {
    enableResponsiveSidebar: true;
    onSidebarToggle?: () => void;
}

/**
 * Renders the application header.
 * 
 * @component
 */
function Header({ enableResponsiveSidebar, onSidebarToggle }: Props): JSX.Element {
    const dispatch = useDispatch(); // used to invalidate all API cache on refresh

    return (
        <Navbar bg="dark" variant="dark" className="p-0 justify-content-between">
            {
                enableResponsiveSidebar &&
                    <Button variant="dark" onClick={onSidebarToggle} className="d-lg-none">
                        <i className="bi bi-list" />
                    </Button>
            }
            <Navbar.Brand href="/" id="app-logo">PortEval</Navbar.Brand>
            <Button variant="dark" size="sm" onClick={() => dispatch(portEvalApi.util.invalidateTags(RTK_API_TAGS))}>
                <i className="bi bi-arrow-clockwise"></i>
            </Button>
        </Navbar>
    )
}

export default Header;