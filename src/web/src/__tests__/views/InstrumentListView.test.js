import React from 'react';
import { fireEvent, screen } from '@testing-library/react';
import { Route, Router } from 'react-router-dom';
import { renderWithProviders } from '../utils';
import { createMemoryHistory } from 'history';
import InstrumentListView from '../../components/views/InstrumentListView';

const renderTestInstrumentListView = () => {
    const history = createMemoryHistory();
    history.push('/instruments')

    renderWithProviders(
        <Router history={history}>
            <Route path="/instruments">
                <InstrumentListView />
            </Route>
        </Router>
    );
}

describe('Instrument list view', () => {
    test('renders instruments table', async () => {
        renderTestInstrumentListView();

        await screen.findByRole('table', { name: /instruments table/i });
    });

    test('renders create new instrument button', async () => {
        renderTestInstrumentListView();

        await screen.findByRole('button', { name: /create new instrument/i });
    });

    test('create new instrument button opens instrument creation form on click', async () => {
        renderTestInstrumentListView();

        const createButton = await screen.findByRole('button', { name: /create new instrument/i });
        fireEvent.click(createButton);

        await screen.findByRole('form', { name: /create instrument form/i });
    })
})