import { isRejectedWithValue } from '@reduxjs/toolkit';
import { Middleware } from 'redux';
import { toast } from 'react-toastify';
import * as constants from '../../constants';
import { isRequestErrorResponse, isValidationErrorResponse } from '../api/apiTypes';

const toastOptions = {
    position: toast.POSITION.BOTTOM_RIGHT,
    autoClose: 5000,
    hideProgressBar: true,
    closeOnClick: true,
    pauseOnHover: true,
    draggable: true,
    progress: undefined,
};

export const rtkQueryErrorDisplay: Middleware = () => (next) => (action) => {
    if(isRejectedWithValue(action)) {
        const data = action.payload?.data;
        if(isRequestErrorResponse(data)) {
            toast.error(data.errorMessage);
        }
        else if(isValidationErrorResponse(data)) {
            Object.entries(data.errors).forEach(
                ([,errors]) => {
                    errors.forEach(error => toast.error(error));
                }
            );
        }
        else {
            toast.error(constants.ERROR_STRING, toastOptions);
        }
    }

    return next(action);
}