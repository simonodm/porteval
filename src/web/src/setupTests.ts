import '@testing-library/jest-dom';
import { configure } from '@testing-library/react';
import { portEvalApi } from './redux/api/portEvalApi';
import setupStore from './redux/store';
import { resetState } from './__tests__/mocks/handlers';
import server from './__tests__/mocks/server';

const store = setupStore({});

global.console = {
    ...console,
    error: jest.fn()
}

configure({ asyncUtilTimeout: 3000 });
jest.setTimeout(10000);

beforeAll(() => {
    server.listen();
});
beforeEach(() => {
    // window.matchMedia(query) mock, as JSDom does not support it
    // see https://jestjs.io/docs/manual-mocks#mocking-methods-which-are-not-implemented-in-jsdom
    Object.defineProperty(window, 'matchMedia', {
        writable: true,
        value: jest.fn().mockImplementation(query => ({
            matches: false,
            media: query,
            onchange: null,
            addListener: jest.fn(),
            removeListener: jest.fn(),
            addEventListener: jest.fn(),
            removeEventListener: jest.fn(),
            dispatchEvent: jest.fn(),
        })),
    });
});
afterEach(() => {
    resetState();
    server.resetHandlers();
    store.dispatch(portEvalApi.util.resetApiState());
});
afterAll(() => {
    server.close();
});
