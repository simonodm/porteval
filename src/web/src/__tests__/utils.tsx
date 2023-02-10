import React from 'react'
import setupStore from '../redux/store';

import { createMemoryRouter, RouterProvider } from 'react-router-dom';
import { parseISO, format } from 'date-fns';
import { DEFAULT_DATE_FORMAT, DEFAULT_TIME_FORMAT } from '../constants';
import { render } from '@testing-library/react';
import { Provider } from 'react-redux';

export function createTestMemoryRouter(route: string, pathname: string, element: React.ReactElement) {
    const routes = [
      {
          path: route,
          element: element
      }
    ];
    const router = createMemoryRouter(routes, {
        initialEntries: [pathname]
    });

    return router;
}

export function renderWithProviders(
    ui: React.ReactElement,
    {
      preloadedState = {},
      store = setupStore(preloadedState),
      router = createTestMemoryRouter('/', '/', ui),
      ...renderOptions
    } = {}
) {
    function Wrapper() {
      return <Provider store={store}><RouterProvider router={router} /></Provider>
    }

    return { store, router, ...render(ui, { wrapper: Wrapper, ...renderOptions }) }
}

export function reformatDateTime(dateTime: string): string {
    const parsedDt = parseISO(dateTime);
    return format(parsedDt, DEFAULT_DATE_FORMAT + ' ' + DEFAULT_TIME_FORMAT);
}