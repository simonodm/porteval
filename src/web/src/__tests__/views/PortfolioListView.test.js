import React from 'react';
import PortfolioListView from '../../components/views/PortfolioListView';
import userEvent from '@testing-library/user-event';
import { fireEvent, screen, within } from '@testing-library/react';
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

const openCreatePortfolioForm = async () => {
    const createPortfolioButton = await screen.findByRole('button', { name: /create new portfolio/i });
    fireEvent.click(createPortfolioButton);

    return await screen.findByRole('form', { name: /create portfolio form/i });
}

const openEditPortfolioForm = async () => {
    const editPortfolioButton = await screen.findAllByRole('button', { name: /edit/i });
    fireEvent.click(editPortfolioButton[0]);

    return await screen.findByRole('form', { name: /edit portfolio form/i });
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

        await openCreatePortfolioForm();
    });

    test('create new portfolio form contains name field', async () => {
        renderTestPortfolioListView();
        
        const form = await openCreatePortfolioForm();
        within(form).getByRole('textbox', { name: /name/i });
    });

    test('create new portfolio form contains currency field', async () => {
        renderTestPortfolioListView();
        
        const form = await openCreatePortfolioForm();
        within(form).getByRole('combobox', { name: /currency/i });
    });

    test('create new portfolio form contains note field', async () => {
        renderTestPortfolioListView();
        
        const form = await openCreatePortfolioForm();
        within(form).getByRole('textbox', { name: /note/i });
    });

    test('created portfolio appears in view after create new portfolio form submit', async () => {
        renderTestPortfolioListView();

        const name = 'Form test portfolio';
        const currency = 'EUR';
        const note = 'Form test note';

        const form = await openCreatePortfolioForm();
        
        const nameInput = await within(form).findByRole('textbox', { name: /name/i });
        await userEvent.clear(nameInput);
        await userEvent.type(nameInput, name);

        const currencyInput = await within(form).findByRole('combobox', { name: /currency/i });
        await userEvent.selectOptions(currencyInput, currency);

        const noteInput = await within(form).findByRole('textbox', { name: /note/i });
        await userEvent.clear(noteInput);
        await userEvent.type(noteInput, note);

        const saveButton = await within(form).findByRole('button', { name: /save/i });
        await userEvent.click(saveButton);

        await screen.findByText(name);
    });

    test('edit portfolio button opens portfolio edit form on click', async () => {
        renderTestPortfolioListView();

        await openEditPortfolioForm();
    });

    test('edit portfolio form contains editable name field', async () => {
        renderTestPortfolioListView();
        
        const form = await openEditPortfolioForm();
        const nameInput = within(form).getByRole('textbox', { name: /name/i });
        expect(nameInput).toBeEnabled();
    });

    test('edit portfolio form contains editable currency field', async () => {
        renderTestPortfolioListView();
        
        const form = await openEditPortfolioForm();
        const currencyInput = within(form).getByRole('combobox', { name: /currency/i });
        expect(currencyInput).toBeEnabled();
    });

    test('edit portfolio form contains editable note field', async () => {
        renderTestPortfolioListView();
        
        const form = await openEditPortfolioForm();
        const noteInput = within(form).getByRole('textbox', { name: /note/i });
        expect(noteInput).toBeEnabled();
    });

    test('edited portfolio changes in view after edit portfolio form submit', async () => {
        renderTestPortfolioListView();

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
})