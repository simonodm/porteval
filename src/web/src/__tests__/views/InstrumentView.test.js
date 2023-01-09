import React from 'react';
import InstrumentView from '../../components/views/InstrumentView';
import { renderWithProviders } from '../utils';
import { Route, Router } from 'react-router-dom';
import { fireEvent, screen } from '@testing-library/react';
import { testInstruments, testPrices } from '../mocks/testData';
import { parseISO } from 'date-fns';
import { createMemoryHistory } from 'history';

const testInstrument = testInstruments[0];
const testInstrumentPrice = testPrices.slice().reverse().find(p => p.instrumentId === testInstrument.id && parseISO(p.time) <= Date.now());

const renderTestInstrumentView = () => {
    const history = createMemoryHistory();
    history.push(`/instruments/${testInstrument.id}`)

    renderWithProviders(
        <Router history={history}>
            <Route path="/instruments/:instrumentId">
                <InstrumentView />
            </Route>
        </Router>
    );
}

describe('Instrument view', () => {
    test('renders instrument name', async () => {
        renderTestInstrumentView();

        await screen.findByRole('heading', { name: testInstrument.name });
    });

    test('renders instrument note', async () => {
        renderTestInstrumentView();

        await screen.findByText(testInstrument.note);
    });

    test('renders instrument current price', async () => {
        renderTestInstrumentView();

        await screen.findByText(`$${testInstrumentPrice.price.toFixed(2)}`);
    });

    test('renders instrument currency', async () => {
        renderTestInstrumentView();

        await screen.findByText(testInstrument.currencyCode);
    });

    test('renders preview chart', async () => {
        renderTestInstrumentView();

        await screen.findByLabelText('chart');
    });

    test('renders price history table', async () => {
        renderTestInstrumentView();

        await screen.findByLabelText(`Instrument ${testInstrument.id} prices table`);
    });

    test('renders add price button', async () => {
        renderTestInstrumentView();

        await screen.findByRole('button', { name: /add price/i });
    });

    test('add price button opens add price form', async () => {
        renderTestInstrumentView();

        const addPriceButton = await screen.findByRole('button', { name: /add price/i });
        fireEvent.click(addPriceButton);

        await screen.findByRole('form', { name: /create instrument price form/i });
    });
});