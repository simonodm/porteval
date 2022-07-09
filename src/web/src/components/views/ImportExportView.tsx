import React from 'react';

import PageHeading from '../ui/PageHeading';
import ImportDataForm from '../forms/ImportDataForm';
import ImportsTable from '../tables/ImportsTable';
import ExportDataForm from '../forms/ExportDataForm';

export default function ImportExportView(): JSX.Element {
    return (
        <>
            <PageHeading heading="Data import and export" />
            <h5>Export</h5>
            <ExportDataForm />
            <h5>Import</h5>
            <ImportDataForm />
            <ImportsTable />
        </>
    )
}