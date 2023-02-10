import React from 'react';
import PositionsTable from '../../components/tables/PositionsTable';
import userEvent from '@testing-library/user-event';
import { testPortfolios, testPositions, testPositionStats } from '../mocks/testData';
import { fireEvent, screen, waitForElementToBeRemoved, within } from '@testing-library/react';
import { renderWithProviders, reformatDateTime } from '../utils';

const testPortfolio = testPortfolios[0];
const testPortfolioPositions = testPositions.filter(p => p.portfolioId === testPortfolio.id);
const testPortfolioPositionStats =
    testPositionStats.filter(s => testPortfolioPositions.find(p => p.id === s.id) !== undefined);

const openAddTransactionForm = async () => {
    const addButton = await screen.findAllByRole('button', { name: /add transaction/i });
    fireEvent.click(addButton[0]);

    return await screen.findByRole('form', { name: /create transaction form/i });
}

const openEditPositionForm = async () => {
    const editButtons = await screen.findAllByRole('button', { name: /edit/i });
    fireEvent.click(editButtons[0]);

    return await screen.findByRole('form', { name: /edit position form/i });
}

describe('Positions table', () => {
    test('renders correct headers', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        const headers =
            ['name', 'exchange', 'currency', 'size', 'profit', 'performance',
                'bep', 'current price', 'note', 'actions'];

        for await (const header of headers) {
            const regexp = new RegExp(header, 'i');
            await screen.findByRole('columnheader', { name: regexp });
        }
    });

    test('renders positions data', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

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
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

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
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        const rows = await screen.findAllByTestId('datarow');
        rows.forEach(row => {
            within(row).getByTestId('expander');
        });
    });

    test('expander renders transactions table on click', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

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
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        const rows = await screen.findAllByTestId('datarow');
        const removeButton = within(rows[0]).getByRole('button', { name: /remove/i });
        fireEvent.click(removeButton);

        await waitForElementToBeRemoved(rows[0]);
    });

    test('edit button renders position edit form', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        await openEditPositionForm();
    });

    test('position edit form renders disabled portfolio dropdown', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        const form = await openEditPositionForm();
        const portfolioInput = within(form).getByRole('combobox', { name: /portfolio/i });
        expect(portfolioInput).toBeDisabled();
    });

    test('position edit form renders disabled instrument dropdown', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        const form = await openEditPositionForm();
        const instrumentInput = within(form).getByRole('combobox', { name: /instrument/i });
        expect(instrumentInput).toBeDisabled();
    });

    test('position edit form renders editable note input', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        const form = await openEditPositionForm();
        const noteInput = within(form).getByRole('textbox', { name: /note/i });
        expect(noteInput).toBeEnabled();
    });

    test('edited position changes in the table after position edit form is submitted', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        const newNote = 'edited position note';

        const form = await openEditPositionForm();
        const noteInput = within(form).getByRole('textbox', { name: /note/i });
        await userEvent.clear(noteInput);
        await userEvent.type(noteInput, newNote);

        const saveButton = within(form).getByRole('button', { name: /save/i });
        await userEvent.click(saveButton);

        await screen.findByRole('cell', { name: newNote });
    });

    test('add transaction button opens add transaction form', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        await openAddTransactionForm();
    });

    test('add transaction form contains editable amount input', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        const form = await openAddTransactionForm();
        within(form).getByRole('textbox', { name: /amount/i });
    });

    test('add transaction form contains editable price input', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        const form = await openAddTransactionForm();
        within(form).getByRole('textbox', { name: /price/i });
    });

    test('add transaction form contains editable date input', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        const form = await openAddTransactionForm();
        within(form).getByRole('textbox', { name: /date/i });
    });

    test('add transaction form contains editable note input', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        const form = await openAddTransactionForm();
        within(form).getByRole('textbox', { name: /note/i });
    });

    test('new transaction appears in table after add transaction is submitted', async () => {
        renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        const newAmount = '0.33';
        const newPrice = '101.12';
        const newDate = reformatDateTime('2022-06-06T12:11:59Z');
        const newNote = 'new transaction note';

        const form = await openAddTransactionForm();

        const amountInput = within(form).getByRole('textbox', { name: /amount/i });
        await userEvent.clear(amountInput);
        await userEvent.type(amountInput, newAmount);

        const priceInput = within(form).getByRole('textbox', { name: /price/i });
        await userEvent.clear(priceInput);
        await userEvent.type(priceInput, newPrice);

        const dateInput = within(form).getByRole('textbox', { name: /date/i });
        await userEvent.clear(dateInput);
        await userEvent.type(dateInput, newDate);

        const noteInput = within(form).getByRole('textbox', { name: /note/i });
        await userEvent.clear(noteInput);
        await userEvent.type(noteInput, newNote);

        const saveButton = within(form).getByRole('button', { name: /save/i });
        await userEvent.click(saveButton);

        const amountRegexp = new RegExp(newAmount, 'i');
        const priceRegexp = new RegExp(`.*${newPrice}.*`, 'i');

        await screen.findByRole('cell', { name: amountRegexp, hidden: true });
        await screen.findByRole('cell', { name: priceRegexp, hidden: true });
        await screen.findByRole('cell', { name: newNote, hidden: true });
        await screen.findByRole('cell', { name: newDate, hidden: true });
    });

    test('chart button navigates to chart view', async () => {
        const { router } = renderWithProviders(<PositionsTable portfolioId={testPortfolio.id} />);

        const chartButtons = await screen.findAllByRole('button', { name: /chart/i });
        fireEvent.click(chartButtons[0]);

        expect(router.state.location.pathname).toBe('/charts/view');
    });
});