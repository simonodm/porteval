import React from 'react';
import { createMemoryHistory } from 'history';
import { testCharts, testInstruments, testPortfolios } from '../mocks/testData';
import { Router, Route } from 'react-router-dom';
import { screen, waitFor, within } from '@testing-library/react';
import ChartView from '../../components/views/ChartView';
import { renderWithProviders } from '../utils';
import userEvent from '@testing-library/user-event';

const testChart = testCharts[0];

const renderTestChartView = () => {
    const history = createMemoryHistory();
    history.push(`/charts/view/${testChart.id}`);

    renderWithProviders(
        <Router history={history}>
            <Route path='/charts/view/:chartId'>
                <ChartView />
            </Route>
        </Router>
    )
}

describe('Chart view', () => {
    test('renders type dropdown', async () => {
        renderTestChartView();

        await screen.findByRole('combobox', { name: /chart type/i });
    });

    test('renders currency dropdown for price charts', async () => {
        renderTestChartView();

        await screen.findByRole('combobox', { name: /currency/i });
    });

    test('renders date range inputs', async () => {
        renderTestChartView();

        await screen.findByRole('textbox', { name: /range start/i });
        await screen.findByRole('textbox', { name: /range end/i });
    });

    test('renders to-date range buttons', async () => {
        renderTestChartView();

        const toDateRanges = ['1d', '5d', '1m', '3m', '6m', '1y'];
        for await (const range of toDateRanges) {
            const regex = new RegExp(range, 'i');
            await screen.findByRole('button', { name: regex });
        }
    });

    test('renders aggregation frequency when chart type is changed to aggregated', async () => {
        renderTestChartView();

        const typeInput = await screen.findByRole('combobox', { name: /chart type/i });
        await userEvent.selectOptions(typeInput, 'aggregatedProfit');

        await screen.findByRole('combobox', { name: /frequency/i });
    });

    test('hides currency dropdown when chart type is changed to performance', async () => {
        renderTestChartView();

        const typeInput = await screen.findByRole('combobox', { name: /chart type/i });
        await userEvent.selectOptions(typeInput, 'performance');

        await waitFor(() => {
            const currencyInput = screen.queryByRole('combobox', { name: /currency/i });
            expect(currencyInput).toBeNull();
        });
    });

    test('renders chart with SVG lines', async () => {
        renderTestChartView();

        const chart = await screen.findByLabelText('chart');

        chart.lines?.forEach(line => {
            within(chart).getByLabelText(`${line.name} line`);
        });
    });

    test('renders instrument picker containing available instruments', async () => {
        renderTestChartView();

        const picker = await screen.findByLabelText(/instrument picker/i);

        for await (const instrument of testInstruments) {
            const regex = new RegExp(`${instrument.name} picker item`, 'i');
            await within(picker).findByLabelText(regex);
        }
    });

    test('renders portfolio picker containing available portfolios', async () => {
        renderTestChartView();

        const picker = await screen.findByLabelText(/portfolio picker/i);

        for await (const portfolio of testPortfolios) {
            const regex = new RegExp(`${portfolio.name} picker item`, 'i');
            await within(picker).findByLabelText(regex);
        }
    });
});