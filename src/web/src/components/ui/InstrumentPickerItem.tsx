import React from 'react';
import { ChartLine, Instrument } from '../../types';

type Props = {
    instrument: Instrument;
    line?: ChartLine;
    onLineAdd?: () => void;
    onLineRemove?: () => void;
    onLineConfigure?: () => void;
}

export default function InstrumentPickerItem({ instrument, line, onLineAdd, onLineRemove, onLineConfigure }: Props): JSX.Element {
    return (
        <div className="picker-item">
            <span className="picker-item-name">{instrument.name}</span>
            {
                line && <span className="picker-line-color-box" style={{ color: line.color }}></span>
            }
            <span className="picker-item-actions">
                {
                    line === undefined
                        ? <button role="button" className="btn btn-primary btn-extra-sm" onClick={onLineAdd}>Add</button>
                        : <>
                            <button role="button" className="btn btn-danger btn-extra-sm" onClick={onLineRemove}>Remove</button>
                            <button role="button" className="btn btn-primary btn-extra-sm" onClick={onLineConfigure}>Modify</button>
                          </>
                }
            </span>
        </div>
    )
}