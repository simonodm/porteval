/* eslint-disable react/prop-types */
/* eslint-disable react/display-name */

import React from 'react';
import InstrumentPricesTable from '../../components/tables/InstrumentPricesTable';
import { fireEvent, screen, waitFor, waitForElementToBeRemoved, within } from '@testing-library/react';
import { testInstruments, testPrices } from '../mocks/testData';
import { reformatDateTime, renderWithProviders } from '../utils';

jest.mock('react-select', () => ({ options, value, onChange }) => {
    function handleChange(event) {
      const option = options.find(
        option => option.value === event.currentTarget.value
      );
      onChange(option);
    }
    return (
        <select data-testid="select" onChange={handleChange} value={value}>
            {options.map(({ label, value }) => (
                <option key={value} value={value}>
                    {label}
                </option>
        ))}
        </select>
    );
  });

describe('Instrument prices table', () => {
    const testInstrument = testInstruments[0];
    const testInstrumentPrices = testPrices.filter(p => p.instrumentId === testInstrument.id);

    test('renders correct headers', async () => {
        renderWithProviders(
            <InstrumentPricesTable
                currencyCode={testInstrument.currencyCode}
                instrumentId={testInstrument.id}
            />
        );

        await screen.findByRole('columnheader', { name: /date/i });
        await screen.findByRole('columnheader', { name: /price/i });
        await screen.findByRole('columnheader', { name: /actions/i });
    });

    test('renders all prices by default', async () => {
        renderWithProviders(
            <InstrumentPricesTable
                currencyCode={testInstrument.currencyCode}
                instrumentId={testInstrument.id}
            />
        );

        const rows = await screen.findAllByTestId('datarow');
        testInstrumentPrices.forEach((price, index) => {
            const row = rows[index];

            const dateRegexp = new RegExp(`.*${reformatDateTime(price.time)}`);
            within(row).getByRole('cell', { name: dateRegexp });
            
            const priceRegexp = new RegExp(`.*${price.price}.*`, 'i');
            within(row).getByRole('cell', { name: priceRegexp });
            within(row).getByRole('button', { name: /remove/i });
        })
    });

    test('renders pagination controls', async () => {
        renderWithProviders(
            <InstrumentPricesTable
                currencyCode={testInstrument.currencyCode}
                instrumentId={testInstrument.id}
            />
        );

        await waitFor(() => {
            const controls = screen.queryAllByLabelText('Pagination controls');
            expect(controls.length).toBe(2);
        });
    });

    test('renders aggregation dropdown', async () => {
        renderWithProviders(
            <InstrumentPricesTable
                currencyCode={testInstrument.currencyCode}
                instrumentId={testInstrument.id}
            />
        );

        const expectedOptions = ['all', 'hourly', 'weekly', 'monthly', 'yearly'];

        const select = await screen.findByTestId('select');
        expectedOptions.forEach(option => {
            within(select).getByText(new RegExp(option, 'i'));
        });
    });

    test('remove buttons removes price', async () => {
        renderWithProviders(
            <InstrumentPricesTable
                currencyCode={testInstrument.currencyCode}
                instrumentId={testInstrument.id}
            />
        );

        const rows = await screen.findAllByTestId('datarow');
        const removeButton = within(rows[0]).getByRole('button', { name: /remove/i });
        fireEvent.click(removeButton);

        await waitForElementToBeRemoved(rows[0]);
    })
})