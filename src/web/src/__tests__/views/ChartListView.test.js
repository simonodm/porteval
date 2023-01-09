import { fireEvent, screen } from '@testing-library/react';
import React from 'react';
import { Route, Router } from 'react-router-dom';
import ChartListView from '../../components/views/ChartListView';
import { renderWithProviders } from '../utils';
import { createMemoryHistory } from 'history';

const renderTestChartListView = (preconfiguredHistory = undefined) => {
    const history = preconfiguredHistory ?? createMemoryHistory();
    history.push('/charts');

    renderWithProviders(
        <Router history={history}>
            <Route>
                <ChartListView />
            </Route>            
        </Router>
    );
}

describe('Chart list view', () => {
    test('renders chart list table', async () => {
        renderTestChartListView();

        await screen.findByRole('table', { name: /Charts table/i });
    });

    test('renders create new chart button', async () => {
        renderTestChartListView();

        await screen.findByRole('button', { name: /create new chart/i });
    });

    test('create new chart button navigates to chart view', async () => {
        const history = createMemoryHistory();
        renderTestChartListView(history);

        const createButton = await screen.findByRole('button', { name: /create new chart/i });
        fireEvent.click(createButton);

        expect(history.location.pathname).toBe('/charts/view');
    });
})