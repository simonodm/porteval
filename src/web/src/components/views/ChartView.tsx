import React, { useRef, useState, useEffect, useLayoutEffect } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import PortEvalChart from '../charts/PortEvalChart';
import useGetRouteState from '../../hooks/useGetRouteState';
import ChartConfigurator from '../charts/ChartConfigurator';
import PortfolioPicker from '../charts/PortfolioPicker';
import InstrumentPicker from '../charts/InstrumentPicker';
import ModalWrapper from '../modals/ModalWrapper';
import ChartLineConfiguratorForm from '../forms/ChartLineConfiguratorForm';
import ChartLineConfigurationContext from '../../context/ChartLineConfigurationContext';
import PageHeading from '../ui/PageHeading';
import EditChartMetaForm from '../forms/EditChartMetaForm';
import * as constants from '../../constants';

import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';

import { skipToken } from '@reduxjs/toolkit/dist/query';
import { isSuccessfulResponse } from '../../redux/api/apiTypes';
import { useParams } from 'react-router';
import { ChartConfig, ChartLine, ChartLineConfigurationContextType,
    ChartLineInstrument, ChartLinePortfolio, ChartLinePosition, Instrument, Portfolio, Position } from '../../types';
import { useCreateChartMutation, useGetChartQuery, useUpdateChartMutation } from '../../redux/api/chartApi';
import { checkIsLoaded, checkIsError } from '../../utils/queries';

