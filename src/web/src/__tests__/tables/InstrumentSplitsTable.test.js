import React from 'react';
import InstrumentSplitsTable from '../../components/tables/InstrumentSplitsTable';
import { fireEvent, screen, within } from '@testing-library/react';
import { testInstruments, testInstrumentSplits } from '../mocks/testData';
import { reformatDateTime, renderWithProviders } from '../utils';

describe('Instrument splits table', () => {
    const testInstrument = testInstruments[0];
    const testSplits = testInstrumentSplits.filter(s => s.instrumentId === testInstrument.id);

    test('renders correct headers', async () => {
        renderWithProviders(<InstrumentSplitsTable instrumentId={testInstrument.id} />);

        const headers = ['date', 'ratio', 'status', 'actions'];
        for await (const header of headers) {
            const regexp = new RegExp(`.*${header}.*`, 'i');
            await screen.findByRole('columnheader', { name: regexp });
        }
    });

    test('renders instrument splits', async () => {
        renderWithProviders(<InstrumentSplitsTable instrumentId={testInstrument.id} />);

        const rows = await screen.findAllByTestId('datarow');
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

    test('rollback button changes status to rollback requested', async () => {
        renderWithProviders(<InstrumentSplitsTable instrumentId={testInstrument.id} />);

        const firstProcessedSplitIndex = testSplits.findIndex(c => c.status === 'processed');

        const rows = await screen.findAllByTestId('datarow');
        const processedSplitRow = rows[firstProcessedSplitIndex];
        const rollbackButton = within(processedSplitRow).getByRole('button', { name: /rollback/i });
        fireEvent.click(rollbackButton);

        await within(processedSplitRow).findByRole('cell', { name: /rollback requested/i });
    });
})