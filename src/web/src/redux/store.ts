import rtkQueryErrorDisplay from './middleware/rtkQueryErrorDisplay';
import { configureStore } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query';
import { portEvalApi } from './api/portEvalApi';
import { ToolkitStore } from '@reduxjs/toolkit/dist/configureStore';

/**
 * Configures PortEval's Redux store with optional initial state.
 * 
 * @category Redux
 * @param preloadedState Initial state
 * @returns A Redux store representing PortEval's application state.
 */
const setupStore = (preloadedState: Record<string, unknown>): ToolkitStore => {
    const store = configureStore({
        reducer: {
            [portEvalApi.reducerPath]: portEvalApi.reducer,
        },
        preloadedState,
        middleware: getDefaultMiddleware =>
            getDefaultMiddleware({
                immutableCheck: false,
                serializableCheck: false,
            })
                .concat(portEvalApi.middleware)
                .concat(rtkQueryErrorDisplay),
    });

    setupListeners(store.dispatch);

    return store;
}

export default setupStore;