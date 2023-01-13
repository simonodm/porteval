import React from 'react';
import PortfolioListView from '../../components/views/PortfolioListView';
import { fireEvent, screen } from '@testing-library/react';
import { Route, Router } from 'react-router-dom';
import { renderWithProviders } from '../utils';
import { createMemoryHistory } from 'history';

const renderTestPortfolioListView = () => {
    const history = createMemoryHistory();
    history.push('/portfolios')

    renderWithProviders(
        <Router history={history}>
            <Route path="/portfolios">
                <PortfolioListView />
            </Route>
        </Router>
    );
}

describe('Portfolio list view', () => {
    test('renders portfolios table', async () => {
        renderTestPortfolioListView();

        await screen.findByRole('table', { name: /portfolios table/i });
    });

    test('renders create new portfolio button', async () => {
        renderTestPortfolioListView();

        await screen.findByRole('button', { name: /create new portfolio/i });
    });

    test('create new portfolio button opens portfolio creation form on click', async () => {
        renderTestPortfolioListView();

        const createButton = await screen.findByRole('button', { name: /create new portfolio/i });
        fireEvent.click(createButton);

        await screen.findByRole('form', { name: /create portfolio form/i });
    })
})