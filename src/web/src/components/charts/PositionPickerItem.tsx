import React from 'react';
import LinePreview from '../charts/LinePreview';

import Button from 'react-bootstrap/Button';

import { LINE_PREVIEW_LENGTH } from '../../constants';
import { ChartLine, Position } from '../../types';

type Props = {
    /**
     * Position to display.
     */
    position: Position;

    /**
     * Existing chart line for the position.
     */
    line?: ChartLine;

    /**
     * A callback which is invoked on position's addition to a chart.
     */
     onLineAdd?: () => void;

     /**
      * A callback which is invoked on position's removal from a chart.
      */
     onLineRemove?: () => void;
 
     /**
      * A callback which is invoked when the existing line configuration is to be displayed. 
      */
     onLineConfigure?: () => void;
}

/**
 * Renders a position list item to be added/removed from a chart.
 * 
 * @category Chart
 * @component 
 */
function PositionPickerItem(
    { position, line, onLineAdd, onLineRemove, onLineConfigure }: Props
): JSX.Element {
    return (
        <div className="picker-item" aria-label={`${position.instrument.name} position picker item`}>
            <span className="picker-item-name">{position.instrument.name}</span>
            {
                line && <LinePreview length={LINE_PREVIEW_LENGTH} line={line} />
            }
            <span className="picker-item-actions">
                {
                    line
                        ?
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
                        :
                            <Button
                                variant="primary"
                                className="btn-xs"
                                onClick={onLineAdd}
                            >
                                Add
                            </Button>
                }
            </span>
        </div>
    )
}

export default PositionPickerItem;