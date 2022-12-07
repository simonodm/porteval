import React, { useState } from 'react';
import { TemplateType } from '../../../types';

type Props = {
    /**
     * Custom class name to use for the form field.
     */
    className?: string;

    /**
     * Binding property for the dropdown's current value.
     */
    value?: TemplateType;

    /**
     * Determines whether the form field is disabled.
     */
    disabled?: boolean;

    /**
     * A callback which is invoked whenever the dropdown's selection changes.
     */
    onChange?: (value: TemplateType) => void;
}

/**
 * Renders an import template type dropdown form field.
 * 
 * @category Forms
 * @subcategory Fields
 * @component
 */
function TemplateTypeDropdown({ className, value, disabled, onChange }: Props): JSX.Element {
    const types: Array<TemplateType> = ['portfolios', 'positions', 'instruments', 'prices', 'transactions'];
    const [type, setType] = useState<TemplateType>(value ?? 'portfolios');

    const handleTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newType = e.target.value as TemplateType;
        setType(newType);
        onChange && onChange(newType);
    }

    return (
        <div className={`form-group ${className ?? ''}`}>
            <label htmlFor="template-type">Template:</label>
            <select
                className="form-control"
                disabled={disabled}
                id="template-type"
                onChange={handleTypeChange}
                value={type}
            >
                {types.map(t => <option key={t} value={t}>{t[0].toUpperCase() + t.substring(1)}</option>)}
            </select>
        </div>
    )
}

export default TemplateTypeDropdown;