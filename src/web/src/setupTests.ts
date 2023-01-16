import '@testing-library/jest-dom';
import ReactModal from 'react-modal';
import { portEvalApi } from './redux/api/portEvalApi';
import setupStore from './redux/store';
import { resetState } from './__tests__/mocks/handlers';
import server from './__tests__/mocks/server';

const store = setupStore({});

global.console = {
    ...console,
    error: jest.fn()
}

jest.setTimeout(10000);

beforeAll(() => {
    server.listen();
});
afterEach(() => {
    resetState();
    server.resetHandlers();
    store.dispatch(portEvalApi.util.resetApiState());
});
afterAll(() => {
    server.close();
});
