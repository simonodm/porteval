import React, { useEffect, useMemo, useState } from 'react';
import Select from 'react-select';
import PageSelector from '../ui/PageSelector';
import useUserSettings from '../../hooks/useUserSettings';
import DataTable, { ColumnDefinition } from './DataTable';

import Button from 'react-bootstrap/Button';

import { useDeleteInstrumentPriceMutation, useGetInstrumentPricePageQuery } from '../../redux/api/instrumentApi';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { formatDateTimeString, getPriceString } from '../../utils/string';
import { AggregationFrequency, InstrumentPrice } from '../../types';

type Props = {
    /**
     * ID of the instrument to load and display prices for.
     */
    instrumentId: number;

    /**
     * Currency code to display prices in.
     */
    currencyCode?: string;
}

type SelectOption = {
    label: string;
    value?: AggregationFrequency;
}

/**
 * Loads instrument prices and renders an instrument prices' table.
 * 
 * @category Tables
 * @component
 */
function InstrumentPricesTable({ instrumentId, currencyCode }: Props): JSX.Element {
    const [page, setPage] = useState(1);
    const [pageLimit] = useState(100);
    const [frequency, setFrequency] = useState<AggregationFrequency | undefined>(undefined);
    const [compressPrices, setCompressPrices] = useState(false);

    const frequencyOptions: Array<SelectOption> = [
        { label: 'All', value: undefined },
        { label: 'Hourly', value: 'hour' },
        { label: 'Daily', value: 'day' },
        { label: 'Weekly', value: 'week' },
        { label: 'Monthly', value: 'month' },
        { label: 'Yearly', value: 'year' }
    ]

    const prices = useGetInstrumentPricePageQuery(
        { instrumentId, page, limit: pageLimit, frequency: frequency, compressed: compressPrices }
    );
    const [deletePrice, mutationStatus] = useDeleteInstrumentPriceMutation()

    const [userSettings] = useUserSettings();

    // reset page to 1 if price aggregation frequency changes
    useEffect(() => {
        setPage(1);
    }, [frequency]);

    const columns: Array<ColumnDefinition<InstrumentPrice>> = useMemo(() => [
        {
            id: 'date',
            header: 'Date',
            accessor: p => p.time,
            render: p => formatDateTimeString(p.time, userSettings.dateFormat + ' ' + userSettings.timeFormat)
        },
        {
            id: 'price',
            header: 'Price',
            accessor: p => p.price,
            render: p => getPriceString(p.price, currencyCode, userSettings)
        },
        {
            id: 'actions',
            header: 'Actions',
            render: p => (
                <Button
                    variant="danger"
                    className="btn-xs"
                    onClick={() => deletePrice({
                        instrumentId: instrumentId,
                        priceId: p.id 
                    })}
                >
                    Remove
                </Button>
            )
        }
    ], [])

    const isLoaded = checkIsLoaded(prices, mutationStatus);
    const isError = checkIsError(prices, mutationStatus);

    return (
        <div className="prices-table">
            <div className="prices-table-configuration">
                <div className="float-right">
                    <PageSelector
                        onPageChange={(p) => setPage(p)}
                        page={page}
                        totalPages={prices.data ? prices.data.totalCount / pageLimit : 1}
                    />
                </div>
                <Select
                    className="float-right mr-2"
                    aria-label='Price aggregation frequency'
                    defaultValue={frequencyOptions[0]}
                    onChange={(newValue) => newValue && setFrequency(newValue.value)}
                    options={frequencyOptions}
                />
                <div className="form-check float-left">
                    <input id="compress-prices" className="form-check-input" type="checkbox"
                        checked={compressPrices} onClick={() => setCompressPrices(!compressPrices)}
                    />
                    <label htmlFor="compress-prices" className="form-check-label">Only display price changes</label>
                </div>
            </div>
            
            <DataTable 
                className="w-100 entity-list"
                columnDefinitions={columns}
                ariaLabel={`Instrument ${instrumentId} prices table`}
                data={{
                    data: prices.data?.data ?? [],
                    isLoading: !isLoaded,
                    isError: isError
                }}
            />

            <div className="float-right">
                <PageSelector
                    onPageChange={(p) => setPage(p)}
                    page={page}
                    totalPages={prices.data ? prices.data.totalCount / pageLimit : 1}
                />
            </div>
        </div>
    )
}

export default InstrumentPricesTable;