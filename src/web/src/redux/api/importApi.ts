import { ImportEntry, ImportStartedResponse } from '../../types';
import { portEvalApi } from './portEvalApi';

/**
 * PortEval's data import API definition.
 * @category API
 */
const importApi = portEvalApi.injectEndpoints({
    endpoints: (build) => ({
        getAllImports: build.query<Array<ImportEntry>, void>({
            query: () => 'imports',
            providesTags: (result) =>
                result
                    ? ['Imports']
                    : []
        }),
        getImport: build.query<ImportEntry, string>({
            query: (id) => `imports/${id}`,
            providesTags: (result) =>
                result
                    ? [{ type: 'Import', id: result.importId }]
                    : []
        }),
        uploadImportFile: build.mutation<ImportStartedResponse, FormData>({
            query: (form) => ({
                url: 'imports',
                method: 'POST',
                body: form
            }),
            invalidatesTags: (result, error) =>
                !error
                    ? ['Imports']
                    : []
        })
    })
});

export const {
    useGetAllImportsQuery,
    useGetImportQuery,
    useUploadImportFileMutation
} = importApi;