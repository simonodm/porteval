import { DateTime } from 'luxon';
import { skipToken } from '@reduxjs/toolkit/dist/query';
import { useEffect, useState } from 'react';

import { useGetInstrumentPriceAtQuery } from '../redux/api/instrumentApi';

type SetInstrumentCallback = (instrumentId: number | undefined) => void;
type SetTimeCallback = (dt: DateTime) => void;
type SetPriceCallback = (price: number | undefined) => void;
type SetAutoUpdateEnabledCallback = (enabled: boolean) => void;
type AutoUpdateReturnType =
    [number | undefined, SetInstrumentCallback, SetTimeCallback, SetPriceCallback, SetAutoUpdateEnabledCallback]

export default function useInstrumentPriceAutoFetchingState(
    initialInstrumentId: number | undefined, initialTime: DateTime
): AutoUpdateReturnType {
    const [instrumentId, setInstrumentId] = useState(initialInstrumentId);
    const [time, setTime] = useState(initialTime);
    const [price, setPrice] = useState<number | undefined>(undefined);
    const [autoUpdateEnabled, setAutoUpdateEnabled] = useState(true);

    const instrumentPrice =
        useGetInstrumentPriceAtQuery(
            instrumentId && autoUpdateEnabled ? { instrumentId, time: time.toISO() } : skipToken
        );

    useEffect(() => {
        if(instrumentPrice.data && autoUpdateEnabled) {
            setPrice(instrumentPrice.data.price);
        }
    }, [instrumentPrice]);

    return [price, setInstrumentId, setTime, setPrice, setAutoUpdateEnabled];
}