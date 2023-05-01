import * as d3 from 'd3';
import { CHART_TICK_INTERVAL } from '../constants';

/**
 * Represents complete data needed to render a line in a line chart.
 * @category Chart
 * @subcategory LineChart
 */
export type LineChartLine = {
    /**
     * Line name.
     */
    name: string;

    /**
     * Line color.
     */
    color: string;

    /**
     * Line strokeDashArray as expected by SVG CSS.
     */
    strokeDashArray: string;

    /**
     * Line width in pixels.
     */
    width: number;

    /**
     * Line data.
     */
    data: Array<LineChartLineDataPoint>;
}

/**
 * Represents a single chart line data point to render.
 * @category Chart
 * @subcategory LineChart
 */
export type LineChartLineDataPoint = {
    /**
     * Data point time in ISO format.
     */
    time: string;

    /**
     * Data point value.
     */
    value: number;
}

/**
 * Represents a single rendered chart line data point.
 * @category Chart
 * @subcategory LineChart
 */
export type RenderedLineChartLineDataPoint = {
    /**
     * X value.
     */
    x: number;

    /**
     * Y value.
     */
    y: number;

    /**
     * Original line data point.
     */
    point: LineChartLineDataPoint;

    /**
     * Next line data point if such exists.
     */
    nextPoint: LineChartLineDataPoint | undefined;

    /**
     * Index of the line as perceived by the line chart.
     */
    lineIndex: number;
}

/**
 * A callback to render additional tooltip information.
 * 
 * @category Chart
 * @subcategory LineChart
 * @param from ISO string of the start of the date range represented by the current hover point.
 * @param to ISO string of the end of the date range represented by the current hover point.
 * @returns An HTML element representing content to append to existing tooltip.
 */
export type TooltipCallback = (from: string | undefined, to: string | undefined) => HTMLElement | null;

/**
 * A callback to render additional SVG on the line point represented by the provided rendered data point.
 * 
 * @category Chart
 * @subcategory LineChart
 * @param dataPoint Rendered data point to draw resulting SVG on top of.
 * @return An SVG element representing graphics to draw on top of the provided data point.
 */
export type RenderCallback = (dataPoint: RenderedLineChartLineDataPoint) => SVGElement;

const twoArgumentMemo = <TFirstArg, TSecondArg, TResult>(
    fn: (a: TFirstArg, b: TSecondArg) => TResult,
): (a: TFirstArg, b: TSecondArg) => TResult => {
    const cache = new Map<TFirstArg, Map<TSecondArg, TResult>>();
    
    return (firstArg, secondArg) => {
        let first = cache.get(firstArg);
        const second = first?.get(secondArg);
        if(second !== undefined) {
            return second;
        }

        const value = fn(firstArg, secondArg);

        if(first === undefined) {
            first = new Map<TSecondArg, TResult>();
            cache.set(firstArg, first);
        }
        first.set(secondArg, value);
        return value;
    }
}

/**
 * Creates an empty line chart.
 * 
 * @category Chart
 * @subcategory LineChart
 * @returns An empty line chart.
 */
export default function createChart(): SVGLineChart {
    return new SVGLineChart();
}

/**
 * Represents an SVG line chart rendered using D3.
 * 
 * @category Chart
 * @subcategory LineChart
 */
class SVGLineChart {
    _lines: Array<LineChartLine> = [];
    _svg: d3.Selection<SVGGElement, unknown, null, undefined> | null = null;

    _xScale: d3.ScaleTime<number, number> | null = null;
    _xFormat: (_: Date) => string = d3.isoFormat;
    _xTooltipFormat: (_: Date) => string = d3.isoFormat;

    _yScale: d3.ScaleLinear<number, number> | null = null;
    _yFormat: (_: number) => string = (num) => `${num}`;

    _xInterval = d3.timeDay;

    _width = 0;
    _height = 0;

    _d3Lines: Array<d3.Line<LineChartLineDataPoint>> = [];
    _container: HTMLElement | null = null;
    
    _tooltip: d3.Selection<HTMLDivElement, unknown, null, undefined> | null = null;
    _tooltipLine: d3.Selection<SVGLineElement, unknown, null, undefined> | null = null;
    _tooltipCallback: TooltipCallback = () => null;

    _renderCallback: RenderCallback | null = null;

    _margins = { top: 0, left: 0, bottom: 0, right: 0 };
    _fontSize = 10;

    _rightSideYAxis = false;

    /**
     * Specifies lines to be rendered in the chart.
     * 
     * @param lines Lines to render in the chart.
     * @returns An updated {@link SVGLineChart}.
     */
    withLines(lines: typeof this._lines) {
        this._lines = lines;
        return this;
    }

