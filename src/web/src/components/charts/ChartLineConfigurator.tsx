import React, { useState } from 'react';

import { ChartLineDashType, ChartLineInstrument, ChartLine } from '../../types';
import { API_MAX_CHART_LINE_WIDTH } from '../../constants';

type Props = {
    line: ChartLine;
    onSave: (line: ChartLineInstrument) => void;
}

export default function ChartLineConfigurator({ line: lineProp, onSave }: Props): JSX.Element {
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
    }

    return (
        <form onSubmit={handleSubmit}>
            <div className="form-group">
                <label htmlFor="width">Width:</label>
                <input className="form-control" id="width" max={API_MAX_CHART_LINE_WIDTH}
                    onChange={(e) => handleWidthChange(parseInt(e.target.value))} type="number" value={line.width}
                />
            </div>
            <div className="form-group">
                <label htmlFor="color">Color:</label>
                <input className="form-control" id="color" onChange={(e) => handleColorChange(e.target.value)}
                    type="color" value={line.color}
                />
            </div>
            <div className="form-group">
                <label htmlFor="dash">Dash:</label>
                <select
                    className="form-control"
                    id="dash"
                    onChange={(e) => handleDashChange(e.target.value as ChartLineDashType)}
                >
                    {dashTypes.map(dashType =>
                        <option key={dashType} selected={line.dash === dashType}>{dashType}</option>)
                    }
                </select>
            </div>
            <button className="btn btn-primary" role="button">Save</button>
        </form>
    )
}