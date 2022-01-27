import { useLayoutEffect, useRef } from 'react';
import * as d3 from 'd3';
import createChart from '../utils/lineChart';
import './LineChart.css';
import { EntityChartDataPoint } from '../../types';
import { getXAxisD3Interval } from '../utils/chart';

type LineSettings = {
    name: string;
    color: string;
    strokeDashArray: string;
    width: number;
}

export type XAxisInterval = 'hour' | 'day' | 'week' | 'month' | 'year';

type ChartConfig = {
    xInterval?: XAxisInterval;
    yFormat?: (yValue: number) => string;
    xFormat?: (xValue: Date) => string;
}

type Props = {
    config?: ChartConfig,
    data: Array<Array<EntityChartDataPoint>>
    lineSettings: Array<LineSettings>;
}

export default function LineChart({ config, data, lineSettings }: Props): JSX.Element {
    const containerRef = useRef<HTMLDivElement>(null);

    const parseTime = d3.isoParse;
    const interval = getXAxisD3Interval(config?.xInterval);
    
    const generateChart = () => {
        if(containerRef.current !== null) {
            containerRef.current.innerHTML = '';

            createChart()
                .withData(data)
                .withXKey('time')
                .withYKey('value')
                .withXParser(parseTime)
                .withXFormat(config?.xFormat ?? d3.timeFormat('%b %d'))
                .withYFormat(config?.yFormat ?? ((y: number) => y.toString()))
                .withLineSettings(lineSettings)
                .withInterval(interval)
                .appendTo(containerRef.current);
        }
    }


    useLayoutEffect(() => {
        window.addEventListener('resize', generateChart);

        return () => {
            window.removeEventListener('resize', generateChart)
        }
    })

    useLayoutEffect(generateChart, [data, containerRef, containerRef.current?.clientHeight, containerRef.current?.clientWidth]);

    return (
        <div ref={containerRef} className="chart">
        </div>
    )
}


