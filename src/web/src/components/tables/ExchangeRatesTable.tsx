import React, { useMemo } from 'react';
import useUserSettings from '../../hooks/useUserSettings';
import DataTable, { ColumnDefinition } from './DataTable';

import { useGetLatestExchangeRatesQuery } from '../../redux/api/currencyApi';
import { CurrencyExchangeRate } from '../../types';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { getPriceString } from '../../utils/string';

type Props = {
    /**
     * Currency code of the source currency to be used for conversion.
     */
    sourceCurrencyCode: string;
}

/**
 * Loads exchange rates for the specified currency and renders an exchange rates table.
 * 
 * @category Tables
 * @component
 */
function ExchangeRatesTable({ sourceCurrencyCode }: Props): JSX.Element {
    const exchangeRates = useGetLatestExchangeRatesQuery(sourceCurrencyCode);
    
    const [userSettings] = useUserSettings();

    const columns: Array<ColumnDefinition<CurrencyExchangeRate>> = useMemo(() => [
        {
            id: 'currencyToCode',
            header: 'Currency',
            accessor: r => r.currencyToCode
        },
        {
            id: 'exchangeRate',
            header: 'Exchange rate',
            accessor: r => r.exchangeRate,
            render: r => getPriceString(r.exchangeRate, r.currencyToCode, userSettings)
        }
    ], []);

    const isLoaded = checkIsLoaded(exchangeRates);
    const isError = checkIsError(exchangeRates);

    return (
        <DataTable
            className="w-100 entity-list"
            sortable
            columns={columns}
            idSelector={r => r.id}
            ariaLabel="Exchange rates table"
            data={{
                data: exchangeRates.data ?? [],
                isLoading: !isLoaded,
                isError: isError
            }}
        />
    )
}

export default ExchangeRatesTable;