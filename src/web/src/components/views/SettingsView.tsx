import React from 'react';
import SettingsForm from '../forms/SettingsForm';
import { toast } from 'react-toastify';
import PageHeading from '../ui/PageHeading';

/**
 * Renders the settings configuration view.
 * 
 * @category Views
 * @component
 */
function SettingsView(): JSX.Element {
    return <>
        <PageHeading heading='Settings' />
        <SettingsForm onSuccess={() => toast.success('Saved')} onFailure={(error) => toast.error(error)}/>
    </>
}

export default SettingsView;