import React from 'react';
import ChartsTable from '../../components/tables/ChartsTable';
import { fireEvent, screen, within, waitForElementToBeRemoved } from '@testing-library/react';
import { testCharts } from '../mocks/testData';
import { renderWithProviders } from '../utils';

describe('Charts table', () => {
    test('renders correct headers', async () => {
        renderWithProviders(<ChartsTable />);
        
        await screen.findByRole('columnheader', { name: /.*name.*/i });
        await screen.findByRole('columnheader', { name: /.*actions.*/i });
    });

    test('renders charts', async () => {
        renderWithProviders(<ChartsTable />);

        const rows = await screen.findAllByTestId('datarow');
        testCharts.forEach((chart, index) => {
            const row = rows[index];

            within(row).getByRole('cell', { name: chart.name });
            within(row).getByRole('button', { name: /remove/i });
        })
    });

    test('remove button deletes chart', async () => {
        renderWithProviders(<ChartsTable />);

        const rows = await screen.findAllByTestId('datarow');
        const removeButton = within(rows[0]).getByRole('button', { name: /remove/i });
        fireEvent.click(removeButton);

        await waitForElementToBeRemoved(rows[0]);
    });
});