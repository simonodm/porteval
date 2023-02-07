import React, { useState } from 'react';
import LinePreview from './LinePreview';

import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

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

    const handleSubmit = (e: React.FormEvent) => {
        onSave(line as ChartLine);
        e.preventDefault();
    }

    return (
        <Form onSubmit={handleSubmit} aria-label="Edit chart line form">
            <Form.Group className="mb-3" controlId="form-line-width">
                <Form.Label>Width:</Form.Label>
                <Form.Range
                    value={line.width}
                    min={1}
                    max={API_MAX_CHART_LINE_WIDTH}
                    onChange={(e) => handleWidthChange(parseInt(e.target.value))}
                />
            </Form.Group>
            <Form.Group className="mb-3" controlId="form-line-color">
                <Form.Label>Color:</Form.Label>
                <Form.Control className="w-100" onChange={(e) => handleColorChange(e.target.value)}
                    type="color" value={line.color} data-testid="line-color-picker"
                />
            </Form.Group>
            <Form.Group className="mb-3">
                {
                    dashTypes.map(dashType =>
                        <Form.Check 
                            inline
                            key={dashType}
                            type="radio"
                            name="form-line-dash"
                            checked={line.dash === dashType} 
                            value={dashType}
                            onChange={(e) => handleDashChange(e.target.value as ChartLineDashType)}
                            label={dashType}
                            id={`dash-${dashType}`}
                        />
                    )
                }
            </Form.Group>
            <div className="mb-3">
                <LinePreview line={line} length={80} />
            </div>
            <Button variant="primary" type="submit">Save</Button>
        </Form>
    )
}

export default ChartLineConfigurator;