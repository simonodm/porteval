import '@testing-library/jest-dom';
import ReactModal from 'react-modal';
import { portEvalApi } from './redux/api/portEvalApi';
import setupStore from './redux/store';
import server from './__tests__/mocks/server';

const store = setupStore({});

beforeAll(() => {
    server.listen();
});
afterEach(() => {
    server.resetHandlers();
    store.dispatch(portEvalApi.util.resetApiState());
});
afterAll(() => {
    server.close();
});
