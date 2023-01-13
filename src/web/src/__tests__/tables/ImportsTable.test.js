import React from 'react';
import ImportsTable from '../../components/tables/ImportsTable';
import { screen, within } from '@testing-library/react';
import { testDataImports } from '../mocks/testData';
import { reformatDateTime, renderWithProviders } from '../utils';

describe('Imports table', () => {
    test('renders correct headers', async () => {
        renderWithProviders(<ImportsTable />);

        const headers = ['time', 'template', 'status', 'status message', 'error log'];
        for await (const header of headers) {
            const regexp = new RegExp(`^${header}$`, 'i');
            await screen.findByRole('columnheader', { name: regexp });
        }
    });

    test('renders import entries', async () => {
        renderWithProviders(<ImportsTable />);

        // table rows and test imports are matched by index
        const rows = await screen.findAllByTestId('datarow');
        testDataImports.forEach((dataImport, index) => {
            const row = rows[index];

            const timeRegexp = new RegExp(`.*${reformatDateTime(dataImport.time)}.*`, 'i');
            within(row).getByRole('cell', { name: timeRegexp });

            within(row).getByRole('cell', { name: dataImport.templateType });
            within(row).getByRole('cell', { name: dataImport.status });

            if(dataImport.statusDetails) {
                within(row).getByRole('cell', { name: dataImport.statusDetails });
            }

            if(dataImport.errorLogAvailable) {
                within(row).getByRole('button', { name: /download/i });
            } else {
                within(row).getByRole('cell', { name: /no error log available/i });
            }
        });
    });
});