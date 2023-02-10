import React, { useState } from 'react';
import Header from './Header';
import ChartListView from './views/ChartListView';
import ChartView from './views/ChartView';
import CurrenciesView from './views/CurrenciesView';
import Dashboard from './views/Dashboard';
import ImportExportView from './views/ImportExportView';
import InstrumentListView from './views/InstrumentListView';
import InstrumentView from './views/InstrumentView';
import PortfolioListView from './views/PortfolioListView';
import PortfolioView from './views/PortfolioView';
import SettingsView from './views/SettingsView';
import OffcanvasSidebar from './OffcanvasSidebar';
import Container from 'react-bootstrap/Container';

import { Routes, Route, Navigate } from 'react-router-dom';

/**
 * Renders the application layout with `react-router` routes.
 * 
 * @component
 */
function Layout(): JSX.Element {
    const [sidebarExpanded, setSidebarExpanded] = useState(false);

    const toggleSidebar = () => {
        setSidebarExpanded(!sidebarExpanded);
    }

    return (
        <>
            <Header enableResponsiveSidebar onSidebarToggle={toggleSidebar}/>
            <Container fluid className="p-0" id="main">
                <OffcanvasSidebar show={sidebarExpanded} onClose={toggleSidebar} />
                <Container fluid id="content">
                    <Routes>
                        <Route path="/instruments" element={<InstrumentListView />} />
                        <Route path="/instruments/:instrumentId" element={<InstrumentView />} />
                        <Route path="/portfolios" element={<PortfolioListView />} />
                        <Route path="/portfolios/:portfolioId" element={<PortfolioView />} />
                        <Route path="/charts" element={<ChartListView />} />
                        <Route path="/charts/view" element={<ChartView />} />
                        <Route path="/charts/view/:chartId" element={<ChartView />} />
                        <Route path="/currencies" element={<CurrenciesView />} />
                        <Route path="/import" element={<ImportExportView />} />
                        <Route path="/settings" element={<SettingsView />} />
                        <Route path="/dashboard" element={<Dashboard />} />
                        <Route path="/" element={<Navigate to="/dashboard" replace/>} />
                    </Routes>
                </Container>
            </Container>
        </>
    )
}

export default Layout;