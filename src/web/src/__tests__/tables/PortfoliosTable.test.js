import React from 'react';
import PortfoliosTable from '../../components/tables/PortfoliosTable';
import { createMemoryHistory } from 'history';
import { renderWithProviders } from '../utils';
import { Router, Route } from 'react-router';
import { fireEvent, screen, waitForElementToBeRemoved, within } from '@testing-library/react';
import { testPortfolios, testPortfoliosStats } from '../mocks/testData';

const renderTestPortfoliosTable = (preconfiguredHistory = null) => {
    const history = preconfiguredHistory ?? createMemoryHistory();
    history.push('/portfolios');

    renderWithProviders(
        <Router history={history}>
            <Route>
                <PortfoliosTable />
            </Route>            
        </Router>
    );
}

describe('Portfolios table', () => {
    test('renders correct headers', async () => {
        renderTestPortfoliosTable();

        const headers = ['name', 'currency', 'profit', 'performance', 'note', 'actions'];
        for await (const header of headers) {
            const regexp = new RegExp(header, 'i');
            await screen.findByRole('columnheader', { name: regexp });
        }
    });

    test('renders portfolios', async () => {
        renderTestPortfoliosTable();

        const rows = await screen.findAllByTestId('datarow');
        testPortfolios.forEach((portfolio, index) => {
            const row = rows[index];

            within(row).getByRole('cell', { name: portfolio.name });
            within(row).getByRole('cell', { name: portfolio.currencyCode });
            within(row).getByRole('cell', { name: portfolio.note });
            within(row).getByRole('button', { name: /chart/i });
            within(row).getByRole('button', { name: /edit/i });
            within(row).getByRole('button', { name: /remove/i });
        });
    });

    test('renders portfolios metrics', async () => {
        renderTestPortfoliosTable();

        const rows = await screen.findAllByTestId('datarow');
        testPortfolios.forEach((portfolio, index) => {
            const row = rows[index];
            const portfolioStats = testPortfoliosStats.find(p => p.id === portfolio.id);

            const formatter = new Intl.NumberFormat('en-US', { style: 'currency', currency: portfolio.currencyCode })

            within(row).getAllByRole('cell', { name: formatter.format(portfolioStats.totalProfit) });
            within(row).getAllByRole('cell', { name: formatter.format(portfolioStats.lastMonthProfit) });
            within(row).getAllByRole('cell', { name: formatter.format(portfolioStats.lastWeekProfit) });
            within(row).getAllByRole('cell', { name: formatter.format(portfolioStats.lastDayProfit) });
            within(row).getAllByRole('cell', { name: new RegExp(`.*${portfolioStats.totalPerformance * 100}.*`)});
            within(row).getAllByRole('cell', { name: new RegExp(`.*${portfolioStats.lastMonthPerformance * 100}.*`)});
            within(row).getAllByRole('cell', { name: new RegExp(`.*${portfolioStats.lastWeekPerformance * 100}.*`)});
            within(row).getAllByRole('cell', { name: new RegExp(`.*${portfolioStats.lastDayPerformance * 100}.*`)});
        });
    });

    test('edit button opens portfolio edit form', async () => {
        renderTestPortfoliosTable();

        const editButtons = await screen.findAllByRole('button', { name: /edit/i });
        fireEvent.click(editButtons[0]);

        await screen.findByLabelText(/edit portfolio form/i);
    });

    test('chart button navigates to chart view', async () => {
        const history = createMemoryHistory();
        renderTestPortfoliosTable(history);

        const chartButtons = await screen.findAllByRole('button', { name: /chart/i });
        fireEvent.click(chartButtons[0]);

        expect(history.location.pathname).toBe('/charts/view');
    });

    test('remove button removes portfolio', async () => {
        renderTestPortfoliosTable();

        const rows = await screen.findAllByTestId('datarow');
        const removeButton = within(rows[0]).getByRole('button', { name: /remove/i });
        fireEvent.click(removeButton);

        await waitForElementToBeRemoved(rows[0]);
    });

    test('renders expanders', async () => {
        renderTestPortfoliosTable();

        const rows = await screen.findAllByTestId('datarow');
        rows.forEach(row => {
            within(row).getByTestId('expander');
        });
    });

    test('expander renders positions table on click', async () => {
        renderTestPortfoliosTable();

        const rows = await screen.findAllByTestId('datarow');
        rows.forEach(row => {
            const expander = within(row).getByTestId('expander');
            fireEvent.click(expander);
        });

        const positionsTables = await screen.findAllByLabelText(/Portfolio .* positions table/i);
        positionsTables.forEach(table => {
            expect(table).toBeVisible();
        });
    });
});