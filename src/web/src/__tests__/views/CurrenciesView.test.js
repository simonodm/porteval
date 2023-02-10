import React from 'react';
import CurrenciesView from '../../components/views/CurrenciesView';
import userEvent from '@testing-library/user-event';
import { screen, within } from '@testing-library/react';
import { renderWithProviders } from '../utils';
import { testCurrencies, testExchangeRates } from '../mocks/testData';

const getLatestExchangeRates = (sourceCurrencyCode) => {
    const latestExchangeRates = {};

    testExchangeRates.forEach(er => {
        if(er.currencyFromCode !== sourceCurrencyCode) {
            return;
        }

        if(!latestExchangeRates.hasOwnProperty(er.currencyToCode)) {
            latestExchangeRates[er.currencyToCode] = er;
        }
    });

    return Object.values(latestExchangeRates);
}

describe('Currencies view', () => {
    const defaultCurrency = testCurrencies.find(c => c.isDefault);

    test('renders currency selection', async () => {
        renderWithProviders(<CurrenciesView />);

        await screen.findByLabelText(/choose default currency/i);
    });

    test('currency selection has default currency selected', async () => {
        renderWithProviders(<CurrenciesView />);

        const currencySelect = await screen.findByLabelText(/choose default currency/i);
        expect(currencySelect.value).toBe(defaultCurrency.code);
    });

    test('renders exchange rates table', async () => {
        renderWithProviders(<CurrenciesView />);

        await screen.findByRole('table', { name: /Exchange rates table/i });
    });

    test('exchange rates table renders correct headers', async () => {
        renderWithProviders(<CurrenciesView />);

        const exchangeRatesTable = await screen.findByRole('table', { name: /exchange rates table/i });

        within(exchangeRatesTable).getByRole('columnheader', { name: /currency/i });
        within(exchangeRatesTable).getByRole('columnheader', { name: /exchange rate/i });
    });

    test('exchange rates table renders exchange rates from default currency', async () => {
        renderWithProviders(<CurrenciesView />);

        const latestExchangeRates = getLatestExchangeRates(defaultCurrency.code);
        const exchangeRatesTable = await screen.findByRole('table', { name: /exchange rates table/i });

        for await (const rate of latestExchangeRates) {
            const regex = new RegExp(`.*${rate.exchangeRate}.*`, 'i');
            await within(exchangeRatesTable).findByRole('cell', { name: regex });
        }
    });

    test('changing default currency displays exchange rates from the new default currency', async () => {
        renderWithProviders(<CurrenciesView />);

        const newDefaultCurrencyCode = 'EUR';

        const currencySelect = await screen.findByLabelText(/choose default currency/i);
        userEvent.selectOptions(currencySelect, newDefaultCurrencyCode);

        const saveButton = await screen.findByRole('button', { name: /save/i });
        userEvent.click(saveButton);

        const exchangeRatesTable = await screen.findByRole('table', { name: /exchange rates table/i });
        const newLatestExchangeRates = getLatestExchangeRates(newDefaultCurrencyCode);

        for await (const rate of newLatestExchangeRates) {
            const regex = new RegExp(`.*${rate.exchangeRate}.*`, 'i');
            await within(exchangeRatesTable).findByRole('cell', { name: regex });
        }
    })
})