    /**
     * Specifies X axis format to be used in the chart.
     * 
     * @param xFormat Date format function.
     * @returns An updated {@link SVGLineChart}.
     */
    withXFormat(xFormat: typeof this._xFormat) {
        this._xFormat = xFormat;
        return this;
    }

    /**
     * Specifies tooltip date format to be used in the chart.
     * 
     * @param xTooltipFormat Date format function.
     * @returns An updated {@link SVGLineChart}.
     */
    withXTooltipFormat(xTooltipFormat: typeof this._xTooltipFormat) {
        this._xTooltipFormat = xTooltipFormat;
        return this;
    }

    /**
     * Specifies Y axis format to be used in the chart.
     * 
     * @param yFormat Number format function.
     * @returns An updated {@link SVGLineChart}.
     */
    withYFormat(yFormat: typeof this._yFormat) {
        this._yFormat = yFormat;
        return this;
    }

    /**
     * Specifies `d3` time interval function to be used in the chart.
     * 
     * @param intervalFunction `d3` time interval function.
     * @returns An updated {@link SVGLineChart}.
     */
    withInterval(intervalFunction: typeof this._xInterval) {
        this._xInterval = intervalFunction;
        return this;
    }

    /**
     * Specifies an additional tooltip render callback to be used in the chart.
     * The result of this method is appended to the base tooltip.
     * 
     * @param tooltipCallback Tooltip render callback.
     * @returns An updated {@link SVGLineChart}.
     */
    withTooltipCallback(tooltipCallback: typeof this._tooltipCallback) {
        this._tooltipCallback = tooltipCallback;
        return this;
    }

    /**
     * Specifies an additional SVG line render callback to be used in the chart.
     * The result of this method is rendered on top of the data point provided in callback parameter.
     * 
     * @param renderCallback SVG render callback.
     * @returns An updated {@link SVGLineChart}.
     */
    withAdditionalRenderCallback(renderCallback: typeof this._renderCallback) {
        this._renderCallback = renderCallback;
        return this;
    }

    /**
     * Configures the chart to render the Y axis on the right side instead of left.
     * 
     * @returns An updated {@link SVGLineChart}
     */
    withRightSideYAxis() {
        this._rightSideYAxis = true;
        return this;
    }

    /**
     * Appends the chart to the specified HTML element.
     */
    appendTo(container: HTMLElement) {
        this._generateMargins();
        this._container = container;
        this._width = container.offsetWidth - this._margins.left - this._margins.right;
        this._height = container.offsetHeight -  this._margins.top - this._margins.bottom;
        this._svg = d3.select(container)
            .append('svg')
            .attr('transform', `translate(${this._margins.left}, ${this._margins.top})`)
            .attr('viewbox', `0, 0, ${this._width}, ${this._height}`)
            .style('width', this._width)
            .style('height', this._height)
            .style('overflow', 'visible')
            .style('font-size', this._fontSize)
            .append('g');
        this._generateScales();
        this._generateD3Lines();
        this._drawGridLines();
        this._drawDataLines();
        this._drawAxes();
        this._setupTooltip();
        return this;
    }

    _generateMargins() {
        // finds the longest value string among all data points in all lines
        const maxValueLength = this._lines
            .map(
                line => 
                    line.data
                        .map(d => this._yFormat(d.value).toString().length)
                        .reduce(
                            (prev, curr) => Math.max(prev, curr),
                            0
                        )
                )
            .reduce(
                (prev, curr) => Math.max(prev, curr),
                0
            );

        const yAxisMargin = maxValueLength * this._fontSize / 1.5

        this._margins.left = this._rightSideYAxis ? 5 : yAxisMargin;
        this._margins.right = this._rightSideYAxis ? yAxisMargin : 5;
        this._margins.bottom = this._fontSize * 2.2;
        this._margins.top = 5;
    }

    _generateScales() {
        const xValues: Array<Date> = [];
        const yValues: Array<number> = [];

        this._lines.forEach(line => {
            line.data.forEach(dataPoint => {
                xValues.push(new Date(dataPoint.time));
                yValues.push(dataPoint.value);
            })
        })
        
        this._xScale = d3.scaleTime()
            .domain(<[Date, Date]>d3.extent(xValues))
            .range([0, this._width]);
            
        this._yScale = d3.scaleLinear()
            .domain(<[number, number]>d3.extent(yValues))
            .range([this._height, 0]);
    }

    _generateD3Lines() {
        if(this._xScale && this._yScale) {
            this._lines.forEach(() => {
                const d3Line = d3.line<LineChartLineDataPoint>()
                    .x(d => this._xScale!(new Date(d.time)))
                    .y(d => this._yScale!(d.value))

                this._d3Lines.push(d3Line);
            });
        }
    }

