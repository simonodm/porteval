import { screen } from '@testing-library/react';
import React from 'react';
import { Route, Router } from 'react-router-dom';
import { renderWithProviders } from '../utils';
import { createMemoryHistory } from 'history';
import SettingsView from '../../components/views/SettingsView';

const renderTestSettingsView = () => {
    const history = createMemoryHistory();
    history.push('/settings');

    renderWithProviders(
        <Router history={history}>
            <Route>
                <SettingsView />
            </Route>            
        </Router>
    );
}

describe('Settings view', () => {
    test('renders settings form', async () => {
        renderTestSettingsView();

        await screen.findByRole('form', { name: /settings form/i });
    });
})