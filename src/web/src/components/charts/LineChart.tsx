import React, { useLayoutEffect, useRef } from 'react';
import createChart, { LineChartLine, RenderCallback, TooltipCallback } from '../../utils/lineChart';
import * as d3 from 'd3';

import { getXAxisD3Interval } from '../../utils/chart';

import './LineChart.css';

/**
 * Represents possible intervals for chart X axis labels.
 * @category Chart
 */
export type XAxisInterval = 'hour' | 'day' | 'week' | 'month' | 'year';

/**
 * Represents chart rendering configuration.
 */
type ChartRenderConfiguration = {
    /**
     * X axis label intervals.
     */
    xInterval?: XAxisInterval;

    /**
     * X axis label formatting function.
     */
    xFormat?: (xValue: Date) => string;

    /**
     * X value tooltip formatting function
     */
    xTooltipFormat?: (xValue: Date) => string;

    /**
     * Y axis label formatting function.
     */
    yFormat?: (yValue: number) => string;

    /**
     * Tooltip rendering callback. The result is appended to the base tooltip.
     */
    tooltipCallback?: TooltipCallback;

    /**
     * Additional rendering callback. The result is rendered on top of the chart line.
     */
    additionalRenderCallback?: RenderCallback;
}

type Props = {
    /**
     * Chart rendering configuration.
     */
    config?: ChartRenderConfiguration,

    /**
     * An array of lines to render in the chart.
     */
    lines: Array<LineChartLine>
}

/**
 * Renders a line chart based on the provided render configuration and line data.
 * 
 * @category Chart
 * @component
 */
function LineChart({ config, lines }: Props): JSX.Element {
    const containerRef = useRef<HTMLDivElement>(null);

    const interval = getXAxisD3Interval(config?.xInterval);
    
    const generateChart = () => {
        if(containerRef.current !== null) {
            containerRef.current.innerHTML = '';

            const chart = createChart()
                .withLines(lines)
                .withXFormat(config?.xFormat ?? d3.timeFormat('%b %d'))
                .withXTooltipFormat(config?.xTooltipFormat ?? d3.timeFormat('%b %d'))
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

    // refresh chart if window is resized
    useLayoutEffect(() => {
        window.addEventListener('resize', generateChart);

        return () => {
            window.removeEventListener('resize', generateChart)
        }
    })

    // refresh chart if dimensions or configuration changes
    useLayoutEffect(generateChart,
        [config, lines, containerRef, containerRef.current?.clientHeight, containerRef.current?.clientWidth]);

    return (
        <div className="chart" aria-label="chart" ref={containerRef}>
        </div>
    )
}

export default LineChart;