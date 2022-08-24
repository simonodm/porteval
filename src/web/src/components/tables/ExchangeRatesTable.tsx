import React, { useMemo } from 'react';
import useUserSettings from '../../hooks/useUserSettings';
import { useGetLatestExchangeRatesQuery } from '../../redux/api/currencyApi';
import { CurrencyExchangeRate } from '../../types';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { getPriceString } from '../../utils/string';
import DataTable, { ColumnDefinition } from './DataTable';

type Props = {
    sourceCurrencyCode: string;
}

export default function ExchangeRatesTable({ sourceCurrencyCode }: Props): JSX.Element {
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
            data={{
                data: exchangeRates.data ?? [],
                isLoading: !isLoaded,
                isError: isError
            }}
        />
    )
}