import './ChartView.css';

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

    // track automated color fill for chart lines when they are first configured
    // color fill progresses through a constant set of default colors (see constants.CHART_LINE_COLOR_CODE_PROGRESSION),
    // and resets to the first one when running through them
    const [lastUsedColorCodeIndex, setLastUsedColorCodeIndex] = useState(-1);

    const chartFromState = useRef(chartId === undefined && chart !== undefined);
    const chartQuery = useGetChartQuery(!chartFromState.current ? parseInt(chartId as string) : skipToken);
    const [createChart] = useCreateChartMutation();
    const [updateChart] = useUpdateChartMutation();

    const [isChanged, setIsChanged] = useState(false);
    const [autosave, setAutosave] = useState(false);

    const [lineModalIsOpen, setLineModalIsOpen] = useState(false);
    const [editModalIsOpen, setEditModalIsOpen] = useState(false);
    const [modalLine, setModalLine] = useState<ChartLine | undefined>(undefined);

    const isLoaded = checkIsLoaded(chartQuery);
    const isError = checkIsError(chartQuery);

    useLayoutEffect(() => {
        if(chartQuery?.data) {
            setChart(chartQuery.data);
            setChartId(chartQuery.data.id.toString())
        }
    }, [chartQuery?.data]);

    // the following two hooks enable autosave functionality
    // essentially, the first hook switches `autosave` state to true every five seconds
    // the second hook then reacts to these changes by actually saving the chart and switching `autosave` back to false
    // this separation is important, because if the chart saving logic was inside the setInterval callback,
    // then it would capture the value of `isChanged` during the first render and never reevaluate it again
    useEffect(() => {
        const autoSaveInterval = setInterval(() => {
            setAutosave(true);
        }, 5000);

        return () => {
            clearInterval(autoSaveInterval);
            setAutosave(false);
        }
    }, []);

    useEffect(() => {
        if(chart && autosave && isChanged) {
            handleChartSave(chart);
            setAutosave(false);
        }        
    }, [autosave, isChanged])

    const addInstrumentLine = (instrument: Instrument) => {
        const colorIndex = (lastUsedColorCodeIndex + 1) % constants.CHART_LINE_COLOR_CODE_PROGRESSION.length;

        setModalLine({
            ...constants.DEFAULT_CHART_LINE,
            type: 'instrument',
            instrumentId: instrument.id,
            name: instrument.name,
            color: constants.CHART_LINE_COLOR_CODE_PROGRESSION[colorIndex]
        } as ChartLine);

        setLastUsedColorCodeIndex(colorIndex);
        setLineModalIsOpen(true);
    }

    const addPortfolioLine = (portfolio: Portfolio) => {
        const colorIndex = (lastUsedColorCodeIndex + 1) % constants.CHART_LINE_COLOR_CODE_PROGRESSION.length;

        setModalLine({
            ...constants.DEFAULT_CHART_LINE,
            type: 'portfolio',
            portfolioId: portfolio.id,
            name: portfolio.name,
            color: constants.CHART_LINE_COLOR_CODE_PROGRESSION[colorIndex]
        } as ChartLine);

        setLastUsedColorCodeIndex(colorIndex);
        setLineModalIsOpen(true);
    }

    const addPositionLine = (position: Position) => {
        const colorIndex = (lastUsedColorCodeIndex + 1) % constants.CHART_LINE_COLOR_CODE_PROGRESSION.length;

        setModalLine({
            ...constants.DEFAULT_CHART_LINE,
            type: 'position',
            positionId: position.id,
            name: position.instrument.name,
            color: constants.CHART_LINE_COLOR_CODE_PROGRESSION[colorIndex]
        } as ChartLine);

        setLastUsedColorCodeIndex(colorIndex);
        setLineModalIsOpen(true);
    }

    const addPortfolioPositionLines = (positions: Array<Position>) => {
        if(!chart) {
            return;
        }

        const newLines: Array<ChartLine> = positions.map((position, idx) => {
            const colorIndex = (lastUsedColorCodeIndex + 1 + idx) % constants.CHART_LINE_COLOR_CODE_PROGRESSION.length;

            return {
                ...constants.DEFAULT_CHART_LINE,
                type: 'position',
                positionId: position.id,
                name: position.instrument.name,
                color: constants.CHART_LINE_COLOR_CODE_PROGRESSION[colorIndex]
            }
        });

        setLastUsedColorCodeIndex(
            (lastUsedColorCodeIndex + positions.length) % constants.CHART_LINE_COLOR_CODE_PROGRESSION.length
        );

        const updatedChart = {
            ...chart,
            lines: [
                ...chart.lines,
                ...newLines
            ]
        };

        setChart(updatedChart);
        setIsChanged(true);
    }

    const configureLine = (line: ChartLine) => {
        setModalLine(line);
        setLineModalIsOpen(true);
    }

    const removeLine = (line: ChartLine) => {
        if(!chart) {
            return;
        }

        const updatedChart = {
            ...chart,
            lines: chart.lines.filter(existingLine => existingLine !== line)
        };
        
        setChart(updatedChart);
        setIsChanged(true);
    }

    const handleLineSave = (line: ChartLine) => {
        if(!chart) {
            return;
        }

        const updatedChart = {
            ...chart,
            lines: [...chart.lines.filter(existingLine =>
                existingLine.type !== line.type
                || (existingLine.type === 'instrument' &&
                    existingLine.instrumentId !== (line as ChartLineInstrument).instrumentId)
                || (existingLine.type === 'portfolio' &&
                    existingLine.portfolioId !== (line as ChartLinePortfolio).portfolioId)
                || (existingLine.type === 'position' &&
                    existingLine.positionId !== (line as ChartLinePosition).positionId)),
                line
            ]
        };

        setChart(updatedChart);
        setIsChanged(true);
        setLineModalIsOpen(false);
    }

    const handleChartEditSave = (updatedChart: ChartConfig) => {
        setEditModalIsOpen(false);
        setChart(updatedChart);
        setIsChanged(true);
    }

    const handleChartSave = (updatedChart: ChartConfig) => {
        if(!chartId) {
            createChart(updatedChart).then((res) => {
                if(isSuccessfulResponse(res)) {
                    setChartId(res.data.id.toString());
                }
            });
        } else {
            updateChart({
                ...updatedChart,
                id: parseInt(chartId)
            });
        }

        setIsChanged(false);
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
        <>
            <PageHeading heading={'Chart: ' + chart?.name ?? ''}>
                <Button
                    variant="primary"
                    size="sm"
                    className="float-right"
                    disabled={!isChanged && chartId !== undefined}
                    onClick={() => chart && handleChartSave(chart)}
                >
                    {isChanged || chartId === undefined ? 'Save' : 'Saved'}
                </Button>
                <Button
                    variant="primary"
                    size="sm"
                    className="float-right"
                    onClick={() => setEditModalIsOpen(true)}
                >
                    Rename
                </Button>
            </PageHeading>
            <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                <ChartLineConfigurationContext.Provider value={context}>
                    <Container fluid className="chart-view-container d-flex flex-column flex-grow-1 g-0">
                        <Row className="mb-5">
                            <Col>
                                <ChartConfigurator onChange={(c) => {
                                        setChart(c);
                                        setIsChanged(true);
                                    }}
                                />
                            </Col>
                        </Row>
                        <Row className="chart-editor flex-grow-1 gy-5">
                            <Col xs={12} lg={8} className="min-vh-50">
                                {chart && <PortEvalChart chart={chart} /> }
                            </Col>
                            <Col xs={12} lg={4} className="d-flex">
                                <Container fluid className="chart-item-pickers">
                                    <PortfolioPicker />
                                    <InstrumentPicker />
                                </Container>  
                            </Col>                          
                        </Row>
                    </Container>
                    <ModalWrapper
                        closeModal={() => setLineModalIsOpen(false)}
                        heading={`Configure line ${modalLine?.name ?? ''}`} 
                        isOpen={lineModalIsOpen}
                    >
                        {
                            modalLine &&
                                <ChartLineConfiguratorForm
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
                        {chart && <EditChartMetaForm chart={chart} onSave={handleChartEditSave} /> }
                    </ModalWrapper>
                </ChartLineConfigurationContext.Provider>
            </LoadingWrapper>
        </>
    )
}

export default ChartView;