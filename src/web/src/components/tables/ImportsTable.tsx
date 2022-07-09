import React from 'react';

import { useGetAllImportsQuery } from '../../redux/api/importApi';
import { checkIsError, checkIsLoaded } from '../../utils/queries';
import useUserSettings from '../../hooks/useUserSettings';
import { formatDateTimeString } from '../../utils/string';
import LoadingWrapper from '../ui/LoadingWrapper';
import { ImportEntry } from '../../types';

export default function ImportsTable(): JSX.Element {
    const imports = useGetAllImportsQuery();

    const [userSettings] = useUserSettings();

    const isLoaded = checkIsLoaded(imports);
    const isError = checkIsError(imports);

    const handleLogDownload = (importEntry: ImportEntry) => {
        fetch(importEntry.errorLogUrl)
            .then(res => res.blob())
            .then(blob => {
                const file = URL.createObjectURL(blob);
                location.assign(file);
            })
    }

    return (
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <table className="w-100 entity-list">
                <thead>
                    <tr>
                        <th>Time</th>
                        <th>Template</th>
                        <th>Status</th>
                        <th>Status message</th>
                        <th>Error log</th>
                    </tr>
                </thead>
                <tbody>
                    {imports.data?.map(importEntry =>
                        <tr key={importEntry.importId}>
                            <td>
                                {formatDateTimeString(
                                    importEntry.time,
                                    userSettings.dateFormat + ' ' + userSettings.timeFormat
                                )}
                            </td>
                            <td>{importEntry.templateType}</td>
                            <td>{importEntry.status}</td>
                            <td>{importEntry.statusDetails}</td>
                            <td>
                                {
                                    importEntry.errorLogAvailable
                                        ? (
                                            <button className="btn btn-primary btn-sm"
                                                onClick={() => handleLogDownload(importEntry)}
                                            >Download
                                            </button>
                                        )
                                        : 'No error log available.'
                                }
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        </LoadingWrapper>
        
    )
}