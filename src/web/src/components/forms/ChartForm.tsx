import React, { useState } from 'react';
import TextInput from './fields/TextInput';

type Props = {
    defaultName: string;
    onSubmit: (name: string) => void;
}

export default function ChartForm({ defaultName, onSubmit }: Props): JSX.Element {
    const [name, setName] = useState(defaultName);

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        onSubmit(name);
        e.preventDefault();
    }

    return (
        <form onSubmit={handleSubmit}>
            <TextInput label='Chart name' defaultValue={defaultName} onChange={(val) => setName(val)} />
            <button role="button" className="btn btn-primary">Save</button>
        </form>
    )
}