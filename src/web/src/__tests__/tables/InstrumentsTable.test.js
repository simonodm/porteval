import React from 'react';
import InstrumentsTable from '../../components/tables/InstrumentsTable';
import { fireEvent, screen, waitForElementToBeRemoved, within } from '@testing-library/react';
import { testInstruments } from '../mocks/testData';
import { renderWithProviders } from '../utils';
import { createMemoryHistory } from 'history';
import { Router, Route } from 'react-router';

const renderTestInstrumentsTable = (preconfiguredHistory = null) => {
    const history = preconfiguredHistory ?? createMemoryHistory();
    history.push('/instruments');

    renderWithProviders(
        <Router history={history}>
            <Route>
                <InstrumentsTable currencyCode='USD' />
            </Route>            
        </Router>
    );
}

describe('Instruments table', () => {
    test('renders correct headers', async () => {
        renderTestInstrumentsTable();

        const headers = ['name', 'symbol', 'currency', 'exchange', 'type', 'current price', 'actions'];
        for await (const header of headers) {
            const regexp = new RegExp(header, 'i');
            await screen.findByRole('columnheader', { name: regexp });
        }
    });

    test('renders instruments', async () => {
        renderTestInstrumentsTable();

        const rows = await screen.findAllByTestId('datarow');
        testInstruments.forEach((instrument, index) => {
            const row = rows[index];

            within(row).getByRole('cell', { name: instrument.name });
            within(row).getByRole('cell', { name: instrument.symbol });
            within(row).getByRole('cell', { name: instrument.currencyCode });

            if(instrument.exchange) {
                within(row).getByRole('cell', { name: instrument.exchange });
            }

            const typeRegexp = new RegExp(instrument.type, 'i');
            within(row).getByRole('cell', { name: typeRegexp });
            
            if(instrument.currentPrice) {
                const priceRegexp = new RegExp(`.*${instrument.currentPrice}.*`, 'i');
                within(row).getByRole('cell', { name: priceRegexp });
            }

            within(row).getByRole('button', { name: /chart/i });
            within(row).getByRole('button', { name: /edit/i });
            within(row).getByRole('button', { name: /remove/i });
        });
    });

    test('renders pagination controls', async () => {
        renderTestInstrumentsTable();

        await screen.findByLabelText('Pagination controls');
    });

    test('remove button removes instrument', async () => {
        renderTestInstrumentsTable();

        const rows = await screen.findAllByTestId('datarow');
        const removeButton = within(rows[0]).getByRole('button', { name: /remove/i });
        fireEvent.click(removeButton);

        await waitForElementToBeRemoved(rows[0]);
    });

    test('edit button opens instrument edit form', async () => {
        renderTestInstrumentsTable();

        const editButtons = await screen.findAllByRole('button', { name: /edit/i });
        fireEvent.click(editButtons[0]);

        await screen.findByLabelText('Edit instrument form');
    });

    test('chart button navigates to chart view', async () => {
        const history = createMemoryHistory();
        renderTestInstrumentsTable(history);

        const chartButtons = await screen.findAllByRole('button', { name: /chart/i });
        fireEvent.click(chartButtons[0]);

        expect(history.location.pathname).toBe('/charts/view');
    });
});