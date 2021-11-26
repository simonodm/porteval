import React, { useEffect, useState } from 'react';
import { useGetAllInstrumentsQuery } from '../../redux/api/instrumentApi';
import { useGetAllPortfoliosQuery } from '../../redux/api/portfolioApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';

type Props = {
    portfolioId?: number;
    instrumentId?: number;
    note?: string;
    onSubmit: (portfolioId: number, instrumentId: number, note: string) => void;
}

export default function PositionForm({ portfolioId: defaultPortfolioId, instrumentId: defaultInstrumentId, note: defaultNote, onSubmit }: Props): JSX.Element {
    const instruments = useGetAllInstrumentsQuery();
    const portfolios = useGetAllPortfoliosQuery();

    const [portfolioId, setPortfolioId] = useState<number | undefined>(defaultPortfolioId);
    const [instrumentId, setInstrumentId] = useState<number | undefined>(defaultInstrumentId)
    const [note, setNote] = useState(defaultNote ?? '');

    const isLoaded = checkIsLoaded(instruments, portfolios);
    const isError = checkIsError(instruments, portfolios);

    useEffect(() => {
        if(!portfolios.isLoading && portfolios.data && portfolioId === undefined) {
            if(portfolios.data.length > 0) {
                setPortfolioId(portfolios.data[0].id);
            }
        }
    }, [portfolios.isLoading])

    useEffect(() => {
        if(!instruments.isLoading && instruments.data && instrumentId === undefined) {
            const firstNonIndexInstrument = instruments.data.find(i => i.type !== 'index');
            if(firstNonIndexInstrument) {
                setInstrumentId(firstNonIndexInstrument.id);
            }
        }
    }, [instruments.isLoading])

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        if(portfolioId !== undefined && instrumentId !== undefined) {
            onSubmit(portfolioId, instrumentId, note)
        }

        e.preventDefault();
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label htmlFor="position-portfolio">Portfolio:</label>
                    <select id="position-portfolio" className="form-control" disabled={defaultPortfolioId !== undefined} onChange={(e) => setPortfolioId(parseInt(e.target.value))}>
                        {portfolios.data?.map(portfolio => <option value={portfolio.id} selected={portfolio.id === portfolioId}>{portfolio.name}</option>)}
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="position-instrument">Instrument:</label>
                    <select id="position-instrument" className="form-control" disabled={defaultInstrumentId !== undefined} onChange={(e) => setInstrumentId(parseInt(e.target.value))}>
                        {instruments.data && instruments.data
                                .filter(instrument => instrument.type !== 'index')
                                .map(instrument => <option value={instrument.id} selected={instrument.id === instrumentId}>{instrument.name}</option>)}
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="position-note">Note:</label>
                    <input type="text" id="position-note" placeholder="Note" className="form-control" value={note} onChange={(e) => setNote(e.target.value)}></input>
                </div>
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
    )
}