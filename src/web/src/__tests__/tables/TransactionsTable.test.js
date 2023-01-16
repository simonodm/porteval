import React from 'react';
import TransactionsTable from '../../components/tables/TransactionsTable';
import userEvent from '@testing-library/user-event';
import { fireEvent, screen, waitForElementToBeRemoved, within } from '@testing-library/react';
import { reformatDateTime, renderWithProviders } from '../utils';
import { testPositions, testTransactions } from '../mocks/testData';

const openEditTransactionForm = async () => {
    const editButtons = await screen.findAllByRole('button', { name: /edit/i });
    fireEvent.click(editButtons[0]);

    return await screen.findByRole('form', { name: /edit transaction form/i });
}

describe('Transactions table', () => {
    const testParentPosition = testPositions[0];
    const testPositionTransactions = testTransactions.filter(t => t.positionId === testParentPosition.id);

    test('renders correct headers', async () => {
        renderWithProviders(<TransactionsTable positionId={testParentPosition.id} currencyCode='USD' />);

        const headers = ['time', 'amount', 'price', 'actions'];
        for await (const header of headers) {
            const regexp = new RegExp(header, 'i');
            await screen.findByRole('columnheader', { name: regexp });
        }
    });

    test('renders each transaction', async () => {
        renderWithProviders(<TransactionsTable positionId={testParentPosition.id} currencyCode='USD' />);

        const rows = await screen.findAllByTestId('datarow');
        testPositionTransactions.forEach((transaction, index) => {
            const row = rows[index];

            const timeRegexp = new RegExp(reformatDateTime(transaction.time), 'i');
            within(row).getByRole('cell', { name: timeRegexp });

            within(row).getByRole('cell', { name: transaction.amount });

            const priceRegexp = new RegExp(`.*${transaction.price}.*`, 'i');
            within(row).getByRole('cell', { name: priceRegexp });

            within(row).getByRole('button', { name: /edit/i });
            within(row).getByRole('button', { name: /remove/i });
        })
    });

    test('edit button opens transaction edit form', async () => {
        renderWithProviders(<TransactionsTable positionId={testParentPosition.id} currencyCode='USD' />);

        await openEditTransactionForm();
    });

    test('transaction edit form contains editable amount input', async () => {
        renderWithProviders(<TransactionsTable positionId={testParentPosition.id} currencyCode='USD' />);

        const form = await openEditTransactionForm();
        const amountInput = within(form).getByRole('textbox', { name: /amount/i });
        expect(amountInput).toBeEnabled();
    });

    test('transaction edit form contains editable price input', async () => {
        renderWithProviders(<TransactionsTable positionId={testParentPosition.id} currencyCode='USD' />);

        const form = await openEditTransactionForm();
        const priceInput = within(form).getByRole('textbox', { name: /price/i });
        expect(priceInput).toBeEnabled();
    });

    test('transaction edit form contains editable date input', async () => {
        renderWithProviders(<TransactionsTable positionId={testParentPosition.id} currencyCode='USD' />);

        const form = await openEditTransactionForm();
        const dateInput = within(form).getByRole('textbox', { name: /date/i });
        expect(dateInput).toBeEnabled();
    });

    test('transaction edit form contains editable note input', async () => {
        renderWithProviders(<TransactionsTable positionId={testParentPosition.id} currencyCode='USD' />);

        const form = await openEditTransactionForm();
        const noteInput = within(form).getByRole('textbox', { name: /note/i });
        expect(noteInput).toBeEnabled();
    });

    test('edited transaction changes in table after edit transaction form is submitted', async () => {
        renderWithProviders(<TransactionsTable positionId={testParentPosition.id} currencyCode='USD' />);

        const newAmount = '0.5';
        const newPrice = '99.99';
        const newDate = reformatDateTime('2020-01-01T16:54:23Z');
        const newNote = 'edited transaction note';

        const form = await openEditTransactionForm();

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

        await screen.findByRole('cell', { name: amountRegexp});
        await screen.findByRole('cell', { name: priceRegexp });
        await screen.findByRole('cell', { name: newNote });
        await screen.findByRole('cell', { name: newDate });
    });

    test('remove button removes transaction', async () => {
        renderWithProviders(<TransactionsTable positionId={testParentPosition.id} currencyCode='USD' />);

        const rows = await screen.findAllByTestId('datarow');
        const removeButton = within(rows[0]).getByRole('button', { name: /remove/i });
        fireEvent.click(removeButton);

        await waitForElementToBeRemoved(rows[0]);
    });
});