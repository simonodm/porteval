import React, { useState } from 'react';

import { ChartLineDashType, ChartLine } from '../../types';
import { API_MAX_CHART_LINE_WIDTH } from '../../constants';

type Props = {
    /**
     * Chart line to configure.
     */
    line: ChartLine;

    /**
     * A callback which is invoked when the new chart line configuration is saved.
     */
    onSave: (line: ChartLine) => void;
}

/**
 * Renders a chart line configuration component, enabling modification of chart line's width, dash type, and color.
 * 
 * @category Chart
 * @component
 */
function ChartLineConfigurator({ line: lineProp, onSave }: Props): JSX.Element {
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
        onSave(line as ChartLine);
    }

    return (
        <form onSubmit={handleSubmit}>
            <div className="form-group">
                <label htmlFor="width">Width:</label>
                <div id="width">
                    <input type="radio" id="width-thin" name="width" checked={line.width === 1} value={1} onChange={(e) => handleWidthChange(parseInt(e.target.value))} />
                    <label htmlFor='width-thin' className="mr-2">thin</label>
                    <input type="radio" id="width-medium" name="width" checked={line.width === 3} value={3} onChange={(e) => handleWidthChange(parseInt(e.target.value))} />
                    <label htmlFor='width-medium' className="mr-2">medium</label>
                    <input type="radio" id="width-thick" name="width" checked={line.width === 5} value={5} onChange={(e) => handleWidthChange(parseInt(e.target.value))} />
                    <label htmlFor='width-thick' className="mr-2">thick</label>
                </div>
            </div>
            <div className="form-group">
                <label htmlFor="color">Color:</label>
                <input className="form-control" id="color" onChange={(e) => handleColorChange(e.target.value)}
                    type="color" value={line.color}
                />
            </div>
            <div className="form-group">
                <label htmlFor="dash">Dash:</label>
                <div id="dash">
                    {
                        dashTypes.map(dashType =>
                            <>
                                <input 
                                    key={dashType}
                                    id={`dash-${dashType}`}
                                    type="radio"
                                    name="dash"
                                    checked={line.dash === dashType} 
                                    value={dashType}
                                    onChange={(e) => handleDashChange(e.target.value as ChartLineDashType)}
                                />
                                <label htmlFor={`dash-${dashType}`} className="mr-2">{dashType}</label>
                            </>
                        )
                    }
                </div>
                
            </div>
            <button className="btn btn-primary" role="button">Save</button>
        </form>
    )
}

export default ChartLineConfigurator;