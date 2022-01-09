import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useGetCurrencyQuery } from '../../redux/api/currencyApi';
import { useDeleteInstrumentMutation, useGetInstrumentCurrentPriceQuery } from '../../redux/api/instrumentApi';
import { Instrument } from '../../types';
import { getPriceString } from '../utils/string';
import { generateDefaultInstrumentChart } from '../utils/chart';
import { NavLink } from 'react-router-dom';
import * as constants from '../../constants';
import ModalWrapper from '../modals/ModalWrapper';
import EditInstrumentModal from '../modals/EditInstrumentModal';

type Props = {
    instrument: Instrument;
}

export default function InstrumentRow({ instrument }: Props): JSX.Element {
    const currency = useGetCurrencyQuery(instrument.currencyCode);
    const currentPrice = useGetInstrumentCurrentPriceQuery(instrument.id);
    const [ deleteInstrument, { isLoading } ] = useDeleteInstrumentMutation();
    const [ isModalOpen, setIsModalOpen ] = useState(false);

    const isLoaded = checkIsLoaded(currency, currentPrice);
    const isError = checkIsError(currency, currentPrice);
    const isBeingDeleted = !isLoading;

    return (
        <><tr>
            <td><a href={'/instruments/' + instrument.id}>{instrument.name}</a></td>
            <td>{instrument.symbol}</td>
            <td>{instrument.currencyCode}</td>
            <td>{constants.INSTRUMENT_TYPE_TO_STRING[instrument.type]}</td>
            <td>
                <LoadingWrapper isLoaded={isLoaded} isError={isError}>
                    <>{getPriceString(currentPrice.data?.price, currency.data?.symbol)}</>
                </LoadingWrapper>
            </td>
            <td>{instrument.note}</td>
            <td>
                <LoadingWrapper isLoaded={isBeingDeleted}>
                    <NavLink className="btn btn-primary btn-extra-sm mr-1" to={{pathname: '/charts/view', state: { chart: generateDefaultInstrumentChart(instrument) }}}>Chart</NavLink>
                    <button role="button" className="btn btn-primary btn-extra-sm mr-1" onClick={() => setIsModalOpen(true)}>Edit</button>
                    <button role="button" className="btn btn-danger btn-extra-sm" onClick={() => deleteInstrument(instrument.id)}>Remove</button>
                </LoadingWrapper>
            </td>
        </tr>
        <ModalWrapper isOpen={isModalOpen} closeModal={() => setIsModalOpen(false)}>
            <EditInstrumentModal instrument={instrument} closeModal={() => setIsModalOpen(false)} />
        </ModalWrapper></>
    );
}