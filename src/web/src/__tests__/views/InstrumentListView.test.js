/* eslint-disable react/prop-types */
/* eslint-disable react/display-name */

import React from 'react';
import { fireEvent, screen, waitForElementToBeRemoved, within } from '@testing-library/react';
import { Route, Router } from 'react-router-dom';
import { renderWithProviders } from '../utils';
import { createMemoryHistory } from 'history';
import InstrumentListView from '../../components/views/InstrumentListView';
import { testInstruments } from '../mocks/testData';
import userEvent from '@testing-library/user-event';

const renderTestInstrumentListView = (preconfiguredHistory) => {
    const history = preconfiguredHistory ?? createMemoryHistory();
    history.push('/instruments')

    renderWithProviders(
        <Router history={history}>
            <Route path="/instruments">
                <InstrumentListView />
            </Route>
        </Router>
    );
}

const openCreateInstrumentForm = async () => {
    const createNewInstrumentButton = await screen.findByRole('button', { name: /create new instrument/i });
    fireEvent.click(createNewInstrumentButton);

    return await screen.findByRole('form', { name: /create instrument form/i });
}

const openEditInstrumentForm = async () => {
    const editButtons = await screen.findAllByRole('button', { name: /edit/i });
    fireEvent.click(editButtons[0]);

    return await screen.findByRole('form', { name: /edit instrument form/i });
}

