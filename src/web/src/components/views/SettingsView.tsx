import React from 'react';
import SettingsForm from '../forms/SettingsForm';
import { toast } from 'react-toastify';

/**
 * Renders the settings configuration view.
 * 
 * @category Views
 * @component
 */
function SettingsView(): JSX.Element {
    return <SettingsForm onSuccess={() => toast.success('Saved')} onFailure={(error) => toast.error(error)}/>
}

export default SettingsView;