import React, { useCallback, useState } from 'react'

import { ChartLine } from '../../types';
import { convertDashToStrokeDashArray } from '../../utils/chart';

type Props = {
    line: ChartLine;
    length: number;
}

type ContainerRefCallback = (node: HTMLSpanElement) => void;

export default function LinePreview({ line, length }: Props): JSX.Element {
    const [height, setHeight] = useState(0);

    const containerRef = useCallback<ContainerRefCallback>(node => {
        if(node !== null) {
            setHeight(node.getBoundingClientRect().height);
        }
    }, [])

    return (
        <span ref={containerRef}>
            <svg height={height} width={length}>
                <line style={{
                        stroke: line.color,
                        strokeWidth: line.width,
                        strokeDasharray: convertDashToStrokeDashArray(line.dash)
                    }} 
                    x1={0}
                    x2={length}
                    y1={height / 2}
                    y2={height / 2} 
                />
            </svg>
        </span>
        
    )
}