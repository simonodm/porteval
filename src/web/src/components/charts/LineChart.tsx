import React, { useLayoutEffect, useRef } from 'react';
import * as d3 from 'd3';

import createChart, { RenderCallback , TooltipCallback } from '../utils/lineChart';
import './LineChart.css';
import { EntityChartDataPoint } from '../../types';
import { getXAxisD3Interval } from '../utils/chart';


export type XAxisInterval = 'hour' | 'day' | 'week' | 'month' | 'year';
export type Line = {
    name: string;
    color: string;
    strokeDashArray: string;
    width: number;
    data: Array<EntityChartDataPoint>
}

type ChartConfig = {
    xInterval?: XAxisInterval;
    yFormat?: (yValue: number) => string;
    xFormat?: (xValue: Date) => string;
    tooltipCallback?: TooltipCallback;
    additionalRenderCallback?: RenderCallback;
}

type Props = {
    config?: ChartConfig,
    lines: Array<Line>
}

export default function LineChart({ config, lines }: Props): JSX.Element {
    const containerRef = useRef<HTMLDivElement>(null);

    const interval = getXAxisD3Interval(config?.xInterval);
    
    const generateChart = () => {
        if(containerRef.current !== null) {
            containerRef.current.innerHTML = '';

            const chart = createChart()
                .withLines(lines)
                .withXFormat(config?.xFormat ?? d3.timeFormat('%b %d'))
                .withYFormat(config?.yFormat ?? ((y: number) => y.toString()))
                .withInterval(interval)

            if(config?.tooltipCallback) {
                chart.withTooltipCallback(config.tooltipCallback)
            }

            if(config?.additionalRenderCallback) {
                chart.withAdditionalRenderCallback(config.additionalRenderCallback)
            }

            chart.appendTo(containerRef.current);
        }
    }


    useLayoutEffect(() => {
        window.addEventListener('resize', generateChart);

        return () => {
            window.removeEventListener('resize', generateChart)
        }
    })

    useLayoutEffect(generateChart,
        [lines, containerRef, containerRef.current?.clientHeight, containerRef.current?.clientWidth]);

    return (
        <div className="chart" ref={containerRef}>
        </div>
    )
}


