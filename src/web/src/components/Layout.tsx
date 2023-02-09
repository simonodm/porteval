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

import { Switch, Route, Redirect } from 'react-router-dom';

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
                    <Switch>
                        <Route path="/instruments/:instrumentId">
                            <InstrumentView />
                        </Route>
                        <Route path="/instruments">
                            <InstrumentListView />
                        </Route>
                        <Route path="/portfolios/:portfolioId">
                            <PortfolioView />
                        </Route>
                        <Route path="/portfolios">
                            <PortfolioListView />
                        </Route>
                        <Route path="/charts/view/:chartId">
                            <ChartView />
                        </Route>
                        <Route path="/charts/view">
                            <ChartView />
                        </Route>
                        <Route path="/charts">
                            <ChartListView />
                        </Route>
                        <Route path="/currencies">
                            <CurrenciesView />
                        </Route>
                        <Route path="/import">
                            <ImportExportView />
                        </Route>
                        <Route path="/settings">
                            <SettingsView />
                        </Route>
                        <Route path="/dashboard">
                            <Dashboard />
                        </Route>
                        <Route path="/">
                            <Redirect to="/dashboard" />
                        </Route>
                    </Switch>
                </Container>
            </Container>
        </>
    )
}

export default Layout;