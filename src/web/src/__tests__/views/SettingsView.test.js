import React from 'react';
import SettingsView from '../../components/views/SettingsView';

import { screen, within } from '@testing-library/react';
import { renderWithProviders } from '../utils';

describe('Settings view', () => {
    test('renders settings form', async () => {
        renderWithProviders(<SettingsView />);

        await screen.findByRole('form', { name: /settings form/i });
    });

    test('settings form contains date format field', async () => {
        renderWithProviders(<SettingsView />);

        const form = await screen.findByRole('form', { name: /settings form/i });
        await within(form).findByRole('textbox', { name: /date format/i });
    });

    test('settings form contains time format field', async () => {
        renderWithProviders(<SettingsView />);

        const form = await screen.findByRole('form', { name: /settings form/i });
        await within(form).findByRole('textbox', { name: /time format/i });
    });

    test('settings form contains decimal separator field', async () => {
        renderWithProviders(<SettingsView />);

        const form = await screen.findByRole('form', { name: /settings form/i });
        await within(form).findByRole('textbox', { name: /decimal separator/i });
    });

    test('settings form contains thousands separator field', async () => {
        renderWithProviders(<SettingsView />);

        const form = await screen.findByRole('form', { name: /settings form/i });
        await within(form).findByRole('textbox', { name: /thousands separator/i });
    });
})