import React from 'react';

import LoadingWrapper from '../ui/LoadingWrapper';


import { checkIsLoaded, checkIsError } from '../utils/queries';
import { useGetPositionsQuery } from '../../redux/api/positionApi';

import PositionRow from './PositionRow';

type Props = {
    portfolioId: number;
}

export default function PositionRows({ portfolioId }: Props): JSX.Element {
    const positions = useGetPositionsQuery(portfolioId);
    const isLoaded = checkIsLoaded(positions);
    const isError = checkIsError(positions);

    return (
        <LoadingWrapper isError={isError} isLoaded={isLoaded}>
            {positions.data?.map(position => <PositionRow key={`position_${position.id}`} position={position} />)}
        </LoadingWrapper>
    )
}