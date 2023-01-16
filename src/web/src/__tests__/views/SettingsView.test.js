import { screen, within } from '@testing-library/react';
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

    test('settings form contains date format field', async () => {
        renderTestSettingsView();

        const form = await screen.findByRole('form', { name: /settings form/i });
        await within(form).findByRole('textbox', { name: /date format/i });
    });

    test('settings form contains time format field', async () => {
        renderTestSettingsView();

        const form = await screen.findByRole('form', { name: /settings form/i });
        await within(form).findByRole('textbox', { name: /time format/i });
    });

    test('settings form contains decimal separator field', async () => {
        renderTestSettingsView();

        const form = await screen.findByRole('form', { name: /settings form/i });
        await within(form).findByRole('textbox', { name: /decimal separator/i });
    });

    test('settings form contains thousands separator field', async () => {
        renderTestSettingsView();

        const form = await screen.findByRole('form', { name: /settings form/i });
        await within(form).findByRole('textbox', { name: /thousands separator/i });
    });
})