import React from 'react';
import { toast } from 'react-toastify';

import SettingsForm from '../forms/SettingsForm';

export default function SettingsView(): JSX.Element {
    return <SettingsForm onSuccess={() => toast.success('Saved')} onFailure={(error) => toast.error(error)}/>
}