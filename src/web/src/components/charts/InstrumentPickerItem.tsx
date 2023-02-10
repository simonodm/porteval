import React from 'react';
import LinePreview from '../charts/LinePreview';

import Button from 'react-bootstrap/Button';

import { LINE_PREVIEW_LENGTH } from '../../constants';
import { ChartLine, Instrument } from '../../types';

type Props = {
    /**
     * Instrument to display.
     */
    instrument: Instrument;

    /**
     * Existing chart line for the instrument.
     */
    line?: ChartLine;

    /**
     * A callback which is invoked on instrument's addition to a chart.
     */
    onLineAdd?: () => void;

    /**
     * A callback which is invoked on instrument's removal from a chart.
     */
    onLineRemove?: () => void;

    /**
     * A callback which is invoked when the existing line configuration is to be displayed. 
     */
    onLineConfigure?: () => void;
}

/**
 * Renders an instrument list item to be added/removed from a chart.
 * 
 * @category Chart
 * @component
 */
function InstrumentPickerItem(
    { instrument, line, onLineAdd, onLineRemove, onLineConfigure }: Props
): JSX.Element {
    return (
        <div className="picker-item" aria-label={`${instrument.name} picker item`}>
            <span className="picker-item-name">{instrument.name}</span>
            {
                line && <LinePreview length={LINE_PREVIEW_LENGTH} line={line} />
            }
            <span className="picker-item-actions">
                {
                    line === undefined
                        ? 
                            <Button
                                variant="primary"
                                className="btn-xs"
                                onClick={onLineAdd}
                            >
                                Add
                            </Button>
                        : 
                            <>
                                <Button
                                    variant="danger"
                                    className="btn-xs"
                                    onClick={onLineRemove}
                                >
                                    Remove
                                </Button>
                                <Button
                                    variant="primary"
                                    className="btn-xs"
                                    onClick={onLineConfigure}
                                >
                                    Modify
                                </Button>
                            </>
                }
            </span>
        </div>
    )
}

export default InstrumentPickerItem;