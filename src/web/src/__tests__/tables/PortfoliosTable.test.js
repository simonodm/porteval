import React from 'react';
import PortfoliosTable from '../../components/tables/PortfoliosTable';
import userEvent from '@testing-library/user-event';
import { renderWithProviders } from '../utils';
import { fireEvent, screen, waitForElementToBeRemoved, within } from '@testing-library/react';
import { testPortfolios, testPortfoliosStats } from '../mocks/testData';

const openEditPortfolioForm = async () => {
    const editPortfolioButton = await screen.findAllByRole('button', { name: /edit/i });
    fireEvent.click(editPortfolioButton[0]);

    return await screen.findByRole('form', { name: /edit portfolio form/i });
}

describe('Portfolios table', () => {
    test('renders correct headers', async () => {
        renderWithProviders(<PortfoliosTable />);

        const headers = ['name', 'currency', 'profit', 'performance', 'note', 'actions'];
        for await (const header of headers) {
            const regexp = new RegExp(header, 'i');
            await screen.findByRole('columnheader', { name: regexp });
        }
    });

    test('renders portfolios', async () => {
        renderWithProviders(<PortfoliosTable />);

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
        renderWithProviders(<PortfoliosTable />);
        
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

    test('edit portfolio button opens portfolio edit form on click', async () => {
        renderWithProviders(<PortfoliosTable />);
        
        await openEditPortfolioForm();
    });

    test('edit portfolio form contains editable name field', async () => {
        renderWithProviders(<PortfoliosTable />);
        
        const form = await openEditPortfolioForm();
        const nameInput = within(form).getByRole('textbox', { name: /name/i });
        expect(nameInput).toBeEnabled();
    });

    test('edit portfolio form contains editable currency field', async () => {
        renderWithProviders(<PortfoliosTable />);
        
        const form = await openEditPortfolioForm();
        const currencyInput = within(form).getByRole('combobox', { name: /currency/i });
        expect(currencyInput).toBeEnabled();
    });

    test('edit portfolio form contains editable note field', async () => {
        renderWithProviders(<PortfoliosTable />);
        
        const form = await openEditPortfolioForm();
        const noteInput = within(form).getByRole('textbox', { name: /note/i });
        expect(noteInput).toBeEnabled();
    });

    test('edited portfolio changes in view after edit portfolio form submit', async () => {
        renderWithProviders(<PortfoliosTable />);

        const newName = 'Form test portfolio';
        const newCurrency = 'CZK';
        const newNote = 'Form test note';

        const form = await openEditPortfolioForm();
        
        const nameInput = await within(form).findByRole('textbox', { name: /name/i });
        await userEvent.clear(nameInput);
        await userEvent.type(nameInput, newName);

        const currencyInput = await within(form).findByRole('combobox', { name: /currency/i });
        await userEvent.selectOptions(currencyInput, newCurrency);

        const noteInput = await within(form).findByRole('textbox', { name: /note/i });
        await userEvent.clear(noteInput);
        await userEvent.type(noteInput, newNote);

        const saveButton = await within(form).findByRole('button', { name: /save/i });
        await userEvent.click(saveButton);

        await screen.findByText(newName);
        await screen.findByText(newCurrency);
        await screen.findByText(newNote);
    });

    test('chart button navigates to chart view', async () => {
        const { router } = renderWithProviders(<PortfoliosTable />);

        const chartButtons = await screen.findAllByRole('button', { name: /chart/i });
        fireEvent.click(chartButtons[0]);

        expect(router.state.location.pathname).toBe('/charts/view');
    });

    test('remove button removes portfolio', async () => {
        renderWithProviders(<PortfoliosTable />);

        const rows = await screen.findAllByTestId('datarow');
        const removeButton = within(rows[0]).getByRole('button', { name: /remove/i });
        fireEvent.click(removeButton);

        await waitForElementToBeRemoved(rows[0]);
    });

    test('renders expanders', async () => {
        renderWithProviders(<PortfoliosTable />);

        const rows = await screen.findAllByTestId('datarow');
        rows.forEach(row => {
            within(row).getByTestId('expander');
        });
    });

    test('expander renders positions table on click', async () => {
        renderWithProviders(<PortfoliosTable />);

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