import React from 'react';
import Button from 'react-bootstrap/Button';
import Offcanvas from 'react-bootstrap/Offcanvas';
import Sidebar from './Sidebar';

type Props = {
    show?: boolean;
    onClose?: () => void;
}

function OffcanvasSidebar({ show, onClose }: Props): JSX.Element {
    return (
        <Offcanvas show={show} onHide={onClose} className="text-bg-dark"
            responsive="lg"
        >
            <Offcanvas.Header className="justify-content-end">
                <Button variant="dark" onClick={onClose}>
                    <i className="bi bi-x"></i>
                </Button>
            </Offcanvas.Header>
            <Offcanvas.Body className="h-100">
                <Sidebar onRouteChange={onClose} />
            </Offcanvas.Body>
        </Offcanvas>
    )
}

export default OffcanvasSidebar;