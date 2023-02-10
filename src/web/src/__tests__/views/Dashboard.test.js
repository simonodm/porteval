import React from 'react';
import Dashboard from '../../components/views/Dashboard';
import { fireEvent, screen } from '@testing-library/react';
import { renderWithProviders } from '../utils';
import { testDashboardLayout } from '../mocks/testData';

describe('Dashboard view', () => {
    test('renders toggle dashboard edit button', async () => {
        renderWithProviders(<Dashboard />);

        await screen.findByRole('button', { name: /toggle dashboard edit/i });
    });

    test('toggle dashboard edit button displays chart remove button on click', async () => {
        renderWithProviders(<Dashboard />);

        const toggleButton = await screen.findByRole('button', { name: /toggle dashboard edit/i });
        fireEvent.click(toggleButton);

        const removeButtons = await screen.findAllByRole('button', { name: /x/i });
        expect(removeButtons.length).toBe(testDashboardLayout.items.length);
    });

    test('renders add charts button', async () => {
        renderWithProviders(<Dashboard />);

        await screen.findByRole('button', { name: /add charts/i });
    });

    test('add charts button displays chart picker on click', async () => {
        renderWithProviders(<Dashboard />);

        const addChartsButton = await screen.findByRole('button', { name: /add charts/i });
        fireEvent.click(addChartsButton);

        await screen.findByRole('picker', { name: /dashboard chart picker/i });
    });

    test('renders dashboard items', async () => {
        renderWithProviders(<Dashboard />);

        const charts = await screen.findAllByLabelText('dashboard-chart');

        expect(charts.length).toBe(testDashboardLayout.items.length);
    });
})