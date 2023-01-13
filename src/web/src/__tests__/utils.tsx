import React from 'react'
import { render } from '@testing-library/react'
import { Provider } from 'react-redux'
import setupStore from '../redux/store';
import { BrowserRouter } from 'react-router-dom';
import { parseISO, format } from 'date-fns';
import { DEFAULT_DATE_FORMAT, DEFAULT_TIME_FORMAT } from '../constants';

export function renderWithProviders(
    ui: React.ReactElement,
    {
      preloadedState = {},
      store = setupStore(preloadedState),
      ...renderOptions
    } = {}
) {
    function Wrapper({ children }: { children: React.ReactElement }) {
      return <Provider store={store}><BrowserRouter>{children}</BrowserRouter></Provider>
    }

    return { store, ...render(ui, { wrapper: Wrapper, ...renderOptions }) }
}

export function reformatDateTime(dateTime: string): string {
    const parsedDt = parseISO(dateTime);
    return format(parsedDt, DEFAULT_DATE_FORMAT + ' ' + DEFAULT_TIME_FORMAT);
}