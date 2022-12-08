import React from 'react'
import { render } from '@testing-library/react'
import { Provider } from 'react-redux'
import { setupListeners } from '@reduxjs/toolkit/dist/query'
import setupStore from '../redux/store';
import { BrowserRouter } from 'react-router-dom';

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