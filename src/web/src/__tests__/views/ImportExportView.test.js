import { fireEvent, screen } from '@testing-library/react';
import React from 'react';
import { Route, Router } from 'react-router-dom';
import { renderWithProviders } from '../utils';
import { createMemoryHistory } from 'history';
import ImportExportView from '../../components/views/ImportExportView';

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

    test('renders data import form', async () => {
        renderTestImportExportView();

        await screen.findByRole('form', { name: /import csv data form/i });
    });

    test('renders data import history', async () => {
        renderTestImportExportView();

        await screen.findByRole('table', { name: /imports table/i });
    })
})