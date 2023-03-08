import React from 'react';
import LinePreview from '../charts/LinePreview';

import Button from 'react-bootstrap/Button';

import { LINE_PREVIEW_LENGTH } from '../../constants';
import { ChartLine, Portfolio } from '../../types';

type Props = {
    /**
     * Portfolio to display.
     */
    portfolio: Portfolio;

    /**
     * Existing chart line for the portfolio.
     */
    line?: ChartLine;

    /**
     * A callback which is invoked on portfolio's addition to a chart.
     */
    onLineAdd?: () => void;

    /**
     * A callback which is invoked on portfolio's removal from a chart.
     */
    onLineRemove?: () => void;

    /**
     * A callback which is invoked when the existing line configuration is to be displayed. 
     */
    onLineConfigure?: () => void;

    /**
     * A callback which is invoked when portfolio's position line configuration is to be displayed.
     */
    onConfigurePositions: () => void;
}

/**
 * Renders a portfolio list item to be added/removed from a chart.
 * 
 * @category Chart
 * @component 
 */
function PortfolioPickerItem(
    { portfolio, line, onLineAdd, onLineRemove, onLineConfigure, onConfigurePositions }: Props
): JSX.Element {
    return (
        <div className="picker-item" aria-label={`${portfolio.name} picker item`}>
            <span className="picker-item-name">{portfolio.name}</span>
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
                <Button
                    variant="primary"
                    className="btn-xs"
                    onClick={onConfigurePositions}
                >
                    Positions
                </Button>
            </span>
        </div>
    )
}

export default PortfolioPickerItem;