import React from 'react';
import ExchangeRatesTable from '../../components/tables/ExchangeRatesTable';
import { screen } from '@testing-library/react';
import { testCurrencies, testExchangeRates } from '../mocks/testData';
import { renderWithProviders } from '../utils';

const getTargetCurrencies = () => {
    return testExchangeRates.reduce((prev, curr) => {
        return prev.add(curr.currencyToCode);
    }, new Set());
}

const getLatestExchangeRates = (sourceCurrencyCode) => {
    return testExchangeRates.filter(er => er.currencyFromCode === sourceCurrencyCode);
}

describe('Exchange rates table', () => {
    const sourceCurrencyCode = testCurrencies.find(c => c.isDefault).code;

    test('renders correct headers', async () => {
        renderWithProviders(<ExchangeRatesTable sourceCurrencyCode={sourceCurrencyCode} />);

        await screen.findByRole('columnheader', { name: /.*currency.*/i });
        await screen.findByRole('columnheader', { name: /.*exchange rate.*/i });
    });

    test('renders all target currencies', async () => {
        renderWithProviders(<ExchangeRatesTable sourceCurrencyCode={sourceCurrencyCode} />);

        const currencyCodes = getTargetCurrencies();

        for await (const code of currencyCodes) {
            await screen.findByRole('cell', { name: code });
        }
    });

    test('renders current exchange rate for each target currency', async () => {
        renderWithProviders(<ExchangeRatesTable sourceCurrencyCode={sourceCurrencyCode} />);

        const latestExchangeRates = getLatestExchangeRates('USD');
        for await (const rate of latestExchangeRates) {
            const regex = new RegExp(`.*${rate.exchangeRate}.*`, 'i');
            await screen.findByRole('cell', { name: regex });
        }
    });
});