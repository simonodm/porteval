import React, { useEffect, useState } from 'react';
import GridLayout, { Layout, WidthProvider } from 'react-grid-layout';
import DashboardChart from '../charts/DashboardChart';
import 'react-grid-layout/css/styles.css';
import './Dashboard.css';
import { useGetDashboardLayoutQuery, useUpdateDashboardLayoutMutation } from '../../redux/api/dashboardApi';
import { useGetAllChartsQuery } from '../../redux/api/chartApi';
import ModalWrapper from '../modals/ModalWrapper';
import DashboardChartPickerModal from '../modals/DashboardChartPickerModal';
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

    const onLayoutChange = (layouts: Layout[]) => {
        const newDashboardPositions = layouts.map(layout => ({
            chartId: parseInt(layout.i),
            dashboardPositionX: layout.x,
            dashboardPositionY: layout.y,
            dashboardWidth: layout.w,
            dashboardHeight: layout.h
        }));

        updateLayout({
            items: newDashboardPositions
        });
    }

    const onDrop = (layout: Layout[], layoutItem: Layout) => {
        if(dashboardLayout) {
            setDashboardLayout({
                items: [
                    ...dashboardLayout.items,
                    {
                        chartId: droppingItemId,
                        dashboardPositionX: layoutItem.x,
                        dashboardPositionY: layoutItem.y,
                        dashboardWidth: layoutItem.w,
                        dashboardHeight: layoutItem.h
                    }
                ]
            })
        }
        
    }

    const onRemove = (chartId: number) => {
        if(dashboardLayout) {
            setDashboardLayout({
                items: [
                    ...dashboardLayout.items.filter(item => item.chartId !== chartId)
                ]
            });
            updateLayout(dashboardLayout);
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
                onLayoutChange={onLayoutChange}
                onDrop={onDrop}>
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
                <DashboardChartPickerModal
                    charts={charts.data?.filter(c => !dashboardLayout?.items.find(item => item.chartId === c.id)) ?? []}
                    onDrag={(id) => setDroppingItemId(id)}
                    closeModal={() => setModalIsOpen(false)}
                />
            </ModalWrapper>
        </>
    )
}