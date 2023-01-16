import React from 'react';
import ChartListView from '../../components/views/ChartListView';
import { fireEvent, screen, waitForElementToBeRemoved, within } from '@testing-library/react';
import { Route, Router } from 'react-router-dom';
import { renderWithProviders } from '../utils';
import { createMemoryHistory } from 'history';
import { testCharts } from '../mocks/testData';

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

    test('renders chart list table', async () => {
        renderTestChartListView();

        await screen.findByRole('table', { name: /Charts table/i });
    });

    test('chart list table renders correct headers', async () => {
        renderTestChartListView();
        
        await screen.findByRole('columnheader', { name: /.*name.*/i });
        await screen.findByRole('columnheader', { name: /.*actions.*/i });
    });

    test('chart list table renders charts', async () => {
        renderTestChartListView();

        const rows = await screen.findAllByTestId('datarow');
        testCharts.forEach((chart, index) => {
            const row = rows[index];

            within(row).getByRole('cell', { name: chart.name });
            within(row).getByRole('button', { name: /remove/i });
        })
    });

    test('chart list table remove button deletes chart', async () => {
        renderTestChartListView();

        const rows = await screen.findAllByTestId('datarow');
        const removeButton = within(rows[0]).getByRole('button', { name: /remove/i });
        fireEvent.click(removeButton);

        await waitForElementToBeRemoved(rows[0]);
    });
})