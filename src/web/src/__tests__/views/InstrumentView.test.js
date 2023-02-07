/* eslint-disable react/prop-types */
/* eslint-disable react/display-name */

import React from 'react';
import InstrumentView from '../../components/views/InstrumentView';
import { renderWithProviders, reformatDateTime } from '../utils';
import { Route, Router } from 'react-router-dom';
import { fireEvent, screen, waitFor, waitForElementToBeRemoved, within } from '@testing-library/react';
import { testInstruments, testInstrumentSplits,testPrices } from '../mocks/testData';
import { parseISO } from 'date-fns';
import { createMemoryHistory } from 'history';
import userEvent from '@testing-library/user-event';

const testInstrument = testInstruments[0];
const testInstrumentPrices = testPrices.filter(p => p.instrumentId === testInstrument.id);
const testSplits = testInstrumentSplits.filter(s => s.instrumentId === testInstrument.id);
const testInstrumentPrice = testPrices
    .slice()
    .reverse()
    .find(p => p.instrumentId === testInstrument.id && parseISO(p.time) <= Date.now());

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

const openPriceAddForm = async () => {
    const addPriceButton = await screen.findByRole('button', { name: /add a price/i });
    fireEvent.click(addPriceButton);

    return await screen.findByRole('form', { name: /create instrument price form/i });
}

const openSplitAddForm = async () => {
    const addSplitButton = await screen.findByRole('button', { name: /add a split/i });
    fireEvent.click(addSplitButton);

    return await screen.findByRole('form', { name: /create instrument split form/i });
}

