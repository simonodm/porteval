import React, { useRef, useState , useLayoutEffect } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import PortEvalChart from '../charts/PortEvalChart';
import useGetRouteState from '../../hooks/useGetRouteState';
import ChartConfigurator from '../charts/ChartConfigurator';
import PortfolioPicker from '../charts/PortfolioPicker';
import InstrumentPicker from '../charts/InstrumentPicker';
import ModalWrapper from '../modals/ModalWrapper';
import ChartLineConfigurator from '../charts/ChartLineConfigurator';
import ChartLineConfigurationContext from '../../context/ChartLineConfigurationContext';
import PageHeading from '../ui/PageHeading';
import EditChartMetaForm from '../forms/EditChartMetaForm';
import * as constants from '../../constants';

import { skipToken } from '@reduxjs/toolkit/dist/query';
import { isSuccessfulResponse } from '../../redux/api/apiTypes';
import { useParams } from 'react-router';
import { ChartConfig, ChartLine, ChartLineConfigurationContextType,
    ChartLineInstrument, ChartLinePortfolio, ChartLinePosition, Instrument, Portfolio, Position } from '../../types';
import { toast } from 'react-toastify';
import { useCreateChartMutation, useGetChartQuery, useUpdateChartMutation } from '../../redux/api/chartApi';
import { checkIsLoaded, checkIsError } from '../../utils/queries';

type Params = {
    /**
     * ID of chart to render.
     */
    chartId: string;
}

/**
 * Renders a full-page view of a chart and its configuration based on query parameters or route state.
 * 
 * @category Views
 * @component 
 */
