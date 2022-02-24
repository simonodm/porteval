import React from 'react';

import { ToastContainer } from 'react-toastify';

import {
    BrowserRouter as Router,
    Switch,
    Route,
    Redirect
} from 'react-router-dom';

import Header from './Header';
import Sidebar from './Sidebar';
import Dashboard from './views/Dashboard';
import InstrumentView from './views/InstrumentView';
import InstrumentListView from './views/InstrumentListView';
import PortfolioListView from './views/PortfolioListView';
import SettingsView from './views/SettingsView';
import PortfolioView from './views/PortfolioView';

import 'react-toastify/dist/ReactToastify.css';


import './App.css';
import ChartListView from './views/ChartListView';
import ChartView from './views/ChartView';

export default function App(): JSX.Element {
    return (
        <div id="app">
            <Router>
                <Header />
                <div id="main">
                    <Sidebar />
                    <div id="content">
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
                    </div>
                </div>
            </Router>
            <ToastContainer
                autoClose={5000}
                closeOnClick
                draggable
                hideProgressBar
                newestOnTop={false}
                pauseOnFocusLoss
                pauseOnHover
                position="bottom-right"
                rtl={false}
            />
        </div>
    );
}
