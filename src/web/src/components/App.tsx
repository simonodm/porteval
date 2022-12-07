import React, { useEffect, useState } from 'react';
import Header from './Header';
import Sidebar from './Sidebar';
import Dashboard from './views/Dashboard';
import InstrumentView from './views/InstrumentView';
import InstrumentListView from './views/InstrumentListView';
import PortfolioListView from './views/PortfolioListView';
import PortfolioView from './views/PortfolioView';
import ChartListView from './views/ChartListView';
import ChartView from './views/ChartView';
import CurrenciesView from './views/CurrenciesView';
import SettingsView from './views/SettingsView';
import ImportExportView from './views/ImportExportView';

import {
    BrowserRouter as Router,
    Switch,
    Route,
    Redirect
} from 'react-router-dom';
import { toast, ToastContainer } from 'react-toastify';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { RTK_API_TAGS, TOAST_OPTIONS } from '../constants';
import { useDispatch } from 'react-redux';
import { portEvalApi } from '../redux/api/portEvalApi';

import './App.css';
import 'react-toastify/dist/ReactToastify.css';

/**
 * Renders the application.
 * 
 * @component
 */
function App(): JSX.Element {
    const dispatch = useDispatch(); // used to invalidate all API cache on refresh
    const [signalRConnection, setSignalRConnection] = useState<HubConnection | null>(null);

    // establish SignalR hub connection on first render
    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl('/api/notifications')
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        setSignalRConnection(connection);
    }, [])

    // init SignalR callbacks when connection changes
    useEffect(() => {
        if(signalRConnection) {
            signalRConnection.start()
                .then(_ => {
                    signalRConnection.on('ReceiveNotification', message => {
                        if(message.message !== undefined && message.message !== null) {
                            toast.info(message.message, TOAST_OPTIONS);
                        }
                        if(message.type === "newDataAvailable") {
                            dispatch(portEvalApi.util.invalidateTags(RTK_API_TAGS));
                        }
                    })
                })
                .catch(e => console.log(`SignalR connection failed: ${e}`));
        }
    }, [signalRConnection])

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

export default App;