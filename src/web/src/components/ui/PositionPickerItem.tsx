import React from 'react';

import { ChartLine, Position } from '../../types';

type Props = {
    position: Position;
    line?: ChartLine;
    onLineAdd?: () => void;
    onLineRemove?: () => void;
    onLineConfigure?: () => void;
}

export default function PositionPickerItem(
    { position, line, onLineAdd, onLineRemove, onLineConfigure }: Props
): JSX.Element {
    return (
        <div className="picker-item">
            <span className="picker-item-name">{position.instrument.name}</span>
            {
                line && <span className="picker-line-color-box" style={{ color: line.color }}>&#9632;</span>
            }
            <span className="picker-item-actions">
                {
                    line === undefined
                        ? 
                            <button
                                className="btn btn-primary btn-extra-sm"
                                onClick={onLineAdd}
                                role="button"
                            >
                                Add
                            </button>
                        :
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
                }
            </span>
        </div>
    )
}