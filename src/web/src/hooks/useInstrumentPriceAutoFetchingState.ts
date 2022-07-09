import { DateTime } from 'luxon';
import { skipToken } from '@reduxjs/toolkit/dist/query';
import { useEffect, useState } from 'react';

import { useGetInstrumentPriceAtQuery } from '../redux/api/instrumentApi';

type SetInstrumentCallback = (instrumentId: number) => void;
type SetTimeCallback = (dt: DateTime) => void;
type SetPriceCallback = (price: number) => void;
type AutoUpdateReturnType =
    [number, SetInstrumentCallback, SetTimeCallback, SetPriceCallback]

export default function useInstrumentPriceAutoFetchingState(
    initialInstrumentId: number | undefined, initialTime: DateTime
): AutoUpdateReturnType {
    const [instrumentId, setInstrumentId] = useState(initialInstrumentId);
    const [time, setTime] = useState(initialTime);
    const [price, setPrice] = useState(0);
    const [isCustomPrice, setIsCustomPrice] = useState(false);

    const instrumentPrice =
        useGetInstrumentPriceAtQuery(instrumentId && !isCustomPrice ? { instrumentId, time: time.toISO() } : skipToken);

    const setCustomPrice = (price: number) => {
        setIsCustomPrice(true);
        setPrice(price);
    }

    useEffect(() => {
        if(instrumentPrice.data) {
            setPrice(instrumentPrice.data.price);
        }
    }, [instrumentPrice.data]);

    return [price, setInstrumentId, setTime, setCustomPrice];
}