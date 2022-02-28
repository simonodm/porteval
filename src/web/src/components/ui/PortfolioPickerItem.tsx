import React from 'react';

import { ChartLine, Portfolio } from '../../types';
import LinePreview from '../charts/LinePreview';
import { LINE_PREVIEW_LENGTH } from '../../constants';

type Props = {
    portfolio: Portfolio;
    line?: ChartLine;
    onLineAdd: () => void;
    onLineRemove: () => void;
    onLineConfigure: () => void;
    onConfigurePositions: () => void;
}

export default function PortfolioPickerItem(
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