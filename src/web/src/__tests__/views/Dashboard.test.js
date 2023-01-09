import { fireEvent, screen } from '@testing-library/react';
import React from 'react';
import { Route, Router } from 'react-router-dom';
import { renderWithProviders } from '../utils';
import { createMemoryHistory } from 'history';
import Dashboard from '../../components/views/Dashboard';
import { testDashboardLayout } from '../mocks/testData';

const renderTestDashboard = () => {
    const history = createMemoryHistory();
    history.push('/');

    renderWithProviders(
        <Router history={history}>
            <Route>
                <Dashboard />
            </Route>            
        </Router>
    );
}

describe('Dashboard view', () => {
    test('renders toggle dashboard edit button', async () => {
        renderTestDashboard();

        await screen.findByRole('button', { name: /toggle dashboard edit/i });
    });

    test('toggle dashboard edit button displays chart remove button on click', async () => {
        renderTestDashboard();

        const toggleButton = await screen.findByRole('button', { name: /toggle dashboard edit/i });
        fireEvent.click(toggleButton);

        const removeButtons = await screen.findAllByRole('button', { name: /x/i });
        expect(removeButtons.length).toBe(testDashboardLayout.items.length);
    })

    test('renders add charts button', async () => {
        renderTestDashboard();

        await screen.findByRole('button', { name: /add charts/i });
    });

    test('add charts button displays chart picker on click', async () => {
        renderTestDashboard();

        const addChartsButton = await screen.findByRole('button', { name: /add charts/i });
        fireEvent.click(addChartsButton);

        await screen.findByRole('picker', { name: /dashboard chart picker/i });
    });

    test('renders dashboard items', async () => {
        renderTestDashboard();

        const charts = await screen.findAllByLabelText('dashboard-chart');

        expect(charts.length).toBe(testDashboardLayout.items.length);
    });
})