    _drawAxes() {
        if(!this._xScale || !this._yScale) return;
        this._svg?.append('g')
            .attr('class', 'x-axis')
            .attr('transform', `translate(0, ${this._height})`)
            .style('font-size', this._fontSize)
            .call(
                d3.axisBottom<Date>(this._xScale)
                    .ticks(this._xInterval
                        ? Math.min(
                            this._xInterval.count(
                                this._xScale.domain()[0],
                                this._xScale.domain()[1]
                            ),
                            this._width / CHART_TICK_INTERVAL
                          )
                        : this._width / CHART_TICK_INTERVAL)
                    .tickSize(10)
                    .tickFormat(this._xFormat)
            )
        this._svg?.append('g')
            .attr('class', 'y-axis')
            .attr('transform', `translate(${this._rightSideYAxis ? this._width : 0}, 0)`)
            .style('font-size', this._fontSize)
            .call(
                this._rightSideYAxis
                    ?   
                        d3.axisRight<number>(this._yScale)
                            .ticks(this._height / CHART_TICK_INTERVAL)
                            .tickFormat(this._yFormat)
                    :  
                        d3.axisLeft<number>(this._yScale)
                            .ticks(this._height / CHART_TICK_INTERVAL)
                            .tickFormat(this._yFormat)
            )
    }

    _drawGridLines() {
        if(!this._xScale || !this._yScale) return;
        this._svg?.append('g')
            .attr('class', 'grid')
            .attr('transform', `translate(0,${this._height})`)
            .call(
                d3.axisBottom<Date>(this._xScale)
                    .ticks(this._xInterval
                        ? Math.min(
                            this._xInterval.count(
                                this._xScale.domain()[0],
                                this._xScale.domain()[1]
                            ),
                            this._width / CHART_TICK_INTERVAL
                          )
                        : this._width / CHART_TICK_INTERVAL)
                    .tickSize(-this._height)
                    .tickFormat(() => '')
            )
            .call(
                g => g.selectAll('line')
                    .style('stroke', '#efefef')
                    .style('stroke-dasharray', '0')
            );
        this._svg?.append('g')
            .attr('class', 'grid')
            .style('stroke-dasharray', '0')
            .call(
                d3.axisLeft<number>(this._yScale)
                    .ticks(this._height / CHART_TICK_INTERVAL)
                    .tickSize(-this._width)
                    .tickFormat(() => '')
            ).call(
                g => g.selectAll('line')
                    .style('stroke', '#efefef')
                    .style('stroke-dasharray', '0')
            );
    }

    _drawDataLines() {
        this._d3Lines.forEach((line, index) => {
            this._svg?.append('path')
                .datum(this._lines[index].data)
                .attr('fill', 'none')
                .attr('aria-label', `${this._lines[index].name} line`)
                .attr('role', 'line')
                .style('stroke', this._lines[index].color)
                .style('stroke-width', this._lines[index].width)
                .style('stroke-dasharray', this._lines[index].strokeDashArray)
                .attr('class', 'line')
                .attr('d', line);

            if(this._renderCallback !== null) {
                this._svg?.selectAll('.custom-render')
                    .data(this._lines[index].data)
                    .enter()
                    .append((dataPoint, pointIndex) => {
                        const nextPoint =
                            pointIndex < this._lines[index].data.length - 1
                                ? this._lines[index].data[pointIndex + 1]
                                : undefined;

                        const pointInfo: RenderedLineChartLineDataPoint = {
                            x: this._xScale!(new Date(dataPoint.time)),
                            y: this._yScale!(dataPoint.value),
                            point: dataPoint,
                            nextPoint: nextPoint,
                            lineIndex: index
                        };

                        return this._renderCallback!(pointInfo);
                    });
            }
        });
    }

    _setupTooltip() {
        const overlay = this._prepareTooltipContainer();
        if(overlay === undefined) {
            return;
        }

        const memoizedFindClosestPoints = twoArgumentMemo(this._findClosestDataPoints);
        const memoizedTooltipCallback = twoArgumentMemo(this._tooltipCallback);

        overlay.on(
            'mousemove',
            (e) => this._handleTooltipMouseMove(e, memoizedFindClosestPoints, memoizedTooltipCallback)
        );
        overlay.on('mouseout', this._clearTooltip);
    }

    _getContainerOffset(): [number, number] {
        if(!this._container) throw new Error('Chart container has not been initialized.');
        const containerRect = this._container.getBoundingClientRect();

        return [containerRect.top, containerRect.left];
    }

