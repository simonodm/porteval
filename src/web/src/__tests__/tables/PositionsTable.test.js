import React from 'react';
import PositionsTable from '../../components/tables/PositionsTable';
import { Router, Route } from 'react-router';
import { testPortfolios, testPositions, testPositionStats } from '../mocks/testData';
import { fireEvent, screen, waitForElementToBeRemoved, within } from '@testing-library/react';
import { createMemoryHistory } from 'history';
import { renderWithProviders } from '../utils';

const testPortfolio = testPortfolios[0];
const testPortfolioPositions = testPositions.filter(p => p.portfolioId === testPortfolio.id);
const testPortfolioPositionStats =
    testPositionStats.filter(s => testPortfolioPositions.find(p => p.id === s.id) !== undefined);

const renderTestPositionsTable = (preconfiguredHistory = null) => {
    const history = preconfiguredHistory ?? createMemoryHistory();
    history.push('/portfolios');

    renderWithProviders(
        <Router history={history}>
            <Route>
                <PositionsTable portfolioId={testPortfolio.id} />
            </Route>            
        </Router>
    );
}

describe('Positions table', () => {
    test('renders correct headers', async () => {
        renderTestPositionsTable();

        const headers =
            ['name', 'exchange', 'currency', 'size', 'profit', 'performance',
                'bep', 'current price', 'note', 'actions'];

        for await (const header of headers) {
            const regexp = new RegExp(header, 'i');
            await screen.findByRole('columnheader', { name: regexp });
        }
    });

    test('renders positions data', async () => {
        renderTestPositionsTable();

        const rows = await screen.findAllByTestId('datarow');
        testPortfolioPositions.forEach((position, index) => {
            const row = rows[index];

            within(row).getByRole('cell', { name: position.instrument.name });

            if(position.instrument.exchange) {
                within(row).getByRole('cell', { name: position.instrument.exchange });
            }

            within(row).getByRole('cell', { name: position.instrument.currencyCode });
            within(row).getByRole('cell', { name: position.positionSize });
            within(row).getByRole('cell', { name: position.note });

            within(row).getByRole('button', { name: /edit/i });
            within(row).getByRole('button', { name: /remove/i });
            within(row).getByRole('button', { name: /add transaction/i });
        });
    });

    test('renders positions metrics', async () => {
        renderTestPositionsTable();

        const rows = await screen.findAllByTestId('datarow');
        testPortfolioPositions.forEach((position, index) => {
            const row = rows[index];
            const positionStats = testPortfolioPositionStats.find(p => p.id === position.id);

            const formatter = new Intl.NumberFormat(
                'en-US',
                { style: 'currency', currency: position.instrument.currencyCode }
            );

            within(row).getAllByRole('cell', { name: formatter.format(positionStats.totalProfit) });
            within(row).getAllByRole('cell', { name: formatter.format(positionStats.lastMonthProfit) });
            within(row).getAllByRole('cell', { name: formatter.format(positionStats.lastWeekProfit) });
            within(row).getAllByRole('cell', { name: formatter.format(positionStats.lastDayProfit) });
            within(row).getAllByRole('cell', { name: new RegExp(`.*${positionStats.totalPerformance * 100}.*`)});
            within(row).getAllByRole('cell', { name: new RegExp(`.*${positionStats.lastMonthPerformance * 100}.*`)});
            within(row).getAllByRole('cell', { name: new RegExp(`.*${positionStats.lastWeekPerformance * 100}.*`)});
            within(row).getAllByRole('cell', { name: new RegExp(`.*${positionStats.lastDayPerformance * 100}.*`)});
            within(row).getAllByRole('cell', { name: formatter.format(positionStats.breakEvenPoint) });
            
            if(position.instrument.currentPrice) {
                within(row).getAllByRole('cell', { name: formatter.format(position.instrument.currentPrice) });
            }
        });
    });

    test('renders expanders', async () => {
        renderTestPositionsTable();

        const rows = await screen.findAllByTestId('datarow');
        rows.forEach(row => {
            within(row).getByTestId('expander');
        });
    });

    test('expander renders transactions table on click', async () => {
        renderTestPositionsTable();

        const rows = await screen.findAllByTestId('datarow');
        rows.forEach(row => {
            const expander = within(row).getByTestId('expander');
            fireEvent.click(expander);
        });

        const transactionTables = await screen.findAllByLabelText(/Position .* transactions table/i);
        transactionTables.forEach(t => {
            expect(t).toBeVisible();
        });
    });

    test('remove button removes position on click', async () => {
        renderTestPositionsTable();

        const rows = await screen.findAllByTestId('datarow');
        const removeButton = within(rows[0]).getByRole('button', { name: /remove/i });
        fireEvent.click(removeButton);

        await waitForElementToBeRemoved(rows[0]);
    });

    test('edit button renders position edit form', async () => {
        renderTestPositionsTable();

        const editButtons = await screen.findAllByRole('button', { name: /edit/i });
        fireEvent.click(editButtons[0]);

        await screen.findByLabelText('Edit position form');
    });

    test('chart button navigates to chart view', async () => {
        const history = createMemoryHistory();
        renderTestPositionsTable(history);

        const chartButtons = await screen.findAllByRole('button', { name: /chart/i });
        fireEvent.click(chartButtons[0]);

        expect(history.location.pathname).toBe('/charts/view');
    });
});