function ChartView(): JSX.Element {
    const [chartId, setChartId] = useState(useParams<Params>().chartId);
    const [chart, setChart] = useState(useGetRouteState<ChartConfig>('chart'));

    const chartFromState = useRef(chartId === undefined && chart !== undefined);
    const chartQuery = useGetChartQuery(!chartFromState.current ? parseInt(chartId) : skipToken);
    const [createChart] = useCreateChartMutation();
    const [updateChart] = useUpdateChartMutation();

    const [isChanged, setIsChanged] = useState(chartFromState.current);

    const isLoaded = checkIsLoaded(chartQuery);
    const isError = checkIsError(chartQuery);
    const [lineModalIsOpen, setLineModalIsOpen] = useState(false);
    const [editModalIsOpen, setEditModalIsOpen] = useState(false);
    const [modalLine, setModalLine] = useState<ChartLine | undefined>(undefined);

    useLayoutEffect(() => {
        if(chartQuery?.data) {
            setChart(chartQuery.data);
            setChartId(chartQuery.data.id.toString())
        }
    }, [chartQuery?.data])

    const addInstrumentLine = (instrument: Instrument) => {
        setModalLine({
            ...constants.DEFAULT_CHART_LINE,
            type: 'instrument',
            instrumentId: instrument.id,
            name: instrument.name
        } as ChartLine);

        setLineModalIsOpen(true);
    }

    const addPortfolioLine = (portfolio: Portfolio) => {
        setModalLine({
            ...constants.DEFAULT_CHART_LINE,
            type: 'portfolio',
            portfolioId: portfolio.id,
            name: portfolio.name
        } as ChartLine);

        setLineModalIsOpen(true);
    }

    const addPositionLine = (position: Position) => {
        setModalLine({
            ...constants.DEFAULT_CHART_LINE,
            type: 'position',
            positionId: position.id,
            name: position.instrument.name
        } as ChartLine);

        setLineModalIsOpen(true);
    }

    const addPortfolioPositionLines = (positions: Array<Position>) => {
        const newLines = positions.map(position => ({
            ...constants.DEFAULT_CHART_LINE,
            type: 'position',
            positionId: position.id,
            name: position.instrument.name
        } as ChartLine));

        chart && setChart({
            ...chart,
            lines: [
            ...chart.lines,
            ...newLines
            ]
        });
        setIsChanged(true);
    }

    const configureLine = (line: ChartLine) => {
        setModalLine(line);
        setLineModalIsOpen(true);
    }

    const removeLine = (line: ChartLine) => {
        chart && setChart({
            ...chart,
            lines: chart.lines.filter(existingLine => existingLine !== line)
        });
        setIsChanged(true);
    }

    const handleChartConfigurationUpdate = (newChart: ChartConfig) => {
        setChart(newChart);
        setIsChanged(true);
    }

    const handleLineSave = (line: ChartLine) => {
        chart && setChart({
            ...chart,
            lines: [...chart.lines.filter(existingLine =>
                existingLine.type !== line.type
                || (existingLine.type === 'instrument' &&
                    existingLine.instrumentId !== (line as ChartLineInstrument).instrumentId)
                || (existingLine.type === 'portfolio' &&
                    existingLine.portfolioId !== (line as ChartLinePortfolio).portfolioId)
                || (existingLine.type === 'position' &&
                    existingLine.positionId !== (line as ChartLinePosition).positionId)),
                line]
            });
        setIsChanged(true);
        setLineModalIsOpen(false);
    }

    const handleEditSave = (updatedChart: ChartConfig) => {
        setChart(updatedChart);
        handleChartSave(updatedChart);
    }

    const handleChartSave = (updatedChart: ChartConfig) => {
        const onSuccess = () => {
            toast.success('Saved');
            setIsChanged(false);
            setEditModalIsOpen(false);
        }

        if(!chartId) {
            createChart(updatedChart).then((res) => {
                if(isSuccessfulResponse(res)) {
                    setChartId(res.data.id.toString());
                    onSuccess();
                }
            });
        } else {
            updateChart({
                ...updatedChart,
                id: parseInt(chartId)
            }).then(onSuccess);
        }
    }

    const context: ChartLineConfigurationContextType = {
        chart,
        addInstrumentLine,
        addPortfolioLine,
        addPositionLine,
        addPortfolioPositionLines,
        configureLine,
        removeLine
    }

    return (
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            <PageHeading heading={'Chart: ' + chart?.name ?? ''}>
                <button
                    className="btn btn-primary btn-sm float-right"
                    disabled={!isChanged}
                    onClick={() => chart && handleChartSave(chart)}
                    role="button"
                >
                    Save
                </button>
                <button
                    className="btn btn-primary btn-sm float-right mr-1"
                    onClick={() => setEditModalIsOpen(true)}
                    role="button"
                >
                    Rename
                </button>
            </PageHeading>
            <ChartLineConfigurationContext.Provider value={context}>
                { chart &&
                    <div className="container-fluid row flex-grow-1">
                        <div className="col-xs-12 col-md-8 container-fluid d-flex flex-column">
                            <div>
                                <ChartConfigurator onChange={handleChartConfigurationUpdate} />
                            </div>
                            <PortEvalChart chart={chart} />
                        </div>
                        
                        <div className="col-xs-12 col-md-4 container-fluid w-100">
                            <PortfolioPicker />
                            <InstrumentPicker />
                        </div>
                        <ModalWrapper
                            closeModal={() => setLineModalIsOpen(false)}
                            heading={`Configure line ${modalLine?.name ?? ''}`} 
                            isOpen={lineModalIsOpen}
                        >
                            {
                                modalLine &&
                                    <ChartLineConfigurator
                                        line={modalLine}
                                        onSave={(line) => handleLineSave(line)}
                                    />
                            }
                        </ModalWrapper>
                        <ModalWrapper
                            closeModal={() => setEditModalIsOpen(false)}
                            heading="Edit chart info"
                            isOpen={editModalIsOpen}
                        >
                            <EditChartMetaForm chart={chart} onSave={handleEditSave} />
                        </ModalWrapper>
                    </div>
                }
            </ChartLineConfigurationContext.Provider>
        </LoadingWrapper>
    )
}

export default ChartView;