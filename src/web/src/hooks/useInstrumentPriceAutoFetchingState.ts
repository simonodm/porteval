import { skipToken } from '@reduxjs/toolkit/dist/query';
import { useEffect, useState } from 'react';
import { useGetInstrumentPriceAtQuery } from '../redux/api/instrumentApi';

/**
 * A callback to change instrument ID.
 * @category Hooks
 * @subcategory Types
 * @ignore
 */
type SetInstrumentCallback = (instrumentId: number | undefined) => void;

/**
 * A callback to change price fetch time.
 * @category Hooks
 * @subcategory Types
 */
type SetTimeCallback = (dt: Date) => void;

/**
 * A callback to change price.
 * @category Hooks
 * @subcategory Types
 */
type SetPriceCallback = (price: number | undefined) => void;

/**
 * A callback to toggle price auto-refetch.
 * @category Hooks
 * @subcategory Types
 */
type SetAutoUpdateEnabledCallback = (enabled: boolean) => void;

/**
 * A set of controls to manage {@link useInstrumentPriceAutoFetchingState} behavior.
 * @category Hooks
 * @subcategory Types
 */
type AutoUpdateReturnType =
    [number | undefined, SetInstrumentCallback, SetTimeCallback, SetPriceCallback, SetAutoUpdateEnabledCallback]

/**
 * A hook to store instrument price at the specified time with possible price auto-refresh on time or instrument change.
 * 
 * @category Hooks
 * @param initialInstrumentId ID of the initial instrument to store price of.
 * @param initialTime Time to fetch price at.
 * @param initialValue Initial price to use.
 * If this parameter is set, then the first auto-fetch will happen only after any of the other parameters change.
 * @returns {AutoUpdateReturnType} A collection of controls to change instrument, time, price, or toggle auto-update.
 */
function useInstrumentPriceAutoFetchingState(
    initialInstrumentId: number | undefined, initialTime: Date, initialValue: number | undefined = undefined
): AutoUpdateReturnType {
    const [instrumentId, setInstrumentId] = useState(initialInstrumentId);
    const [time, setTime] = useState(initialTime);
    const [price, setPrice] = useState<number | undefined>(initialValue);
    const [autoUpdateEnabled, setAutoUpdateEnabled] = useState(true);

    const [newPropsReceived, setNewPropsReceived] = useState(false);

    const instrumentPrice =
        useGetInstrumentPriceAtQuery(
            instrumentId && autoUpdateEnabled && (initialValue === undefined || newPropsReceived)
                ? { instrumentId, time: time.toISOString() }
                : skipToken
        );

    useEffect(() => {
        setNewPropsReceived(true);
    }, [initialInstrumentId, initialTime]);

    // update state if new price is received from query
    useEffect(() => {
        if(instrumentPrice.data && autoUpdateEnabled) {
            setPrice(instrumentPrice.data.price);
        }
    }, [instrumentPrice]);

    return [price, setInstrumentId, setTime, setPrice, setAutoUpdateEnabled];
}

export default useInstrumentPriceAutoFetchingState;