import React, { useState } from 'react';
import { ChartLineDashType, ChartLineInstrument, ChartLine } from '../../types';

type Props = {
    line: ChartLine;
    onSave: (line: ChartLineInstrument) => void;
    closeModal: () => void;
}

export default function ChartLineConfiguratorModal({ line: lineProp, closeModal, onSave }: Props): JSX.Element {
    const dashTypes: Array<ChartLineDashType> = ['solid', 'dashed', 'dotted'];
    const [line, setLine] = useState(lineProp);

    const handleWidthChange = (width: number) => {
        setLine({
            ...line,
            width
        });
    }

    const handleColorChange = (color: string) => {
        setLine({
            ...line,
            color
        });
    }

    const handleDashChange = (dash: ChartLineDashType) => {
        setLine({
            ...line,
            dash
        });
    }

    const handleSubmit = () => {
        onSave(line as ChartLineInstrument);
        closeModal();
    }

    return (
        <form onSubmit={handleSubmit}>
            <div className="form-group">
                <label htmlFor="width">Width:</label>
                <input id="width" className="form-control" type="number" value={line.width} onChange={(e) => handleWidthChange(parseInt(e.target.value))} />
            </div>
            <div className="form-group">
                <label htmlFor="color">Color:</label>
                <input id="color" className="form-control" type="color" value={line.color} onChange={(e) => handleColorChange(e.target.value)} />
            </div>
            <div className="form-group">
                <label htmlFor="dash">Dash:</label>
                <select id="dash" className="form-control" onChange={(e) => handleDashChange(e.target.value as ChartLineDashType)}>
                    {dashTypes.map(dashType => <option selected={line.dash === dashType}>{dashType}</option>)}
                </select>
            </div>
            <button role="button" className="btn btn-primary">Save</button>
        </form>
    )
}