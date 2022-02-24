import React, { useEffect, useState } from 'react';
import GridLayout, { Layout, WidthProvider } from 'react-grid-layout';
import DashboardChart from '../charts/DashboardChart';
import 'react-grid-layout/css/styles.css';
import './Dashboard.css';
import { useGetDashboardLayoutQuery, useUpdateDashboardLayoutMutation } from '../../redux/api/dashboardApi';
import { useGetAllChartsQuery } from '../../redux/api/chartApi';
import ModalWrapper from '../modals/ModalWrapper';
import DashboardChartPicker from '../charts/DashboardChartPicker';
import PageHeading from '../ui/PageHeading';

const ResponsiveGridLayout = WidthProvider(GridLayout);

export default function Dashboard(): JSX.Element {
    const layoutQuery = useGetDashboardLayoutQuery();
    const [dashboardLayout, setDashboardLayout] = useState(layoutQuery.data);
    const charts = useGetAllChartsQuery();

    const [updateLayout] = useUpdateDashboardLayoutMutation();

    const [isEditable, setIsEditable] = useState(false);
    const [modalIsOpen, setModalIsOpen] = useState(false);
    const [droppingItemId, setDroppingItemId] = useState(0);

    const handleDrag = (id: number) => {
        setDroppingItemId(id);
        setModalIsOpen(false);
    }

    const saveLayout = (layout: Layout[]) => {
        if(dashboardLayout !== undefined) {
            const newDashboardLayout = {
                items: layout.map(item => ({
                    chartId: parseInt(item.i),
                    dashboardPositionX: item.x,
                    dashboardPositionY: item.y,
                    dashboardWidth: item.w,
                    dashboardHeight: item.h
                }))
            }
            setDashboardLayout(newDashboardLayout);
            updateLayout(newDashboardLayout);
        }
    }

    const onRemove = (chartId: number) => {
        if(dashboardLayout) {
            const newDashboardLayout = {
                items: [
                    ...dashboardLayout.items.filter(item => item.chartId !== chartId)
                ]
            }
            setDashboardLayout(newDashboardLayout);
            updateLayout(newDashboardLayout);
        }
    }

    useEffect(() => {
        if(layoutQuery.data) {
            setDashboardLayout(layoutQuery.data);
        }
    }, [layoutQuery.data]);

    return (
        <>
            <PageHeading heading="Dashboard">
                <button role="button" className="btn btn-success btn-sm float-right" onClick={() => setIsEditable(!isEditable)}>Toggle dashboard edit</button>
                <button role="button" className="btn btn-primary btn-sm float-right mr-1" onClick={() => { setModalIsOpen(true); setIsEditable(true) }}>Add charts</button>
            </PageHeading>
            <ResponsiveGridLayout
                className="layout"
                cols={6}
                isBounded={true}
                droppingItem={{i: droppingItemId.toString(), w: 1, h: 1}}
                isDraggable={isEditable}
                isResizable={isEditable}
                isDroppable={true}
                onDragStop={saveLayout}
                onResizeStop={saveLayout}
                onDrop={saveLayout}>
                {charts.data && dashboardLayout && charts.data.map(chart => {
                    const chartPosition = dashboardLayout.items.find(l => l.chartId === chart.id);

                    if(chartPosition) {
                        return (
                            <div key={chart.id} className={isEditable ? 'editable-grid-item' : ''} data-grid={{
                                x: chartPosition.dashboardPositionX,
                                y: chartPosition.dashboardPositionY,
                                w: chartPosition.dashboardWidth,
                                h: chartPosition.dashboardHeight,
                                isBounded: true
                            }}>
                                <DashboardChart chart={chart} />
                                { isEditable && 
                                    <button role="button" className="btn btn-danger grid-remove-button" onClick={() => onRemove(chart.id)}>X</button>
                                }
                            </div>
                        )
                    }
                })}
            </ResponsiveGridLayout>
            <ModalWrapper isOpen={modalIsOpen} closeModal={() => setModalIsOpen(false)}>
                <DashboardChartPicker
                    charts={charts.data?.filter(c => !dashboardLayout?.items.find(item => item.chartId === c.id)) ?? []}
                    onDrag={handleDrag}
                />
            </ModalWrapper>
        </>
    )
}