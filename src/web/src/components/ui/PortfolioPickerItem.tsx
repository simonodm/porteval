import React from 'react';
import LinePreview from '../charts/LinePreview';
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
        <div className="picker-item">
            <span className="picker-item-name">{portfolio.name}</span>
            {
                line && <LinePreview length={LINE_PREVIEW_LENGTH} line={line} />
            }
            <span className="picker-item-actions">
                {
                    line
                        ? 
                            <>
                                <button
                                    className="btn btn-danger btn-extra-sm"
                                    onClick={onLineRemove}
                                    role="button"
                                >
                                    Remove
                                </button>
                                <button
                                    className="btn btn-primary btn-extra-sm"
                                    onClick={onLineConfigure}
                                    role="button"
                                >
                                    Modify
                                </button>
                            </>
                        : 
                            <button
                                className="btn btn-primary btn-extra-sm"
                                onClick={onLineAdd}
                                role="button"
                            >
                                Add
                            </button>
                }
                <button
                    className="btn btn-primary btn-extra-sm"
                    onClick={onConfigurePositions}
                    role="button"
                >
                    Positions
                </button>
            </span>
        </div>
    )
}

export default PortfolioPickerItem;