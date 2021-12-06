import { skipToken } from '@reduxjs/toolkit/dist/query';
import { DateTime } from 'luxon';
import React, { useEffect, useState } from 'react';
import { useGetInstrumentPriceAtQuery } from '../../redux/api/instrumentApi';
import { useGetAllPortfoliosQuery } from '../../redux/api/portfolioApi';
import { useGetPositionsQuery, useGetPositionQuery } from '../../redux/api/positionApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import DatePicker from 'react-datepicker';

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
    const [inputAmount, setInputAmount] = useState(defaultAmount?.toString() ?? '1'); // separate state for amount <input> - done to allow typing minus for negative numbers
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

    const handlePortfolioChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newPortfolioId = parseInt(e.target.value);
        if(!isNaN(newPortfolioId)) {
            setPortfolioId(newPortfolioId);
            setPositionId(undefined);
        }
    }

    const handlePositionChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const newPositionId = parseInt(e.target.value);
        if(!isNaN(newPositionId)) {
            setPositionId(newPositionId);
        }
    }

    const handleAmountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setInputAmount(e.target.value);
        const parsedFloat = parseFloat(e.target.value);
        if(parsedFloat) {
            setAmount(parseFloat(e.target.value));
        }
    }

    const handlePriceChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPrice(parseFloat(e.target.value));
    }

    const handleTimeChange = (date: Date) => {
        const dt = DateTime.fromJSDate(date);
        if(dt) {
            setTime(dt);
        }
    }

    const handleNoteChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setNote(e.target.value);
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
                <div className="form-group">
                    <label htmlFor="transaction-portfolio">Portfolio:</label>
                    <select id="transaction-portfolio" className="form-control" disabled={defaultPortfolioId !== undefined} onChange={handlePortfolioChange}>
                        {portfolios.data?.map(portfolio => <option value={portfolio.id} selected={portfolio.id === portfolioId}>{portfolio.name}</option>)}
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="transaction-position">Position:</label>
                    <select disabled={positions?.data === undefined || defaultPositionId !== undefined} id="portfolio-position" className="form-control" onChange={handlePositionChange}>
                        {positions.data?.map(position => <option value={position.id} selected={position.id === positionId}>{position.instrument.name}</option>)}
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="transaction-amount">Amount:</label>
                    <input
                        type="number"
                        id="transaction-amount"
                        className="form-control"
                        value={inputAmount}
                        disabled={defaultAmount !== undefined}
                        onChange={handleAmountChange} />
                </div>
                <div className="form-group">
                    <label htmlFor="transaction-price">Price:</label>
                    <input
                        type="number"
                        id="transaction-price"
                        className="form-control"
                        value={price}
                        disabled={defaultPrice !== undefined}
                        onChange={handlePriceChange} />
                </div>
                <div className="form-group">
                    <label htmlFor="transaction-time">Time:</label>
                    <DatePicker
                        selected={time.toJSDate()}
                        onChange={handleTimeChange}
                        showTimeSelect
                        timeIntervals={1}
                        dateFormat="MMM dd, yyyy, HH:mm"
                        disabled={defaultTime !== undefined}
                        id="transaction-time" />
                </div>
                <div className="form-group">
                    <label htmlFor="transaction-note">Note:</label>
                    <input 
                        type="text"
                        id="position-note"
                        placeholder="Note"
                        className="form-control"
                        value={note}
                        onChange={handleNoteChange} />
                </div>
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
    )
}