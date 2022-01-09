import React, { useState } from 'react';
import { useCreateChartMutation, useGetChartQuery, useUpdateChartMutation } from '../../redux/api/chartApi';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import LoadingWrapper from '../ui/LoadingWrapper';
import { skipToken } from '@reduxjs/toolkit/dist/query';
import PortEvalChart from '../charts/PortEvalChart';
import useGetRouteState from '../../hooks/useGetRouteState';
import { ChartConfig, ChartLine, ChartLineConfigurationContextType, ChartLineInstrument, ChartLinePortfolio, ChartLinePosition } from '../../types';
import { useParams } from 'react-router';
import ChartConfigurator from '../charts/ChartConfigurator';
import PortfolioPicker from '../charts/PortfolioPicker';
import InstrumentPicker from '../charts/InstrumentPicker';
import ModalWrapper from '../modals/ModalWrapper';
import ChartLineConfiguratorModal from '../modals/ChartLineConfiguratorModal';
import * as constants from '../../constants';
import ChartLineConfigurationContext from '../../context/ChartLineConfigurationContext';
import { useLayoutEffect } from 'react';
import { toast } from 'react-toastify';
import EditChartMetaModal from '../modals/EditChartMetaModal';
import PageHeading from '../ui/PageHeading';

type Params = {
    chartId: string;
}

export default function ChartView(): JSX.Element {
    const chartId = useParams<Params>().chartId;
    const [chart, setChart] = useState(useGetRouteState<ChartConfig>('chart'));

    const chartFromState = chartId === undefined && chart !== undefined;
    const chartQuery = useGetChartQuery(!chartFromState ? parseInt(chartId) : skipToken);
    const [createChart] = useCreateChartMutation();
    const [updateChart] = useUpdateChartMutation();

    const [isChanged, setIsChanged] = useState(chartFromState);

    const isLoaded = checkIsLoaded(chartQuery);
    const isError = checkIsError(chartQuery);
    const [lineModalIsOpen, setLineModalIsOpen] = useState(false);
    const [editModalIsOpen, setEditModalIsOpen] = useState(false);
    const [modalLine, setModalLine] = useState<ChartLine | undefined>(undefined);

    useLayoutEffect(() => {
        if(chartQuery?.data) {
            setChart(chartQuery.data);
        }
    }, [chartQuery?.data])

    const addInstrumentLine = (instrumentId: number) => {
        setModalLine({
            ...constants.DEFAULT_CHART_LINE,
            type: 'instrument',
            instrumentId
        } as ChartLine);

        setLineModalIsOpen(true);
    }

    const addPortfolioLine = (portfolioId: number) => {
        setModalLine({
            ...constants.DEFAULT_CHART_LINE,
            type: 'portfolio',
            portfolioId
        } as ChartLine);

        setLineModalIsOpen(true);
    }

    const addPositionLine = (portfolioId: number, positionId: number) => {
        setModalLine({
            ...constants.DEFAULT_CHART_LINE,
            type: 'position',
            portfolioId,
            positionId
        } as ChartLine);

        setLineModalIsOpen(true);
    }

    const addPortfolioPositionLines = (portfolioId: number, positionIds: Array<number>) => {
        const newLines = positionIds.map(id => ({
            ...constants.DEFAULT_CHART_LINE,
            type: 'position',
            portfolioId,
            positionId: id
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
                || (existingLine.type === 'instrument' && existingLine.instrumentId !== (line as ChartLineInstrument).instrumentId)
                || (existingLine.type === 'portfolio' && existingLine.portfolioId !== (line as ChartLinePortfolio).portfolioId)
                || (existingLine.type === 'position' && existingLine.positionId !== (line as ChartLinePosition).positionId)),
                line]
            });
        setIsChanged(true);
    }

    const handleEditSave = (updatedChart: ChartConfig) => {
        setChart(updatedChart);
        handleChartSave();
    }

    const handleChartSave = () => {
        const onSuccess = () => {
            toast.success('Saved');
            setIsChanged(false);
        }

        if(chartFromState) {
            createChart(chart).then(onSuccess);
        }
        else {
            chart && updateChart({
                ...chart,
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
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <PageHeading heading={'Chart: ' + chart?.name ?? ''}>
                <button disabled={!isChanged} role="button" className="btn btn-primary btn-sm float-right" onClick={handleChartSave}>Save</button>
                <button role="button" className="btn btn-primary btn-sm float-right mr-1" onClick={() => setEditModalIsOpen(true)}>Rename</button>
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
                        <ModalWrapper isOpen={lineModalIsOpen} closeModal={() => setLineModalIsOpen(false)}>
                            {
                                modalLine &&
                                    <ChartLineConfiguratorModal
                                        line={modalLine}
                                        onSave={(line) => handleLineSave(line)}
                                        closeModal={() => setLineModalIsOpen(false)}
                                    />
                            }
                        </ModalWrapper>
                        <ModalWrapper isOpen={editModalIsOpen} closeModal={() => setEditModalIsOpen(false)}>
                            <EditChartMetaModal chart={chart} onSave={handleEditSave} closeModal={() => setEditModalIsOpen(false)} />
                        </ModalWrapper>
                    </div>
                }
            </ChartLineConfigurationContext.Provider>
        </LoadingWrapper>
    )
}