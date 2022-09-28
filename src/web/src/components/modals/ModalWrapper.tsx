import React from 'react';
import ReactModal from 'react-modal';
import PageHeading from '../ui/PageHeading';

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
            <div className="modal-controls">
                <button className="btn btn-sm btn-danger float-right" onClick={closeModal}>
                    <i className="bi bi-x"></i>
                </button>
            </div>
            { heading !== undefined && <PageHeading heading={heading} /> }
            <div className="modal-inner">
                {
                    Array.isArray(children)
                        ? children.map(child => React.cloneElement(child, { closeModal }))
                        : children ? React.cloneElement(children, { closeModal }) : null
                }
            </div>
        </ReactModal>
    )
}

export default ModalWrapper;