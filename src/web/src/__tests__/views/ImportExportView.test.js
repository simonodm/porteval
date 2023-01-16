import { screen, within } from '@testing-library/react';
import React from 'react';
import { Route, Router } from 'react-router-dom';
import { renderWithProviders, reformatDateTime } from '../utils';
import { createMemoryHistory } from 'history';
import ImportExportView from '../../components/views/ImportExportView';
import { testDataImports } from '../mocks/testData';

const renderTestImportExportView = () => {
    const history = createMemoryHistory();
    history.push('/import');

    renderWithProviders(
        <Router history={history}>
            <Route>
                <ImportExportView />
            </Route>            
        </Router>
    );
}

describe('Import/export view', () => {
    test('renders data export form', async () => {
        renderTestImportExportView();

        await screen.findByRole('form', { name: /export csv data form/i });
    });

    test('renders export data type select', async () => {
        renderTestImportExportView();

        await screen.findByRole('combobox', { name: /export data type/i });
    });

    test('renders data import form', async () => {
        renderTestImportExportView();

        await screen.findByRole('form', { name: /import csv data form/i });
    });

    test('renders data import history', async () => {
        renderTestImportExportView();

        await screen.findByRole('table', { name: /imports table/i });
    });

    test('import history table renders correct headers', async () => {
        renderTestImportExportView();

        const importHistoryTable = await screen.findByRole('table', { name: /imports table/i });
        const headers = ['time', 'template', 'status', 'status message', 'error log'];
        for await (const header of headers) {
            const regexp = new RegExp(`^${header}$`, 'i');
            within(importHistoryTable).getByRole('columnheader', { name: regexp });
        }
    });

    test('import history table renders import entries', async () => {
        renderTestImportExportView();

        const importHistoryTable = await screen.findByRole('table', { name: /imports table/i });
        const rows = await within(importHistoryTable).findAllByTestId('datarow');
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
})