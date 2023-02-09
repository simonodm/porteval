import React from 'react';
import LoadingBubbles from './LoadingBubbles';
import useUserSettings from '../../hooks/useUserSettings';

import { Instrument } from '../../types';
import { getPriceString } from '../../utils/string';

type Props = {
    instrument: Instrument;
}

/**
 * Renders the instrument's current price or a loading animation if the instrument is still searching for prices.
 * 
 * @category UI
 * @component
 */
export default function InstrumentCurrentPriceText({ instrument }: Props): JSX.Element {
    const [userSettings] = useUserSettings();

    return instrument.trackingStatus === 'searchingForPrices' || instrument.trackingStatus === 'created'
            ? <LoadingBubbles />
            : <>{getPriceString(instrument.currentPrice, instrument.currencyCode, userSettings)}</>
}