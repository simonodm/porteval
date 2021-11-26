import React from 'react';
import { ChartLine, Portfolio } from '../../types';

type Props = {
    portfolio: Portfolio;
    line?: ChartLine;
    onLineAdd: () => void;
    onLineRemove: () => void;
    onLineConfigure: () => void;
    onConfigurePositions: () => void;
}

export default function PortfolioPickerItem({ portfolio, line, onLineAdd, onLineRemove, onLineConfigure, onConfigurePositions }: Props): JSX.Element {
    return (
        <div className="picker-item">
            <span className="picker-item-name">{portfolio.name}</span>
            <span className="picker-item-actions">
                {
                    line
                        ? <>
                            <button role="button" className="btn btn-danger btn-extra-sm" onClick={onLineRemove}>Remove</button>
                            <button role="button" className="btn btn-primary btn-extra-sm" onClick={onLineConfigure}>Modify</button>
                          </>
                        : <button role="button" className="btn btn-primary btn-extra-sm" onClick={onLineAdd}>Add</button>
                }
                <button role="button" className="btn btn-primary btn-extra-sm" onClick={onConfigurePositions}>Positions</button>
            </span>
        </div>
    )
}