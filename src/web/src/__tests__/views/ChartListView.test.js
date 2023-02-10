import React from 'react';
import ChartListView from '../../components/views/ChartListView';
import { fireEvent, screen, waitForElementToBeRemoved, within } from '@testing-library/react';
import { testCharts } from '../mocks/testData';
import { renderWithProviders } from '../utils';

describe('Chart list view', () => {
    test('renders create new chart button', async () => {
        renderWithProviders(<ChartListView />);

        await screen.findByRole('button', { name: /create new chart/i });
    });

    test('create new chart button navigates to chart view', async () => {
        const { router } = renderWithProviders(<ChartListView />);

        const createButton = await screen.findByRole('button', { name: /create new chart/i });
        fireEvent.click(createButton);

        expect(router.state.location.pathname).toBe('/charts/view');
    });

    test('renders chart list table', async () => {
        renderWithProviders(<ChartListView />);

        await screen.findByRole('table', { name: /Charts table/i });
    });

    test('chart list table renders correct headers', async () => {
        renderWithProviders(<ChartListView />);
        
        await screen.findByRole('columnheader', { name: /.*name.*/i });
        await screen.findByRole('columnheader', { name: /.*actions.*/i });
    });

    test('chart list table renders charts', async () => {
        renderWithProviders(<ChartListView />);

        const rows = await screen.findAllByTestId('datarow');
        testCharts.forEach((chart, index) => {
            const row = rows[index];

            within(row).getByRole('cell', { name: chart.name });
            within(row).getByRole('button', { name: /remove/i });
        })
    });

    test('chart list table remove button deletes chart', async () => {
        renderWithProviders(<ChartListView />);

        const rows = await screen.findAllByTestId('datarow');
        const removeButton = within(rows[0]).getByRole('button', { name: /remove/i });
        fireEvent.click(removeButton);

        await waitForElementToBeRemoved(rows[0]);
    });
})