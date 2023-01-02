import React from 'react';
import useUserSettings from '../../hooks/useUserSettings';
import { Instrument } from '../../types';
import { getPriceString } from '../../utils/string';
import LoadingBubbles from './LoadingBubbles';

type Props = {
    instrument: Instrument;
}

export default function InstrumentCurrentPriceText({ instrument }: Props): JSX.Element {
    const [userSettings] = useUserSettings();

    return instrument.trackingStatus === 'searchingForPrices' || instrument.trackingStatus === 'created'
            ? <LoadingBubbles />
            : <>{getPriceString(instrument.currentPrice, instrument.currencyCode, userSettings)}</>
}