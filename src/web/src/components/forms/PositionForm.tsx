import React, { useState } from 'react';
import { useGetAllInstrumentsQuery } from '../../redux/api/instrumentApi';
import { useGetAllPortfoliosQuery } from '../../redux/api/portfolioApi';
import LoadingWrapper from '../ui/LoadingWrapper';
import { checkIsLoaded, checkIsError } from '../utils/queries';
import PortfolioDropdown from './fields/PortfolioDropdown';
import InstrumentDropdown from './fields/InstrumentDropdown';
import TextInput from './fields/TextInput';

type Props = {
    portfolioId?: number;
    instrumentId?: number;
    note?: string;
    onSubmit: (portfolioId: number, instrumentId: number, note: string) => void;
}

export default function PositionForm({ portfolioId: defaultPortfolioId, instrumentId: defaultInstrumentId, note: defaultNote, onSubmit }: Props): JSX.Element {
    const [portfolioId, setPortfolioId] = useState<number | undefined>(defaultPortfolioId);
    const [instrumentId, setInstrumentId] = useState<number | undefined>(defaultInstrumentId)
    const [note, setNote] = useState(defaultNote ?? '');

    const instruments = useGetAllInstrumentsQuery();
    const portfolios = useGetAllPortfoliosQuery();

    const isLoaded = checkIsLoaded(instruments, portfolios);
    const isError = checkIsError(instruments, portfolios);

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        if(portfolioId !== undefined && instrumentId !== undefined) {
            onSubmit(portfolioId, instrumentId, note)
        }

        e.preventDefault();
    }

    return (
        <LoadingWrapper isLoaded={isLoaded} isError={isError}>
            <form onSubmit={handleSubmit}>
                <PortfolioDropdown portfolios={portfolios.data ?? []} defaultPortfolioId={portfolioId} onChange={(id) => setPortfolioId(id)} />
                <InstrumentDropdown instruments={instruments.data ?? []} defaultInstrumentId={instrumentId} onChange={(id) => setInstrumentId(id)} />
                <TextInput label='Note' onChange={(val) => setNote(val)} />
                <button 
                    role="button"
                    className="btn btn-primary"
                    >Save</button>
            </form>
        </LoadingWrapper>
    )
}