jest.mock('react-select/creatable', () => ({'aria-label': ariaLabel, id, isDisabled, onChange, placeholder, value}) => {
    const handleChange = (e) => {
        onChange && onChange({
            label: e.target.value,
            value: e.target.value
        });
    }

    return (
        <input
            aria-label={ariaLabel}
            id={id}
            disabled={isDisabled}
            onChange={handleChange}
            placeholder={placeholder}
            value={value?.value}
        />
    );
});

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

        await openCreateInstrumentForm();
    });

    test('renders correct headers', async () => {
        renderTestInstrumentListView();

        const headers = ['name', 'symbol', 'currency', 'exchange', 'type', 'current price', 'actions'];
        for await (const header of headers) {
            const regexp = new RegExp(header, 'i');
            await screen.findByRole('columnheader', { name: regexp });
        }
    });

    test('renders instruments', async () => {
        renderTestInstrumentListView();

        const rows = await screen.findAllByTestId('datarow');
        testInstruments.forEach((instrument, index) => {
            const row = rows[index];

            within(row).getByRole('cell', { name: instrument.name });
            within(row).getByRole('cell', { name: instrument.symbol });
            within(row).getByRole('cell', { name: instrument.currencyCode });

            if(instrument.exchange) {
                within(row).getByRole('cell', { name: instrument.exchange });
            }

            const typeRegexp = new RegExp(instrument.type, 'i');
            within(row).getByRole('cell', { name: typeRegexp });
            
            if(instrument.currentPrice) {
                const priceRegexp = new RegExp(`.*${instrument.currentPrice}.*`, 'i');
                within(row).getByRole('cell', { name: priceRegexp });
            }

            within(row).getByRole('button', { name: /chart/i });
            within(row).getByRole('button', { name: /edit/i });
            within(row).getByRole('button', { name: /remove/i });
        });
    });

    test('renders pagination controls', async () => {
        renderTestInstrumentListView();

        await screen.findByLabelText('Pagination controls');
    });

    test('remove button removes instrument', async () => {
        renderTestInstrumentListView();

        const rows = await screen.findAllByTestId('datarow');
        const removeButton = within(rows[0]).getByRole('button', { name: /remove/i });
        fireEvent.click(removeButton);

        await waitForElementToBeRemoved(rows[0]);
    });

    test('edit button opens instrument edit form', async () => {
        renderTestInstrumentListView();

        await openEditInstrumentForm();
    });

    test('instrument edit form contains editable name field', async () => {
        renderTestInstrumentListView();

        const form = await openEditInstrumentForm();
        const nameInput = within(form).getByRole('textbox', { name: /name/i });
        expect(nameInput).toBeEnabled();
    });

    test('instrument edit form contains non-editable symbol field', async () => {
        renderTestInstrumentListView();

        const form = await openEditInstrumentForm();
        const symbolInput = within(form).getByRole('textbox', { name: /symbol/i });
        expect(symbolInput).toBeDisabled();
    });

    test('instrument edit form contains editable exchange field', async () => {
        renderTestInstrumentListView();

        const form = await openEditInstrumentForm();
        const exchangeInput = within(form).getByRole('textbox', { name: /exchange/i });
        expect(exchangeInput).toBeEnabled();
    });

    test('instrument edit form contains non-editable type field', async () => {
        renderTestInstrumentListView();

        const form = await openEditInstrumentForm();
        const typeInput = within(form).getByRole('combobox', { name: /instrument type/i });
        expect(typeInput).toBeDisabled();
    });

    test('instrument edit form contains editable note field', async () => {
        renderTestInstrumentListView();

        const form = await openEditInstrumentForm();
        const noteInput = within(form).getByRole('textbox', { name: /note/i });
        expect(noteInput).toBeEnabled();
    });

    test('edit instrument form modifies original instrument on submit', async () => {
        renderTestInstrumentListView();

        const form = await openEditInstrumentForm();

        const newName = 'Modified Apple Inc.';
        const newExchange = 'NASDAQTEST';
        const newNote = 'modified instrument note';

        const nameInput = within(form).getByRole('textbox', { name: /name/i });
        await userEvent.clear(nameInput);
        await userEvent.type(nameInput, newName);

        const exchangeInput = within(form).getByRole('textbox', { name: /exchange/i });
        await userEvent.clear(exchangeInput);
        await userEvent.type(exchangeInput, newExchange);

        const noteInput = within(form).getByRole('textbox', { name: /note/i });
        await userEvent.clear(noteInput);
        await userEvent.type(noteInput, newNote);

        const saveButton = within(form).getByRole('button', { name: /save/i });
        await userEvent.click(saveButton);

        await screen.findByRole('cell', { name: newName });
        await screen.findByRole('cell', { name: newExchange });
        await screen.findByRole('cell', { name: newNote });
    });

    test('chart button navigates to chart view', async () => {
        const history = createMemoryHistory();
        renderTestInstrumentListView(history);

        const chartButtons = await screen.findAllByRole('button', { name: /chart/i });
        fireEvent.click(chartButtons[0]);

        expect(history.location.pathname).toBe('/charts/view');
    });

    test('create instrument form renders name input', async () => {
        renderTestInstrumentListView();

        const form = await openCreateInstrumentForm();
        await within(form).findByRole('textbox', { name: /name/i });
    });

    test('create instrument form renders symbol input', async () => {
        renderTestInstrumentListView();

        const form = await openCreateInstrumentForm();
        await within(form).findByRole('textbox', { name: /symbol/i });
    });

    test('create instrument form renders exchange input', async () => {
        renderTestInstrumentListView();

        const form = await openCreateInstrumentForm();
        await within(form).findByRole('textbox', { name: /exchange/i });
    });

    test('create instrument form renders instrument type input', async () => {
        renderTestInstrumentListView();

        const form = await openCreateInstrumentForm();
        await within(form).findByRole('combobox', { name: /instrument type/i });
    });

    test('create instrument form renders currency input', async () => {
        renderTestInstrumentListView();

        const form = await openCreateInstrumentForm();
        await within(form).findByRole('combobox', { name: /currency/i });
    });

    test('create instrument form renders note input', async () => {
        renderTestInstrumentListView();

        const form = await openCreateInstrumentForm();
        await within(form).findByRole('textbox', { name: /note/i });
    });

    test('created instrument appears in list after create instrument form submit', async () => {
        renderTestInstrumentListView();

        const name = 'Test form instrument';
        const symbol = 'FORM';
        const exchange = 'NASDAQ';
        const type = 'stock';
        const currency = 'USD';
        const note = 'test form';

        const form = await openCreateInstrumentForm();

        const nameInput = within(form).getByRole('textbox', { name: /name/i });
        await userEvent.clear(nameInput);
        await userEvent.type(nameInput, name);

        const symbolInput = within(form).getByRole('textbox', { name: /symbol/i });
        await userEvent.clear(symbolInput);
        await userEvent.type(symbolInput, symbol);

        const exchangeInput = within(form).getByRole('textbox', { name: /exchange/i });
        await userEvent.type(exchangeInput, exchange);

        const typeInput = within(form).getByRole('combobox', { name: /instrument type/i });
        await userEvent.selectOptions(typeInput, type);

        const currencyInput = within(form).getByRole('combobox', { name: /currency/i });
        await userEvent.selectOptions(currencyInput, currency);

        const noteInput = within(form).getByRole('textbox', { name: /note/i });
        await userEvent.clear(noteInput);
        await userEvent.type(noteInput, note);

        const saveButton = within(form).getByRole('button', { name: /save/i });
        await userEvent.click(saveButton);


        await screen.findByText(name);
    })
})