jest.mock('react-select', () => ({ options, value, 'aria-label': ariaLabel, onChange }) => {
    function handleChange(event) {
      const option = options.find(
        option => option.value === event.currentTarget.value
      );
      onChange(option);
    }
    return (
        <select aria-label={ariaLabel} data-testid="select" onChange={handleChange}
            value={value}
        >
            {options.map(({ label, value }) => (
                <option key={value} value={value}>
                    {label}
                </option>
        ))}
        </select>
    );
  });

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

        await screen.findByLabelText('Chart preview');
    });

    test('renders add price button', async () => {
        renderTestInstrumentView();

        await screen.findByRole('button', { name: /add a price/i });
    });

    test('add price button opens add price form', async () => {
        renderTestInstrumentView();

        await openPriceAddForm();
    });

    test('add price form renders date field', async () => {
        renderTestInstrumentView();

        const form = await openPriceAddForm();
        await within(form).findByRole('textbox', { name: /date/i });
    });

    test('add price form renders price field', async () => {
        renderTestInstrumentView();

        const form = await openPriceAddForm();
        await within(form).findByRole('textbox', { name: /price/i });
    });

    test('new price appears in view after add price form submit', async () => {
        renderTestInstrumentView();

        const form = await openPriceAddForm();

        const date = reformatDateTime('2023-01-01T03:23:00Z');
        const price = '135.15';

        const dateInput = await within(form).findByRole('textbox', { name: /date/i });
        await userEvent.clear(dateInput);
        await userEvent.type(dateInput, date);

        const priceInput = await within(form).findByRole('textbox', { name: /price/i });
        await userEvent.clear(priceInput);
        await userEvent.type(priceInput, price);

        const saveButton = await within(form).findByRole('button', { name: /save/i });
        await userEvent.click(saveButton);

        const priceRegexp = new RegExp(`.*${price}.*`, 'i');
        await screen.findByRole('cell', { name: priceRegexp });
    });

    test('add split button opens add split form', async () => {
        renderTestInstrumentView();

        await openSplitAddForm();
    });

    test('add split form contains date field', async () => {
        renderTestInstrumentView();

        const form = await openSplitAddForm();
        await within(form).findByRole('textbox', { name: /date/i });
    });

    test('add split form contains numerator field', async () => {
        renderTestInstrumentView();

        const form = await openSplitAddForm();
        await within(form).findByRole('textbox', { name: /numerator/i });
    });

    test('add split form contains denominator field', async () => {
        renderTestInstrumentView();

        const form = await openSplitAddForm();
        await within(form).findByRole('textbox', { name: /denominator/i });
    });

    test('new split appears in view after add split form submit', async () => {
        renderTestInstrumentView();

        const form = await openSplitAddForm();

        const date = reformatDateTime('2023-01-01T03:23:00Z');
        const numerator = '6';
        const denominator = '2';

        const dateInput = within(form).getByRole('textbox', { name: /date/i });
        await userEvent.clear(dateInput);
        await userEvent.type(dateInput, date);

        const numeratorInput = within(form).getByRole('textbox', { name: /numerator/i });
        await userEvent.clear(numeratorInput);
        await userEvent.type(numeratorInput, numerator);

        const denominatorInput = within(form).getByRole('textbox', { name: /denominator/i });
        await userEvent.clear(denominatorInput);
        await userEvent.type(denominatorInput, denominator);

        const saveButton = within(form).getByRole('button', { name: /save/i });
        await userEvent.click(saveButton);

        const splitRegexp = new RegExp(`.*${numerator}:${denominator}.*`, 'i');
        await screen.findByRole('cell', { name: splitRegexp });
    });

    test('renders price history table', async () => {
        renderTestInstrumentView();

        await screen.findByLabelText(`Instrument ${testInstrument.id} prices table`);
    });

    test('prices table renders correct headers', async () => {
        renderTestInstrumentView();

        const pricesTable = await screen.findByRole('table', { name: `Instrument ${testInstrument.id} prices table` });

        await within(pricesTable).findByRole('columnheader', { name: /date/i });
        await within(pricesTable).findByRole('columnheader', { name: /price/i });
        await within(pricesTable).findByRole('columnheader', { name: /actions/i });
    });

    test('prices table renders all prices by default', async () => {
        renderTestInstrumentView();

        const pricesTable = await screen.findByRole('table', { name: `Instrument ${testInstrument.id} prices table` });
        const rows = await within(pricesTable).findAllByTestId('datarow');
        testInstrumentPrices.forEach((price, index) => {
            const row = rows[index];

            const dateRegexp = new RegExp(`.*${reformatDateTime(price.time)}`);
            within(row).getByRole('cell', { name: dateRegexp });
            
            const priceRegexp = new RegExp(`.*${price.price}.*`, 'i');
            within(row).getByRole('cell', { name: priceRegexp });
            within(row).getByRole('button', { name: /remove/i });
        })
    });

    test('prices table renders pagination controls', async () => {
        renderTestInstrumentView();

        await waitFor(() => {
            const controls = screen.queryAllByLabelText('Pagination controls');
            expect(controls.length).toBe(2);
        });
    });

    test('renders price aggregation dropdown', async () => {
        renderTestInstrumentView();

        const expectedOptions = ['all', 'hourly', 'weekly', 'monthly', 'yearly'];

        const select = await screen.findByLabelText(/price aggregation frequency/i);
        expectedOptions.forEach(option => {
            within(select).getByText(new RegExp(option, 'i'));
        });
    });

    test('prices table remove buttons remove price', async () => {
        renderTestInstrumentView();

        const pricesTable = await screen.findByRole('table', { name: `Instrument ${testInstrument.id} prices table` });
        const rows = await within(pricesTable).findAllByTestId('datarow');
        const removeButton = within(rows[0]).getByRole('button', { name: /remove/i });
        fireEvent.click(removeButton);

        await waitForElementToBeRemoved(rows[0]);
    });

    test('renders split history table', async () => {
        renderTestInstrumentView();

        await screen.findByLabelText(`Instrument ${testInstrument.id} splits table`);
    });

    test('splits table renders correct headers', async () => {
        renderTestInstrumentView();

        const splitsTable = await screen.findByRole('table', { name: `Instrument ${testInstrument.id} splits table` });
        const headers = ['date', 'ratio', 'status', 'actions'];
        for await (const header of headers) {
            const regexp = new RegExp(`.*${header}.*`, 'i');
            await within(splitsTable).findByRole('columnheader', { name: regexp });
        }
    });

    test('splits table renders instrument splits', async () => {
        renderTestInstrumentView();

        const splitsTable = await screen.findByRole('table', { name: `Instrument ${testInstrument.id} splits table` });
        const rows = await within(splitsTable).findAllByTestId('datarow');
        testSplits.forEach((split, index) => {
            const row = rows[index];

            const timeRegexp = new RegExp(`.*${reformatDateTime(split.time)}.*`, 'i');
            within(row).getByRole('cell', { name: timeRegexp });

            const statusRegexp = new RegExp(split.status, 'i');
            within(row).getByRole('cell', { name: statusRegexp });

            const ratioRegexp = new RegExp(`${split.splitRatioNumerator}:${split.splitRatioDenominator}`);
            within(row).getByRole('cell', { name: ratioRegexp });

            if(split.status === 'processed') {
                within(row).getByRole('button', { name: /rollback/i });
            }            
        });
    });

    test('split rollback button changes status to rollback requested', async () => {
        renderTestInstrumentView();

        const firstProcessedSplitIndex = testSplits.findIndex(c => c.status === 'processed');

        const splitsTable = await screen.findByRole('table', { name: `Instrument ${testInstrument.id} splits table` });
        const rows = await within(splitsTable).findAllByTestId('datarow');
        const processedSplitRow = rows[firstProcessedSplitIndex];
        const rollbackButton = within(processedSplitRow).getByRole('button', { name: /rollback/i });
        fireEvent.click(rollbackButton);

        await within(processedSplitRow).findByRole('cell', { name: /rollback requested/i });
    });
});