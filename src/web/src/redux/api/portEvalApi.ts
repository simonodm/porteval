import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { RTK_API_TAGS } from '../../constants';

/**
 * PortEval's base API definition.
 * @category API
 */
export const portEvalApi = createApi({
    baseQuery: fetchBaseQuery({ baseUrl: '/api' }),
    endpoints: () => ({}),
    tagTypes: RTK_API_TAGS
});