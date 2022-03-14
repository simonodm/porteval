import React, { useState } from 'react';

import Select from 'react-select';

import { useDeleteInstrumentPriceMutation, useGetInstrumentPricePageQuery,
    usePrefetch } from '../../redux/api/instrumentApi';
import { REFRESH_INTERVAL } from '../../constants';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { formatDateTimeString, getPriceString, getPerformanceString } from '../../utils/string';
import LoadingWrapper from '../ui/LoadingWrapper';
import PageSelector from '../ui/PageSelector';
import useUserSettings from '../../hooks/useUserSettings';
import { AggregationFrequency } from '../../types';

type Props = {
    instrumentId: number;
    currencySymbol?: string;
}

type SelectOption = {
    label: string;
    value: AggregationFrequency;
}

export default function InstrumentPricesTable({ instrumentId, currencySymbol }: Props): JSX.Element {
    const [page, setPage] = useState(1);
    const [pageLimit] = useState(100);
    const [frequency, setFrequency] = useState<AggregationFrequency>('5min');

    const frequencyOptions: Array<SelectOption> = [
        { label: '5 minutes', value: '5min' },
        { label: 'Hour', value: 'hour' },
        { label: 'Day', value: 'day' },
        { label: 'Week', value: 'week' },
        { label: 'Month', value: 'month' },
        { label: 'Year', value: 'year' }
    ]

    const prices = useGetInstrumentPricePageQuery(
        { instrumentId, page, limit: pageLimit, frequency: frequency },
        { pollingInterval: REFRESH_INTERVAL }
    );
    const prefetchPrices = usePrefetch('getInstrumentPricePage');
    const [deletePrice, mutationStatus] = useDeleteInstrumentPriceMutation()

    const [userSettings] = useUserSettings();

    const pricesLoaded = checkIsLoaded(prices, mutationStatus);
    const pricesError = checkIsError(prices, mutationStatus);

    return (
        <div className="prices-table">
            <div className="prices-table-configuration">
                <div className="float-right">
                    <PageSelector
                        onPageChange={(p) => setPage(p)}
                        page={page}
                        prefetch={(p) => 
                            prefetchPrices({ instrumentId, page: p, limit: pageLimit, frequency })}
                        totalPages={prices.data ? prices.data.totalCount / pageLimit : 1}
                    />
                </div>
                <Select
                    className="float-right mr-2"
                    defaultValue={frequencyOptions[0]}
                    onChange={(newValue) => newValue && setFrequency(newValue.value)}
                    options={frequencyOptions}
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
                <LoadingWrapper isError={pricesError} isLoaded={pricesLoaded}>
                    <tbody>
                        {prices.data?.data.map((price, index, array) => (
                            <tr key={price.id}>
                                <td>
                                    {
                                        formatDateTimeString(
                                            price.time,
                                            userSettings.dateFormat + ' ' + userSettings.timeFormat
                                        )
                                    }
                                </td>
                                <td>
                                    {
                                        getPriceString(
                                            price.price,
                                            currencySymbol,
                                            userSettings
                                        )
                                    }
                                </td>
                                <td>{index < array.length - 1
                                        ? getPerformanceString(
                                            price.price / array[index + 1].price - 1,
                                            userSettings
                                        )
                                        : getPerformanceString(0, userSettings)}
                                </td>
                                <td>
                                    <button
                                        className="btn btn-danger btn-extra-sm"
                                        onClick={() => deletePrice({
                                            instrumentId: instrumentId,
                                            priceId: price.id 
                                        })}
                                        type="button"
                                    >
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
                    onPageChange={(p) => setPage(p)}
                    page={page}
                    prefetch={(p) => 
                        prefetchPrices({ instrumentId, page: p, limit: pageLimit, frequency: 'day' })}
                    totalPages={prices.data ? prices.data.totalCount / pageLimit : 1}
                />
            </div>
        </div>
        
    )
}