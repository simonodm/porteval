import { DateTime } from 'luxon';
import { skipToken } from '@reduxjs/toolkit/dist/query';
import React, { useEffect, useState } from 'react';
import { useGetInstrumentPriceAtQuery } from '../redux/api/instrumentApi';

type SetInstrumentCallback = (instrumentId: number) => void;
type SetTimeCallback = (dt: DateTime) => void;
type AutoUpdateReturnType = [number, SetInstrumentCallback, SetTimeCallback, React.Dispatch<React.SetStateAction<number>>]

export default function useInstrumentPriceAutoFetchingState(initialInstrumentId: number | undefined, initialTime: DateTime): AutoUpdateReturnType {
    const [instrumentId, setInstrumentId] = useState(initialInstrumentId);
    const [time, setTime] = useState(initialTime);
    const [price, setPrice] = useState(0);

    const instrumentPrice = useGetInstrumentPriceAtQuery(instrumentId ? { instrumentId, time: time.toISO() } : skipToken);

    useEffect(() => {
        if(instrumentPrice.data) {
            setPrice(instrumentPrice.data.price);
        }
    }, [instrumentPrice.data]);

    return [price, setInstrumentId, setTime, setPrice];
}