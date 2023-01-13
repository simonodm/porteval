import React from 'react';
import CurrenciesView from '../../components/views/CurrenciesView';
import { screen } from '@testing-library/react';
import { Route, Router } from 'react-router-dom';
import { renderWithProviders } from '../utils';
import { createMemoryHistory } from 'history';

const renderTestCurrenciesView = () => {
    const history = createMemoryHistory();
    history.push('/currencies');

    renderWithProviders(
        <Router history={history}>
            <Route>
                <CurrenciesView />
            </Route>            
        </Router>
    );
}

describe('Currencies view', () => {
    test('renders currency selection', async () => {
        renderTestCurrenciesView();

        await screen.findByLabelText(/choose default currency/i);
    });

    test('renders exchange rates table', async () => {
        renderTestCurrenciesView();

        await screen.findByRole('table', { name: /Exchange rates table/i });
    });
})