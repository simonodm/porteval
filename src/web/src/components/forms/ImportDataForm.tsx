import React, { useState } from 'react'
import FileUpload from './fields/FileUpload'
import TemplateTypeDropdown from './fields/TemplateTypeDropdown';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

import { useUploadImportFileMutation } from '../../redux/api/importApi';
import { onSuccessfulResponse } from '../../utils/queries';
import { TemplateType } from '../../types';

type Props = {
    /**
     * A callback which is invoked whenever the form is successfully submitted.
     */
    onSuccess?: () => void;
}

/**
 * Renders a CSV data import form.
 * 
 * @category Forms
 * @subcategory Forms
 * @component
 */
function ImportDataForm({ onSuccess }: Props): JSX.Element {
    const [file, setFile] = useState<File | undefined>(undefined);
    const [templateType, setTemplateType] = useState<TemplateType>('portfolios');

    const [uploadFile] = useUploadImportFileMutation();

    const handleFileSelect = (file: File) => {
        setFile(file);
    }

    const handleTemplateExport = (e: React.FormEvent) => {
        const templateUrl = `/api/imports/template?templateType=${templateType}`;

        fetch(templateUrl)
            .then(res => res.blob())
            .then(blob => {
                const file = URL.createObjectURL(blob);
                location.assign(file);
            });

        e.preventDefault();
    }

    const handleUpload = (e: React.FormEvent) => {
        if(file) {
            const formData = new FormData();
            formData.append('file', file);
            formData.append('type', templateType);

            uploadFile(formData)
                .then(res => onSuccessfulResponse(res, onSuccess));
        }

        e.preventDefault();
    }
    
    return (
        <Form aria-label="Import CSV data form">
            <TemplateTypeDropdown className="mb-2" label='Import template' onChange={setTemplateType} />
            <Button
                className="mb-4"
                variant="primary"
                size="sm"
                onClick={handleTemplateExport}
            >
                Download template
            </Button>
            <FileUpload className="mb-2" label="Choose import file" onFileSelected={handleFileSelect} />
            <Button
                className="mb-2"
                variant="primary"
                role="button"
                onClick={handleUpload}
            >
                Upload
            </Button>
        </Form>
    )
}

export default ImportDataForm;