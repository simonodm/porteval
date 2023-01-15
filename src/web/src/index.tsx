import React, { StrictMode } from 'react';
import { render } from 'react-dom';

import './index.css';
import { Provider } from 'react-redux';

import App from './components/App';
import reportWebVitals from './reportWebVitals';
import setupStore from './redux/store';
import ReactModal from 'react-modal';

const root = document.getElementById('root');
const store = setupStore({});

ReactModal.setAppElement('#app');

render(
    <StrictMode>
        <Provider store={store}>
            <App />
        </Provider>
    </StrictMode>,
  root
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
