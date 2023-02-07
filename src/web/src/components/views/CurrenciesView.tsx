import React, { useMemo } from 'react'
import LoadingWrapper from '../ui/LoadingWrapper';
import PageHeading from '../ui/PageHeading';
import ExchangeRatesTable from '../tables/ExchangeRatesTable';
import ChangeDefaultCurrencyForm from '../forms/ChangeDefaultCurrencyForm';

import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

import { useGetAllKnownCurrenciesQuery, useGetLatestExchangeRatesQuery } from '../../redux/api/currencyApi';
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

    const defaultCurrency = useMemo(() => currencies.data?.find(c => c.isDefault), [currencies.data]);

    const exchangeRates = useGetLatestExchangeRatesQuery(defaultCurrency?.code ?? skipToken);

    const isLoaded = checkIsLoaded(currencies, exchangeRates);
    const isError = checkIsError(currencies, exchangeRates);

    return (
        <>
            <PageHeading heading="Currencies" />
            <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                <Container fluid>
                    <Row className="mb-5">
                        <Col>
                            <ChangeDefaultCurrencyForm
                                currencies={currencies?.data ?? []}
                                onSuccess={() => toast.success('Default currency saved.')}
                            />
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <h5>Exchange rates</h5>
                            {
                                defaultCurrency &&
                                    <ExchangeRatesTable sourceCurrencyCode={defaultCurrency?.code} />
                            }
                        </Col>                        
                    </Row>
                </Container>
            </LoadingWrapper>
        </>
    )
}

export default CurrenciesView;