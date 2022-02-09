import { useLayoutEffect, useRef } from 'react';
import * as d3 from 'd3';
import createChart from '../utils/lineChart';
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
    tooltipCallback?: (from: string, to: string) => HTMLElement | null;
    additionalRenderCallback?: (from: string, to: string) => HTMLElement | null;
}

type Props = {
    config?: ChartConfig,
    lines: Array<Line>
}

export default function LineChart({ config, lines }: Props): JSX.Element {
    const containerRef = useRef<HTMLDivElement>(null);

    const parseTime = d3.isoParse;
    const interval = getXAxisD3Interval(config?.xInterval);
    
    const generateChart = () => {
        if(containerRef.current !== null) {
            containerRef.current.innerHTML = '';

            createChart()
                .withLines(lines)
                .withXKey('time')
                .withYKey('value')
                .withXParser(parseTime)
                .withXFormat(config?.xFormat ?? d3.timeFormat('%b %d'))
                .withYFormat(config?.yFormat ?? ((y: number) => y.toString()))
                .withInterval(interval)
                .withTooltipCallback(config?.tooltipCallback)
                .withAdditionalRenderCallback(config?.additionalRenderCallback)
                .appendTo(containerRef.current);
        }
    }


    useLayoutEffect(() => {
        window.addEventListener('resize', generateChart);

        return () => {
            window.removeEventListener('resize', generateChart)
        }
    })

    useLayoutEffect(generateChart, [lines, containerRef, containerRef.current?.clientHeight, containerRef.current?.clientWidth]);

    return (
        <div ref={containerRef} className="chart">
        </div>
    )
}


