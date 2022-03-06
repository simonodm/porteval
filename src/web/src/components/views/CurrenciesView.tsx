import React, { useEffect, useState } from 'react'
import { skipToken } from '@reduxjs/toolkit/dist/query';
import { toast } from 'react-toastify';

import { useGetAllKnownCurrenciesQuery,
    useGetLatestExchangeRatesQuery, useUpdateCurrencyMutation } from '../../redux/api/currencyApi';
import { Currency } from '../../types';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { getPriceString } from '../../utils/string';
import LoadingWrapper from '../ui/LoadingWrapper';
import PageHeading from '../ui/PageHeading';
import './CurrenciesView.css';
import useUserSettings from '../../hooks/useUserSettings';

export default function CurrenciesView(): JSX.Element {
    const currencies = useGetAllKnownCurrenciesQuery();
    const [selectedCurrency, setSelectedCurrency] = useState<Currency | undefined>(undefined);
    const exchangeRates = useGetLatestExchangeRatesQuery(selectedCurrency?.code ?? skipToken);
    const [updateCurrency] = useUpdateCurrencyMutation();

    const [userSettings] = useUserSettings();

    const isLoaded = checkIsLoaded(currencies, exchangeRates);
    const isError = checkIsError(currencies, exchangeRates);

    useEffect(() => {
        if(currencies.data) {
            const foundDefault = currencies.data.find(c => c.isDefault);
            if(foundDefault) {
                setSelectedCurrency(foundDefault);
            }
        }
    }, [currencies.data]);

    const onCurrencyChange = (currencyCode: string) => {
        const currency = currencies.data?.find(c => c.code === currencyCode);
        setSelectedCurrency(currency);
    }

    const onSave = () => {
        if(selectedCurrency !== undefined) {
            const updatedCurrency = {
                ...selectedCurrency,
                isDefault: true
            };

            setSelectedCurrency(updatedCurrency);
            updateCurrency(updatedCurrency).then(() => toast.success('Default currency saved.'));
        }
    }

    return (
        <>
            <PageHeading heading="Currencies" />
            <div className="container-fluid">
                <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                    <div className="row default-currency-selector">
                        <label htmlFor="currency-select">Choose default currency:</label>
                        <select id="currency-select" onChange={(e) => onCurrencyChange(e.target.value)}>
                            {
                                currencies.data?.map(currency =>
                                    <option
                                        key={currency.code}
                                        selected={currency.code === selectedCurrency?.code}
                                        value={currency.code}
                                    >
                                        {currency.code} ({currency.symbol})
                                    </option>)
                            }
                        </select>
                        <button className="btn btn-primary btn-sm" onClick={onSave} role="button">Save</button>
                    </div>
                    <div className="row mt-5">
                        <h5>Exchange rates</h5>
                        <table className="entity-list w-100">
                            <thead>
                                <tr>
                                    <th>Currency</th>
                                    <th>Exchange rate</th>
                                </tr>
                            </thead>
                            <tbody>
                                {
                                    exchangeRates.data?.map(exchangeRate => 
                                        <tr key={exchangeRate.id}>
                                            <td>{exchangeRate.currencyToCode}</td>
                                            <td>
                                                {
                                                    getPriceString(
                                                        exchangeRate.exchangeRate,
                                                        userSettings.decimalSeparator,
                                                        currencies.data?.find(
                                                            c => c.code === exchangeRate.currencyToCode
                                                        )?.symbol
                                                    )
                                                }
                                            </td>
                                        </tr>)
                                }
                            </tbody>
                        </table>
                    </div>
                </LoadingWrapper>
            </div>
        </>
    )
}