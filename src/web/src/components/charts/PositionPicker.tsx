import React, { useContext } from 'react';

import { useGetPositionsQuery } from '../../redux/api/positionApi';
import { Portfolio } from '../../types';
import LoadingWrapper from '../ui/LoadingWrapper';
import PositionPickerItem from '../ui/PositionPickerItem';
import ChartLineConfigurationContext from '../../context/ChartLineConfigurationContext';
import { checkIsLoaded, checkIsError } from '../../utils/queries';

type Props = {
    portfolio: Portfolio;
}

export default function PositionPicker({ portfolio }: Props): JSX.Element {
    const context = useContext(ChartLineConfigurationContext);
    const positions = useGetPositionsQuery(portfolio.id);

    const isLoaded = checkIsLoaded(positions);
    const isError = checkIsError(positions);
    
    const handleAddAllPositions = () => {
        const positionsToAdd = positions.data?.filter(
            position => 
                !context.chart?.lines.find(line => line.type === 'position' && line.positionId === position.id)
        ) ?? [];

        context.addPortfolioPositionLines(positionsToAdd);
    }

    return (
        <div className="chart-picker">
            <h6>Positions</h6>
            <LoadingWrapper isError={isError} isLoaded={isLoaded}>
                <button
                    className="btn btn-primary"
                    onClick={handleAddAllPositions}
                    role="button"
                >Add all positions
                </button>
                {positions.data?.map(position => {
                    const line = context.chart?.lines.find(existingLine =>
                        existingLine.type === 'position'
                        && existingLine.positionId === position.id);

                    return (
                        <PositionPickerItem
                            key={position.id}
                            line={line}
                            onLineAdd={() => context.addPositionLine(position)}
                            onLineConfigure={() => line ? context.configureLine(line) : undefined}
                            onLineRemove={() => line ? context.removeLine(line) : undefined}
                            position={position}
                        />
                    )
                })}
            </LoadingWrapper>
        </div>
    );
}