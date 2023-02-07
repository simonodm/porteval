import React from 'react';
import ReactModal from 'react-modal';
import PageHeading from '../ui/PageHeading';

import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';

import { ModalCallbacks } from '../../types';

import './ModalWrapper.css';

type Props = {
    /**
     * Modal header.
     */
    heading?: string;

    /**
     * Modal content.
     */
    children?: Array<JSX.Element> | JSX.Element | null;

    /**
     * Determines whether the modal is displayed.
     */
    isOpen: boolean;
} & ModalCallbacks

/**
 * Renders a modal containing the specified children.
 * 
 * @category Modal
 * @component
 */
function ModalWrapper({ heading, children, isOpen, closeModal }: Props): JSX.Element {
    return (
        <ReactModal
            isOpen={isOpen}
            onRequestClose={closeModal}
            overlayClassName="modal-overlay"
            shouldCloseOnEsc={true}
            shouldCloseOnOverlayClick={true}
        >
            <Container fluid>
                <Row>
                    <Col>
                        { heading !== undefined &&
                            <PageHeading heading={heading}>
                                <span className="modal-controls">
                                    <Button variant="danger" size="sm"
                                        className="float-right" onClick={closeModal}
                                    >
                                        <i className="bi bi-x"></i>
                                    </Button>
                                </span>                                
                            </PageHeading>
                        }
                    </Col>
                </Row>
                <Row>
                    <Col>                        
                        <Container fluid className="modal-inner">
                            {
                                Array.isArray(children)
                                    ? children.map(child => React.cloneElement(child, { closeModal }))
                                    : children ? React.cloneElement(children, { closeModal }) : null
                            }
                        </Container>
                    </Col>
                </Row>
            </Container>
        </ReactModal>
    )
}

export default ModalWrapper;