    _prepareTooltipContainer(): d3.Selection<SVGRectElement, unknown, null, unknown> | undefined {
        if(!this._container || !this._svg) return undefined;
        this._tooltip = d3.select(this._container).append('div')
            .style('position', 'absolute')
            .style('background-color', '#fff')
            .style('padding', '6px')
            .style('display', 'none')
            .style('border', '2px solid gray')
            .style('border-radius', '3px')
        this._tooltipLine = this._svg.append('line')
        const overlay = this._svg.append('rect')
            .attr('width', this._width)
            .attr('height', this._height)
            .attr('opacity', 0);

        return overlay;
    }

    _handleTooltipMouseMove(
        event: MouseEvent,
        memoizedFindClosestPoints: typeof this._findClosestDataPoints,
        memoizedTooltipCallback: typeof this._tooltipCallback
    ): void {
        if(!this._xScale || !this._container || !this._tooltip || !this._tooltipLine) return;
        if(this._lines.length > 0) {
            const longestLineIndex = this._lines.reduce<number>(
                (prev, curr, idx) => this._lines[idx].data.length >= this._lines[prev].data.length ? idx : prev, 0
            );
            const time = Math.floor(this._xScale.invert(d3.pointer(event)[0]).getTime());

            const [, currDataPoint, nextDataPoint] = memoizedFindClosestPoints(longestLineIndex, time);

            if(!currDataPoint) return;

            const x = new Date(currDataPoint.time);

            this._tooltipLine.attr('stroke', 'black')
                .attr('x1', this._xScale(x))
                .attr('x2', this._xScale(x))
                .attr('y1', 0)
                .attr('y2', this._height);
            this._tooltip.html(this._xTooltipFormat(new Date(currDataPoint.time)))
                .style('display', 'block')
                .selectAll()
                .data(this._lines).enter()
                .append('div')
                .style('color', d => d.color)
                .html((d, i) => {
                    const [, point] = memoizedFindClosestPoints(i, time);
                    return point
                        ? `${d.name}: ${this._yFormat(point ? point.value : 0)}`
                        : '';
                });

            const from = currDataPoint.time;
            const to = nextDataPoint ? nextDataPoint.time : undefined;
            const element = memoizedTooltipCallback(from, to);

            if(element) {
                this._tooltip.node()?.appendChild(element);
            }

            const tooltipWidth = this._tooltip.node()?.getBoundingClientRect()['width'] ?? 0;
            const tooltipHeight = this._tooltip.node()?.getBoundingClientRect()['height'] ?? 0;

            const [chartTopBound, chartLeftBound] = this._getContainerOffset();

            const offsetXToParent = event.clientX - chartLeftBound;
            const offsetYToParent = event.clientY - chartTopBound;

            const posX = offsetXToParent + tooltipWidth >= this._width
                ? offsetXToParent - tooltipWidth - 5
                : offsetXToParent + 5;
            const posY = offsetYToParent + tooltipHeight >= this._height
                ? offsetYToParent - tooltipHeight - 5
                : offsetYToParent + 5;

            this._tooltip
                .style('top', posY + 'px')
                .style('left', posX + 'px');
        }
    }

    _clearTooltip(): void {
        if(this._tooltip) {
            this._tooltip.style('display', 'none');
        }
        if(this._tooltipLine) {
            this._tooltipLine.attr('stroke', 'none');
        }
    }

    _findClosestDataPoints = (
        lineIndex: number, time: number
    ): [LineChartLineDataPoint?, LineChartLineDataPoint?, LineChartLineDataPoint?] => {
        const data = this._lines[lineIndex].data;

        if(data.length === 0) {
            return [undefined, undefined, undefined];
        }

        let previous, current, next;

        const firstAfterIndex = data.findIndex(dataPoint => new Date(dataPoint.time).getTime() >= time);
        if(firstAfterIndex === -1) {
            previous = data.length > 1 ? data[data.length - 1] : undefined;
            current = data[data.length - 1];
            next = undefined;
        } else if(firstAfterIndex === 0) {
            previous = undefined;
            current = new Date(data[0].time).getTime() > time ? undefined : data[0];
            next = data.length > 1 ? data[1] : undefined;
        } else {
            const before = data[firstAfterIndex - 1];
            const after = data[firstAfterIndex];
            const beforeTime = new Date(before.time).getTime();
            const afterTime = new Date(after.time).getTime();
            if(time - beforeTime <= afterTime - time) {
                previous = firstAfterIndex >= 2 ? data[firstAfterIndex - 2] : undefined;
                current = before;
                next = data[firstAfterIndex];
            } else {
                previous = before;
                current = after;
                next = data.length > firstAfterIndex + 1 ? data[firstAfterIndex + 1] : undefined;
            }
        }

        return [previous, current, next];
    }
}