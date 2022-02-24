import { configureStore } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query';

import { portEvalApi } from './api/portEvalApi';
import { rtkQueryErrorDisplay } from './middleware/rtkQueryErrorDisplay';

export const store = configureStore({
    reducer: {
        [portEvalApi.reducerPath]: portEvalApi.reducer
    },
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware()
            .concat(portEvalApi.middleware)
            .concat(rtkQueryErrorDisplay)
});

setupListeners(store.dispatch);