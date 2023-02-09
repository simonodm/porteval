import React from 'react';
import Button from 'react-bootstrap/Button';
import Offcanvas from 'react-bootstrap/Offcanvas';
import Sidebar from './Sidebar';

type Props = {
    /**
     * Determines whether the sidebar is displayed.
     * Has no effect on large screens when the sidebar is not off-canvas.
     */
    show?: boolean;

    /**
     * A callback to trigger when the sidebar is closed from inside the component.
     */
    onClose?: () => void;
}

/**
 * Renders the responsive offcanvas sidebar of the application. At larger screens,
 * the sidebar is rendered as usual.
 * 
 * @component
 */
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