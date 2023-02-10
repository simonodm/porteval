import React from 'react';
import PortfolioView from '../../components/views/PortfolioView';
import userEvent from '@testing-library/user-event';

import { fireEvent, screen, within } from '@testing-library/react';
import { createTestMemoryRouter, renderWithProviders } from '../utils';
import { testInstruments, testPortfolios, testPortfoliosStats } from '../mocks/testData';

const testPortfolio = testPortfolios[0];
const testStats = testPortfoliosStats[0];

const renderTestPortfolioView = () => {
    const router = createTestMemoryRouter(
        '/portfolios/:portfolioId',
        `/portfolios/${testPortfolio.id}`,
        <PortfolioView />
    );

    renderWithProviders(
        <PortfolioView />,
        {
            router
        }
    );
}

const openCreatePositionForm = async () => {
    const openPositionButton = await screen.findByRole('button', { name: /open a position/i });
    fireEvent.click(openPositionButton);

    return await screen.findByRole('form', { name: /open position form/i });
}

describe('Portfolio view', () => {
    test('renders portfolio name', async () => {
        renderTestPortfolioView();

        await screen.findByRole('heading', { name: testPortfolio.name });
    });

    test('renders portfolio note', async () => {
        renderTestPortfolioView();

        await screen.findByText(testPortfolio.note);
    });

    test('renders portfolio value', async () => {
        renderTestPortfolioView();

        await screen.findByText('$111.00');
    });

    test('renders total profit', async () => {
        renderTestPortfolioView();

        const sign = testStats.totalProfit < 0 ? '-' : ''
        const profitString = testStats.totalProfit.toFixed(2);

        await screen.findByText(`${sign}$${profitString}`)
    });

    test('renders total performance', async () => {
        renderTestPortfolioView();

        const sign = testStats.totalPerformance < 0 ? '-' : '+'
        const performanceString = (testStats.totalPerformance * 100).toFixed(2);

        await screen.findByText(`${sign}${performanceString}%`)
    });

    test('renders preview chart', async () => {
        renderTestPortfolioView();

        await screen.findByLabelText('Chart preview');
    });

    test('renders positions table', async () => {
        renderTestPortfolioView();

        await screen.findByLabelText(`Portfolio ${testPortfolio.id} positions table`);
    });

    test('renders expand all button', async () => {
        renderTestPortfolioView();

        await screen.findByRole('button', { name: 'Expand all'});
    });

    test('renders collapse all button', async () => {
        renderTestPortfolioView();

        await screen.findByRole('button', { name: 'Collapse all'});
    });

    test('renders open position button', async () => {
        renderTestPortfolioView();

        await screen.findByRole('button', { name: /open a position/i });
    });

    test('open position button displays open position form on click', async () => {
        renderTestPortfolioView();

        await openCreatePositionForm();
    });

    test('open position form contains instrument field', async () => {
        renderTestPortfolioView();

        const form = await openCreatePositionForm();
        await within(form).findByRole('combobox', { name: /instrument/i });
    });

    test('open position form contains amount field', async () => {
        renderTestPortfolioView();

        const form = await openCreatePositionForm();
        await within(form).findByRole('textbox', { name: /amount/i });
    });

    test('open position form contains date field', async () => {
        renderTestPortfolioView();

        const form = await openCreatePositionForm();
        await within(form).findByRole('textbox', { name: /date/i });
    });

    test('open position form contains price field', async () => {
        renderTestPortfolioView();

        const form = await openCreatePositionForm();
        await within(form).findByRole('textbox', { name: /price/i });
    });

    test('open position form contains note field', async () => {
        renderTestPortfolioView();

        const form = await openCreatePositionForm();
        await within(form).findByRole('textbox', { name: /note/i });
    });

    test('created position appears in view after open position form is submitted', async () => {
        renderTestPortfolioView();

        const instrument = testInstruments[1];
        const amount = '3';
        const price = '120.61';
        const note = 'test form note';

        const form = await openCreatePositionForm();
        const instrumentInput = await within(form).findByRole('combobox', { name: /instrument/i });
        await userEvent.selectOptions(instrumentInput, instrument.id.toString());

        const amountInput = await within(form).findByRole('textbox', { name: /amount/i });
        await userEvent.clear(amountInput);
        await userEvent.type(amountInput, amount);

        const priceInput = await within(form).findByRole('textbox', { name: /price/i });
        await userEvent.clear(priceInput);
        await userEvent.type(priceInput, price);

        const noteInput = await within(form).findByRole('textbox', { name: /note/i });
        await userEvent.clear(noteInput);
        await userEvent.type(noteInput, note);

        const saveButton = await within(form).findByRole('button', { name: /save/i });
        await userEvent.click(saveButton);

        await screen.findByRole('cell', { name: instrument.name });
    })
})