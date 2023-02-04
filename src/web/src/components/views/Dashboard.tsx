import React, { useEffect, useState } from 'react';
import GridLayout, { Layout, WidthProvider } from 'react-grid-layout';
import DashboardChart from '../charts/DashboardChart';
import ModalWrapper from '../modals/ModalWrapper';
import DashboardChartPicker from '../charts/DashboardChartPicker';
import PageHeading from '../ui/PageHeading';

import { useGetDashboardLayoutQuery, useUpdateDashboardLayoutMutation } from '../../redux/api/dashboardApi';
import { useGetAllChartsQuery } from '../../redux/api/chartApi';

import 'react-grid-layout/css/styles.css';
import './Dashboard.css';

const ResponsiveGridLayout = WidthProvider(GridLayout);

/**
 * Renders the dashboard.
 * 
 * @category Views
 * @component
 */
function Dashboard(): JSX.Element {
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

            // doublecheck if the layout has indeed changed, as react-grid-layout sometimes triggers the change event
            // multiple times
            if(JSON.stringify(newDashboardLayout) !== JSON.stringify(dashboardLayout)) {
                setDashboardLayout(newDashboardLayout);
                updateLayout(newDashboardLayout);
            }            
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

    // Refresh layout if a new layout is received from query
    useEffect(() => {
        if(layoutQuery.data) {
            setDashboardLayout(layoutQuery.data);
        }
    }, [layoutQuery.data]);

    return (
        <>
            <PageHeading heading="Dashboard">
                <button
                    className="btn btn-success btn-sm float-right"
                    onClick={() => setIsEditable(!isEditable)}
                    role="button"
                >
                    Toggle dashboard edit
                </button>
                <button
                    className="btn btn-primary btn-sm float-right mr-1"
                    onClick={() => {
                        setModalIsOpen(true); setIsEditable(true) 
                    }}
                    role="button"
                >
                    Add charts
                </button>
            </PageHeading>
            <ResponsiveGridLayout
                className="layout"
                cols={6}
                droppingItem={{i: droppingItemId.toString(), w: 1, h: 1}}
                isBounded={true}
                isDraggable={isEditable}
                isDroppable={true}
                isResizable={isEditable}
                onLayoutChange={saveLayout}
                rowHeight={150}
            >
                {charts.data && dashboardLayout && charts.data.map(chart => {
                    const chartPosition = dashboardLayout.items.find(l => l.chartId === chart.id);

                    if(chartPosition) {
                        return (
                            <div className={isEditable ? 'editable-grid-item' : ''} data-grid={{
                                x: chartPosition.dashboardPositionX,
                                y: chartPosition.dashboardPositionY,
                                w: chartPosition.dashboardWidth,
                                h: chartPosition.dashboardHeight,
                                isBounded: true
                            }} key={chart.id}
                            >
                                <DashboardChart chart={chart} disabled={isEditable} />
                                { isEditable && 
                                    <button
                                        className="btn btn-danger grid-remove-button"
                                        onClick={() => onRemove(chart.id)}
                                    >
                                        X
                                    </button>
                                }
                            </div>
                        )
                    }
                })}
            </ResponsiveGridLayout>
            <ModalWrapper closeModal={() => setModalIsOpen(false)} heading="Add charts to dashboard"
                isOpen={modalIsOpen}
            >
                <DashboardChartPicker
                    charts={charts.data?.filter(c => !dashboardLayout?.items.find(item => item.chartId === c.id)) ?? []}
                    onDrag={handleDrag}
                />
            </ModalWrapper>
        </>
    )
}

export default Dashboard;