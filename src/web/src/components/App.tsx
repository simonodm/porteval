import React, { useEffect, useState } from 'react';
import ReactModal from 'react-modal';
import Layout from './Layout';
import Container from 'react-bootstrap/Container';

import { BrowserRouter as Router } from 'react-router-dom';
import { toast, ToastContainer } from 'react-toastify';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { TOAST_OPTIONS } from '../constants';
import { useDispatch } from 'react-redux';
import { invalidateFinancialData } from '../utils/queries';

import './App.css';
import 'react-toastify/dist/ReactToastify.css';
import 'bootstrap/dist/css/bootstrap.min.css';


/**
 * Renders the application.
 * 
 * @component
 */
function App(): JSX.Element {
    const dispatch = useDispatch();
    const [signalRConnection, setSignalRConnection] = useState<HubConnection | null>(null);

    const startSignalRConnection = (connection: HubConnection) => {
        if(connection) {
            connection.start()
            .then(() => {
                connection.on('ReceiveNotification', message => {
                    if(message.message !== undefined && message.message !== null) {
                        toast.info(message.message, TOAST_OPTIONS);
                    }
                    if(message.type === 'newDataAvailable') {
                        dispatch(invalidateFinancialData());
                    }
                })
            })
            .catch(e => console.log(`SignalR connection failed: ${e}`));
        }
    }

    // establish SignalR hub connection on first render
    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl('/api/notifications')
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        setSignalRConnection(connection);
    }, [])

    useEffect(() => {
        ReactModal.setAppElement('#app');
    }, [])

    // init SignalR callbacks when connection changes
    useEffect(() => {
        if(signalRConnection) {
            startSignalRConnection(signalRConnection);
            signalRConnection.onclose(() => startSignalRConnection(signalRConnection));
        }
    }, [signalRConnection])

    return (
        <Container fluid className="p-0" id="app">
            <Router>
                <Layout />
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
        </Container>
    );
}

export default App;