import React from 'react';
import ReactModal from 'react-modal';
import './ModalWrapper.css';
import { ModalCallbacks } from '../../types';

type Props = {
    children?: Array<JSX.Element> | JSX.Element;
    isOpen: boolean;
} & ModalCallbacks

export default function ModalWrapper({ children, isOpen, closeModal }: Props): JSX.Element {
    return (
        <ReactModal
            isOpen={isOpen}
            onRequestClose={closeModal}
            shouldCloseOnEsc={true}
            shouldCloseOnOverlayClick={true}
            overlayClassName="modal-overlay"
            >
            {
                Array.isArray(children)
                    ? children.map(child => React.cloneElement(child, { closeModal }))
                    : children ? React.cloneElement(children, { closeModal }) : <></>
            }
        </ReactModal>
    )
}