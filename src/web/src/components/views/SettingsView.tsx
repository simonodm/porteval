import React from 'react';
import SettingsForm from '../forms/SettingsForm';
import PageHeading from '../ui/PageHeading';

import Container from 'react-bootstrap/Container';

import { toast } from 'react-toastify';

/**
 * Renders the settings configuration view.
 * 
 * @category Views
 * @component
 */
function SettingsView(): JSX.Element {
    return (
        <>
            <PageHeading heading='Settings' />
            <Container fluid className="g-0">
                <SettingsForm onSuccess={() => toast.success('Saved')} onFailure={(error) => toast.error(error)}/>
            </Container>            
        </>
    )
}

export default SettingsView;