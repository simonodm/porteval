import React, { useState } from 'react'

import { useUploadImportFileMutation } from '../../redux/api/importApi';

import { onSuccessfulResponse } from '../../utils/queries';

import { TemplateType } from '../../types';

import FileUpload from './fields/FileUpload'
import TemplateTypeDropdown from './fields/TemplateTypeDropdown';

type Props = {
    onSuccess?: () => void;
}

export default function ImportDataForm({ onSuccess }: Props): JSX.Element {
    const [file, setFile] = useState<File | undefined>(undefined);
    const [templateType, setTemplateType] = useState<TemplateType>('portfolios');

    const [uploadFile] = useUploadImportFileMutation();

    const handleFileUpload = (file: File) => {
        setFile(file);
    }

    const handleSubmit = (e: React.FormEvent) => {
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
        <form onSubmit={handleSubmit}>
            <TemplateTypeDropdown onChange={setTemplateType} />
            <FileUpload label="Choose import file" onUpload={handleFileUpload} />
            <button 
                className="btn btn-primary"
                role="button"
            >Upload
            </button>
        </form>
    )
}