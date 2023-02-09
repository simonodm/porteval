import React from 'react';
import InstrumentCurrentPriceText from './InstrumentCurrentPriceText';
import { Instrument } from '../../types';

type Props = {
    /**
     * Instrument to display information of.
     */
    instrument: Instrument;
}

/**
 * Renders the instrument's key information in a table format.
 * 
 * @category UI
 * @component
 */
function InstrumentInformation({ instrument }: Props): JSX.Element {
    return (
        <table className="entity-data w-100">
            <tbody>
                <tr>
                    <td>Name:</td>
                    <td>{ instrument.name }</td>
                </tr>
                <tr>
                    <td>Symbol:</td>
                    <td>{ instrument.symbol }</td>
                </tr> 
                <tr>
                    <td>Exchange:</td>
                    <td>{ instrument.exchange }</td>
                </tr>
                <tr>
                    <td>Currency:</td>
                    <td>{ instrument.currencyCode }</td>
                </tr>
                <tr>
                    <td>Current price:</td>
                    <td>
                        <InstrumentCurrentPriceText instrument={instrument} />
                    </td>
                </tr>
                <tr>
                    <td>Note:</td>
                    <td>{ instrument.note }</td>
                </tr>
            </tbody>
        </table>
    );
}

export default InstrumentInformation;