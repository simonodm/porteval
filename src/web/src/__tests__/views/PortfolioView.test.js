import React from 'react';
import PortfolioView from '../../components/views/PortfolioView';
import { fireEvent, screen } from '@testing-library/react';
import { renderWithProviders } from '../utils';
import { Route, Router } from 'react-router-dom';
import { testPortfolios, testPortfolioStats } from '../mocks/testData';
import { createMemoryHistory } from 'history';

const testPortfolio = testPortfolios[0];
const testStats = testPortfolioStats[0];

const renderTestPortfolioView = () => {
    const history = createMemoryHistory();
    history.push(`/portfolios/${testPortfolio.id}`)

    renderWithProviders(
        <Router history={history}>
            <Route path="/portfolios/:portfolioId">
                <PortfolioView />
            </Route>
        </Router>
    );
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

        await screen.findByRole('button', { name: 'Open position' });
    });

    test('open position button displays open position form on click', async () => {
        renderTestPortfolioView();

        const openPositionButton = await screen.findByRole('button', { name: 'Open position' });
        fireEvent.click(openPositionButton);

        await screen.findByRole('form', { name: 'Open position form' });
    });
})