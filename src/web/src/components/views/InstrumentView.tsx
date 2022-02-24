import React, { useState } from 'react';
import LoadingWrapper from '../ui/LoadingWrapper';

import { useGetCurrencyQuery } from '../../redux/api/currencyApi';
import { useDeleteInstrumentPriceMutation, useGetInstrumentByIdQuery, useGetInstrumentCurrentPriceQuery, useGetInstrumentPricePageQuery, usePrefetch } from '../../redux/api/instrumentApi';
import { useParams } from 'react-router-dom';
import { checkIsLoaded, checkIsError } from '../utils/queries';

import { skipToken } from '@reduxjs/toolkit/dist/query';
import { getDateTimeLocaleString, getPerformanceString, getPriceString } from '../utils/string';
import * as constants from '../../constants';
import PortEvalChart from '../charts/PortEvalChart';
import ModalWrapper from '../modals/ModalWrapper';
import { generateDefaultInstrumentChart } from '../utils/chart';
import PageHeading from '../ui/PageHeading';
import PageSelector from '../ui/PageSelector';

import './InstrumentView.css';
import { DateTime } from 'luxon';
import CreateInstrumentPriceForm from '../forms/CreateInstrumentPriceForm';

type Params = {
    instrumentId?: string;
}

export default function InstrumentView(): JSX.Element {
    const params = useParams<Params>();
    const instrumentId = params.instrumentId ? parseInt(params.instrumentId) : 0;

    const [page, setPage] = useState(1);
    const [pageLimit] = useState(100);

    const prefetchPrices = usePrefetch('getInstrumentPricePage');

    const instrument = useGetInstrumentByIdQuery(instrumentId);
    const currentPrice = useGetInstrumentCurrentPriceQuery(instrumentId);
    const prices = useGetInstrumentPricePageQuery({ instrumentId, page, limit: pageLimit }, { pollingInterval: constants.REFRESH_INTERVAL });
    const currency = useGetCurrencyQuery(instrument.data?.currencyCode ?? skipToken)
    const [deletePrice, mutationStatus] = useDeleteInstrumentPriceMutation()

    const instrumentLoaded = checkIsLoaded(instrument, currentPrice);
    const instrumentError = checkIsError(instrument, currentPrice);

    const pricesLoaded = checkIsLoaded(prices, mutationStatus);
    const pricesError = checkIsError(prices, mutationStatus);

    const [modalIsOpen, setModalIsOpen] = useState(false);

    const chart = instrument.data ? generateDefaultInstrumentChart(instrument.data) : undefined;

    return (
        <LoadingWrapper isLoaded={instrumentLoaded} isError={instrumentError}>
            <PageHeading heading={instrument.data?.name ?? 'Instrument'}>
                { instrument.data?.isTracked  && instrument.data.lastPriceUpdate &&
                    <span className="float-right last-updated">
                        Prices updated: {DateTime.fromISO(instrument.data?.lastPriceUpdate).toLocaleString(DateTime.DATETIME_MED)}
                    </span>
                }
            </PageHeading>
            <div className="row mb-5">
                <div className="col-xs-12 col-md-6">
                    <h5>Data</h5>
                        <table className="entity-data w-100">
                            <tbody>
                                <tr>
                                    <td>Name:</td>
                                    <td>{ instrument.data?.name }</td>
                                </tr>
                                <tr>
                                    <td>Symbol:</td>
                                    <td>{ instrument.data?.symbol }</td>
                                </tr> 
                                <tr>
                                    <td>Exchange:</td>
                                    <td>{ instrument.data?.exchange }</td>
                                </tr>
                                <tr>
                                    <td>Currency:</td>
                                    <td>{ instrument.data?.currencyCode }</td>
                                </tr>
                                <tr>
                                    <td>Current price:</td>
                                    <td>{ getPriceString(currentPrice.data?.price, currency.data?.symbol) }</td>
                                </tr>
                                <tr>
                                    <td>Note:</td>
                                    <td>{ instrument.data?.note }</td>
                                </tr>
                            </tbody>
                        </table>
                </div>
                <div className="col-xs-12 col-md-6">
                    { chart && <PortEvalChart chart={chart} /> }
                </div>
            </div>
            <div className="action-buttons">
                <button role="button" className="btn btn-success btn-sm float-right" onClick={() => setModalIsOpen(true)}>Add price</button>
            </div>
            <div className="row">
                <div className="col-xs-12 container-fluid">
                    <div className="content-heading">
                        <h5>Price history</h5>
                    </div>
                    <div className="float-right">
                    <PageSelector
                            page={page}
                            totalPages={prices.data ? prices.data.totalCount / pageLimit : 1}
                            onPageChange={(p) => setPage(p)}
                            prefetch={(p) => prefetchPrices({ instrumentId, page: p, limit: pageLimit })}
                        />
                    </div>
                    <table className="entity-list w-100">
                        <thead>
                            <tr>
                                <th>Date</th>
                                <th>Price</th>
                                <th>Change</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <LoadingWrapper isLoaded={pricesLoaded} isError={pricesError}>
                            <tbody>
                                {prices.data?.data.map((price, index, array) => (
                                    <tr>
                                        <td>{getDateTimeLocaleString(price.time)}</td>
                                        <td>{getPriceString(price.price, currency.data?.symbol)}</td>
                                        <td>{index < array.length - 1 ?
                                                getPerformanceString(price.price / array[index + 1].price - 1) :
                                                getPerformanceString(0)}</td>
                                        <td>
                                            <button
                                                type="button"
                                                className="btn btn-danger btn-extra-sm"
                                                onClick={() => instrument.data ? deletePrice({ instrumentId: instrument.data.id, priceId: price.id }) : undefined}>
                                                    Remove
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </LoadingWrapper>
                    </table>
                    <div className="float-right">
                        <PageSelector
                            page={page}
                            totalPages={prices.data ? prices.data.totalCount / pageLimit : 1}
                            onPageChange={(p) => setPage(p)}
                            prefetch={(p) => prefetchPrices({ instrumentId, page: p, limit: pageLimit })}
                        />
                    </div>
                </div>
            </div>
            <ModalWrapper isOpen={modalIsOpen} closeModal={() => setModalIsOpen(false)}>
                { instrument.data && <CreateInstrumentPriceForm instrumentId={instrument.data.id} onSuccess={() => setModalIsOpen(false)} /> }
            </ModalWrapper>
        </LoadingWrapper>
    )
}
