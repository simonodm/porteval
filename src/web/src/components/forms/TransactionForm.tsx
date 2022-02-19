import { skipToken } from '@reduxjs/toolkit/dist/query';
import { DateTime } from 'luxon';
import React, { useEffect, useState } from 'react';
import { useGetInstrumentPriceAtQuery } from '../../redux/api/instrumentApi';
import { useGetAllPortfoliosQuery } from '../../redux/api/portfolioApi';
import { useGetPositionsQuery, useGetPositionQuery } from '../../redux/api/positionApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import PortfolioDropdown from './fields/PortfolioDropdown';
import PositionDropdown from './fields/PositionDropdown';
import NumberInput from './fields/NumberInput';
import DateTimeSelector from './fields/DateTimeSelector';
import TextInput from './fields/TextInput';

type Props = {
    portfolioId?: number;
    positionId?: number;
    amount?: number;
    price?: number;
    time?: DateTime;
    note?: string;
    onSubmit: (portfolioId: number, positionId: number, amount: number, price: number, time: DateTime, note: string) => void;
}

export default function TransactionForm({
        portfolioId: defaultPortfolioId,
        positionId: defaultPositionId,
        amount: defaultAmount,
        price: defaultPrice,
        time: defaultTime,
        note: defaultNote,
        onSubmit   
    }: Props): JSX.Element {
    const [portfolioId, setPortfolioId] = useState<number | undefined>(defaultPortfolioId ?? undefined);
    const [positionId, setPositionId] = useState<number | undefined>(defaultPortfolioId && defaultPositionId ? defaultPositionId : undefined);
    const [amount, setAmount] = useState<number>(defaultAmount ?? 1);
    const [price, setPrice] = useState<number>(defaultPrice ?? 0);
    const [time, setTime] = useState(defaultTime ?? DateTime.now());
    const [note, setNote] = useState(defaultNote ?? '');

    const portfolios = useGetAllPortfoliosQuery();
    const positions = useGetPositionsQuery(portfolioId ?? skipToken);
    const position = useGetPositionQuery(positionId ? { positionId } : skipToken);
    const instrumentPrice = useGetInstrumentPriceAtQuery(position.data ? { instrumentId: position.data.instrumentId, time: time.toISO() } : skipToken);

    const isLoaded = checkIsLoaded(portfolios, positions, position);
    const isError = checkIsError(portfolios, positions, position);

    useEffect(() => {
        if(!portfolios.isLoading && portfolios.data && portfolioId === undefined) {
            if(portfolios.data.length > 0) {
                setPortfolioId(portfolios.data[0].id);
            }
        }
    }, [portfolios.data])

    useEffect(() => {
        if(!positions.isLoading && positions.data && positionId === undefined) {
            if(positions.data.length > 0) {
                setPositionId(positions.data[0].id);
            }
        }
    }, [positions.data])

    useEffect(() => {
        if(!instrumentPrice.isLoading && instrumentPrice.data) {
            setPrice(instrumentPrice.data.price);
        }
    }, [instrumentPrice.data]);

    const handlePortfolioChange = (newPortfolioId: number) => {
        setPortfolioId(newPortfolioId);
        setPositionId(undefined);
    }

    const handlePositionChange = (newPositionId: number) => {
        setPositionId(newPositionId);
    }

    const handleAmountChange = (newAmount: number) => {
        setAmount(newAmount);
    }

    const handlePriceChange = (newPrice: number) => {
        setPrice(newPrice);
    }

    const handleTimeChange = (dt: DateTime) => {
        setTime(dt);
    }

    const handleNoteChange = (newNote: string) => {
        setNote(newNote);
    }

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        if(portfolioId !== undefined && positionId !== undefined) {
            onSubmit(portfolioId, positionId, amount, price, time, note);
        }

        e.preventDefault();
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={handleSubmit}>
                <PortfolioDropdown portfolios={portfolios.data ?? []} defaultPortfolioId={portfolioId} onChange={handlePortfolioChange} />
                <PositionDropdown positions={positions.data ?? []} defaultPositionId={positionId} onChange={handlePositionChange} />
                <NumberInput label='Amount' defaultValue={defaultAmount} allowNegativeValues allowFloat onChange={handleAmountChange} />
                <NumberInput label='Price' defaultValue={defaultPrice} allowFloat onChange={handlePriceChange} />
                <DateTimeSelector label='Date' format='MMM dd, yyyy, HH:mm' timeInterval={1} defaultTime={defaultTime} onChange={handleTimeChange} />
                <TextInput label='Note' defaultValue={defaultNote} onChange={handleNoteChange} />
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
    )
}