import React, { useContext } from 'react';
import InstrumentPickerItem from './InstrumentPickerItem';
import LoadingWrapper from '../ui/LoadingWrapper';
import ChartLineConfigurationContext from '../../context/ChartLineConfigurationContext';

import { useGetAllInstrumentsQuery } from '../../redux/api/instrumentApi';
import { checkIsLoaded, checkIsError } from '../../utils/queries';

/**
 * Loads and renders a list of instruments which can be added to the chart.
 * See {@link ChartLineConfigurationContext}.
 * 
 * @category Chart
 * @component
 */
function InstrumentPicker(): JSX.Element {
    const instruments = useGetAllInstrumentsQuery();
    const context = useContext(ChartLineConfigurationContext);

    const isLoaded = checkIsLoaded(instruments);
    const isError = checkIsError(instruments);

    return (
        <div className="chart-picker" aria-label="Instrument picker">
            <h6>Instruments</h6>
            <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                {instruments.data?.map(instrument => {
                    const line = context.chart?.lines.find(
                        line => line.type === 'instrument' && line.instrumentId === instrument.id
                    );
                    return (
                        <InstrumentPickerItem
                            instrument={instrument}
                            key={instrument.id}
                            line={line}
                            onLineAdd={() => context.addInstrumentLine(instrument)}
                            onLineConfigure={line ? () => context.configureLine(line) : undefined}
                            onLineRemove={() => line ? context.removeLine(line) : undefined}
                        />
                    )
                })}
            </LoadingWrapper>
        </div>
    )
}

export default InstrumentPicker;