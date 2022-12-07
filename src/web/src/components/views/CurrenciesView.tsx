import React, { useEffect, useState } from 'react'
import LoadingWrapper from '../ui/LoadingWrapper';
import PageHeading from '../ui/PageHeading';
import ExchangeRatesTable from '../tables/ExchangeRatesTable';

import { useGetAllKnownCurrenciesQuery,
    useGetLatestExchangeRatesQuery, useUpdateCurrencyMutation } from '../../redux/api/currencyApi';
import { Currency } from '../../types';
import { checkIsLoaded, checkIsError } from '../../utils/queries';
import { skipToken } from '@reduxjs/toolkit/dist/query';
import { toast } from 'react-toastify';

import './CurrenciesView.css';

/**
 * Renders the currencies' and their exchange rates view.
 * 
 * @category Views
 * @component
 */
function CurrenciesView(): JSX.Element {
    const currencies = useGetAllKnownCurrenciesQuery();
    const [selectedCurrency, setSelectedCurrency] = useState<Currency | undefined>(undefined);
    const exchangeRates = useGetLatestExchangeRatesQuery(selectedCurrency?.code ?? skipToken);
    const [updateCurrency] = useUpdateCurrencyMutation();

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
                        {
                            selectedCurrency !== undefined &&
                                <>
                                    <h5>Exchange rates</h5>
                                    <ExchangeRatesTable sourceCurrencyCode={selectedCurrency.code} />
                                </>
                        }
                    </div>
                </LoadingWrapper>
            </div>
        </>
    )
}

export default CurrenciesView;