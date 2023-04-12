import { isRejectedWithValue } from '@reduxjs/toolkit';
import { Middleware } from 'redux';
import { toast } from 'react-toastify';
import { isRequestErrorResponse, isValidationErrorResponse } from '../api/apiTypes';

import * as constants from '../../constants';

/**
 * An RTK middleware which is invoked whenever an RTK query fails.
 * 
 * @category Redux
 * @returns {Middleware} Middleware to be added to the RTK middleware chain.
 */
const rtkQueryErrorDisplay: Middleware = () => (next) => (action) => {
    if(isRejectedWithValue(action)) {
        const data = action.payload?.data;
        if(isRequestErrorResponse(data)) {
            toast.error(data.errorMessage);
        } else if(isValidationErrorResponse(data)) {
            Object.entries(data.errors).forEach(
                ([,errors]) => {
                    errors.forEach(error => toast.error(error));
                }
            );
        } else {
            toast.error(data, constants.TOAST_OPTIONS);
        }
    }

    return next(action);
}

export default rtkQueryErrorDisplay;