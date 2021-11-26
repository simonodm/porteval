import React, { useState } from 'react';

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
            <div className="form-group">
                <label htmlFor="chart-name">Chart name:</label>
                <input type="text" id="chart-name" className="form-control" onChange={(e) => setName(e.target.value)} value={name} />
            </div>
            <button role="button" className="btn btn-primary">Save</button>
        </form>
    )
}