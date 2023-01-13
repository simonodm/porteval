import React from 'react';
import TransactionsTable from '../../components/tables/TransactionsTable';
import { fireEvent, screen, waitForElementToBeRemoved, within } from '@testing-library/react';
import { reformatDateTime, renderWithProviders } from '../utils';
import { testPositions, testTransactions } from '../mocks/testData';

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

        const editButtons = await screen.findAllByRole('button', { name: /edit/i });
        fireEvent.click(editButtons[0]);

        await screen.findByLabelText('Edit transaction form');
    });

    test('remove button removes transaction', async () => {
        renderWithProviders(<TransactionsTable positionId={testParentPosition.id} currencyCode='USD' />);

        const rows = await screen.findAllByTestId('datarow');
        const removeButton = within(rows[0]).getByRole('button', { name: /remove/i });
        fireEvent.click(removeButton);

        await waitForElementToBeRemoved(rows[0]);
    });
});