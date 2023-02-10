import React from 'react';
import { testCharts, testInstruments, testPortfolios, testPositions } from '../mocks/testData';
import { fireEvent, screen, waitFor, within } from '@testing-library/react';
import ChartView from '../../components/views/ChartView';
import { createTestMemoryRouter, renderWithProviders } from '../utils';
import userEvent from '@testing-library/user-event';

const testChart = testCharts[0];
const testPortfolio = testPortfolios[0];

const renderTestChartView = () => {
    const router = createTestMemoryRouter('/charts/view/:chartId', `/charts/view/${testChart.id}`, <ChartView />);

    renderWithProviders(
        <ChartView />,
        {
            router
        }
    );
}

const openChartInfoForm = async () => {
    const renameButton = await screen.findByRole('button', { name: /rename/i });
    fireEvent.click(renameButton);

    return await screen.findByRole('form', { name: /edit chart information form/i });
}

const openPositionsPickerForTestPortfolio = async () => {
    const portfolioPicker = await screen.findByLabelText(/portfolio picker/i);

    const positionsButtons = await within(portfolioPicker).findAllByRole('button', { name: /positions/i });
    fireEvent.click(positionsButtons[0]);

    return await screen.findByLabelText(/position picker/i);
}

const addPortfolioLine = async () => {
    const picker = await screen.findByLabelText(/portfolio picker/i);

    const addButtons = await within(picker).findAllByRole('button', { name: /^add$/i });
    fireEvent.click(addButtons[0]);
}

const addPositionLine = async () => {
    const picker = await screen.findByLabelText(/position picker/i);

    const addButtons = await within(picker).findAllByRole('button', { name: /^add$/i });
    fireEvent.click(addButtons[0]);
}

const addInstrumentLine = async () => {
    const picker = await screen.findByLabelText(/instrument picker/i);

    const addButtons = await within(picker).findAllByRole('button', { name: /^add$/i });
    fireEvent.click(addButtons[0]);
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

    test('adding instrument from instrument picker opens line configuration form', async () => {
        renderTestChartView();

        await addInstrumentLine();
        await screen.findByRole('form', { name: /edit chart line form/i });
    });

    test('renders portfolio picker containing available portfolios', async () => {
        renderTestChartView();

        const picker = await screen.findByLabelText(/portfolio picker/i);

        for await (const portfolio of testPortfolios) {
            const regex = new RegExp(`${portfolio.name} picker item`, 'i');
            await within(picker).findByLabelText(regex);
        }
    });

    test('adding a portfolio line opens line configuration form', async () => {
        renderTestChartView();

        await addPortfolioLine();
        await screen.findByRole('form', { name: /edit chart line form/i });
    });

    test('positions button in portfolio picker opens position picker', async () => {
        renderTestChartView();

        await openPositionsPickerForTestPortfolio();
    });

    test('position picker contains portfolio positions', async () => {
        renderTestChartView();

        const picker = await openPositionsPickerForTestPortfolio();
        for await(const position of testPositions.filter(p => p.portfolioId === testPortfolio.id)) {
            const regex = new RegExp(`${position.instrument.name} position picker item`, 'i');
            await within(picker).findByLabelText(regex);
        }
    });

    test('adding position from position picker opens line configuration form', async () => {
        renderTestChartView();

        await openPositionsPickerForTestPortfolio();
        await addPositionLine();
        await screen.findByRole('form', { name: /edit chart line form/i })
    });

    test('adding all positions from position picker adds all position lines to chart', async () => {
        renderTestChartView();

        const picker = await openPositionsPickerForTestPortfolio();
        const button = await within(picker).findByRole('button', { name: /add all positions/i });
        fireEvent.click(button);

        const chart = await screen.findByLabelText('chart');
        const lines = await within(chart).findAllByRole('line');
        expect(lines.length)
            .toBe(testChart.lines.length + testPositions.filter(p => p.portfolioId === testPortfolio.id).length);
    })

    test('line configuration form contains width slider', async () => {
        renderTestChartView();

        await addInstrumentLine();
        const form = await screen.findByRole('form', { name: /edit chart line form/i });
        within(form).getByRole('slider', { name: /width/i });
    });

    test('line configuration form contains color picker', async () => {
        renderTestChartView();

        await addInstrumentLine();
        const form = await screen.findByRole('form', { name: /edit chart line form/i });
        within(form).getByTestId('line-color-picker');
    });

    test('line configuration form contains dash radio buttons', async () => {
        renderTestChartView();

        await addInstrumentLine();
        const form = await screen.findByRole('form', { name: /edit chart line form/i });
        within(form).getByRole('radio', { name: /solid/i });
        within(form).getByRole('radio', { name: /dashed/i });
        within(form).getByRole('radio', { name: /dotted/i });
    });

    test('saving line configuration form after adding a line adds the configured line to the chart', async () => {
        renderTestChartView();

        await addInstrumentLine();
        const form = await screen.findByRole('form', { name: /edit chart line form/i });

        const widthSlider = within(form).getByRole('slider', { name: /width/i });
        fireEvent.change(widthSlider, { target: { value: 3 }});

        const colorPicker = within(form).getByTestId('line-color-picker');
        fireEvent.input(colorPicker, { target: { value: '#ffffff' }}); // no better way in RTL to simulate color picker 

        const dashedRadioButton = within(form).getByRole('radio', { name: /dashed/i });
        await userEvent.click(dashedRadioButton);

        const saveButton = within(form).getByRole('button', { name: /save/i });
        fireEvent.click(saveButton);

        const chart = await screen.findByLabelText('chart');
        await waitFor(() => {
            const lines = within(chart).getAllByRole('line');
            expect(lines.length).toBe(testChart.lines.length + 1);

            const addedLine = lines[lines.length - 1];

            expect(addedLine).toHaveStyle('stroke: #ffffff');
            expect(addedLine).toHaveStyle('stroke-width: 3');
            expect(addedLine).toHaveStyle('stroke-dasharray: 6');
        });
    });

    test('renders rename button', async () => {
        renderTestChartView();

        await screen.findByRole('button', { name: /rename/i });
    });

    test('rename button opens edit chart information form', async () => {
        renderTestChartView();

        await openChartInfoForm();
    });

    test('edit chart information form contains name input', async () => {
        renderTestChartView();

        const form = await openChartInfoForm();
        within(form).getByRole('textbox', { name: /name/i });
    });

    test('chart name changes after edit chart information form is submitted', async () => {
        renderTestChartView();

        const newName = 'New chart name';

        const form = await openChartInfoForm();
        const nameInput = within(form).getByRole('textbox', { name: /name/i });
        await userEvent.clear(nameInput);
        await userEvent.type(nameInput, newName);

        const saveButton = within(form).getByRole('button', { name: /save/i });
        await userEvent.click(saveButton);

        const nameRegexp = new RegExp(`.*${newName}.*`, 'i');
        await screen.findByText(nameRegexp);
    });


});