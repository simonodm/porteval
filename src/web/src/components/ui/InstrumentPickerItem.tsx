import React from 'react';

import { ChartLine, Instrument } from '../../types';
import LinePreview from '../charts/LinePreview';
import { LINE_PREVIEW_LENGTH } from '../../constants';

type Props = {
    instrument: Instrument;
    line?: ChartLine;
    onLineAdd?: () => void;
    onLineRemove?: () => void;
    onLineConfigure?: () => void;
}

export default function InstrumentPickerItem(
    { instrument, line, onLineAdd, onLineRemove, onLineConfigure }: Props
): JSX.Element {
    return (
        <div className="picker-item">
            <span className="picker-item-name">{instrument.name}</span>
            {
                line && <LinePreview length={LINE_PREVIEW_LENGTH} line={line} />
            }
            <span className="picker-item-actions">
                {
                    line === undefined
                        ? 
                            <button
                                className="btn btn-primary btn-extra-sm"
                                onClick={onLineAdd} role="button"
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