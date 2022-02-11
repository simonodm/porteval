import * as d3 from 'd3';
import { CHART_TICK_INTERVAL } from '../../constants';

export type Line = {
    name: string;
    color: string;
    strokeDashArray: string;
    width: number;
    data: Array<LineData>;
}

export type LineData = {
    time: string;
    value: number;
}

export type RenderedDataPointInfo = {
    x: number;
    y: number;
    point: LineData;
    nextPoint: LineData | undefined;
    lineIndex: number;
}

export type TooltipCallback = (from: string | undefined, to: string | undefined) => HTMLElement | null;
export type RenderCallback = (dataPoint: RenderedDataPointInfo) => SVGElement;

export default function createChart(): D3LineChart {
    return new D3LineChart();
}

class D3LineChart {
    _lines: Array<Line> = [];
    _svg: d3.Selection<SVGGElement, unknown, null, undefined> | null = null;

    _xScale: d3.ScaleTime<number, number> | null = null;
    _xFormat: (x: Date) => string = d3.isoFormat;

    _yScale: d3.ScaleLinear<number, number> | null = null;
    _yFormat: (y: number) => string = (num) => `${num}`;

    _xInterval = d3.timeDay;

    _width = 0;
    _height = 0;

    _d3Lines: Array<d3.Line<LineData>> = [];
    _container: HTMLElement | null = null;
    
    _tooltip: d3.Selection<HTMLDivElement, unknown, null, undefined> | null = null;
    _tooltipLine: d3.Selection<SVGLineElement, unknown, null, undefined> | null = null;
    _tooltipCallback: TooltipCallback = () => null;

    _renderCallback: RenderCallback | null = null;

    _margins = { top: 0, left: 0, bottom: 0, right: 0 };
    _fontSize = 10;

    withLines(lines: typeof this._lines) {
        this._lines = lines;
        return this;
    }

    withXFormat(xFormat: typeof this._xFormat) {
        this._xFormat = xFormat;
        return this;
    }

    withYFormat(yFormat: typeof this._yFormat) {
        this._yFormat = yFormat;
        return this;
    }

    withInterval(intervalFunction: typeof this._xInterval) {
        this._xInterval = intervalFunction;
        return this;
    }

    withTooltipCallback(tooltipCallback: typeof this._tooltipCallback) {
        this._tooltipCallback = tooltipCallback;
        return this;
    }

    withAdditionalRenderCallback(renderCallback: typeof this._renderCallback) {
        this._renderCallback = renderCallback;
        return this;
    }

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
        this._margins.left = maxValueLength * this._fontSize / 1.5;
        this._margins.bottom = this._fontSize * 2.2;
        this._margins.right = 5;
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
                const d3Line = d3.line<LineData>()
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
                        ? Math.min(this._xInterval.count(this._xScale.domain()[0], this._xScale.domain()[1]), this._width / CHART_TICK_INTERVAL)
                        : this._width / CHART_TICK_INTERVAL)
                    .tickSize(10)
                    .tickFormat(this._xFormat)
            )
        this._svg?.append('g')
            .attr('class', 'y-axis')
            .style('font-size', this._fontSize)
            .call(
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
                        ? Math.min(this._xInterval.count(this._xScale.domain()[0], this._xScale.domain()[1]), this._width / CHART_TICK_INTERVAL)
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
                        const nextPoint = pointIndex < this._lines[index].data.length - 1 ? this._lines[index].data[pointIndex + 1] : undefined;

                        const pointInfo: RenderedDataPointInfo = {
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
        if(!this._container || !this._svg) return;
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
            .attr('opacity', 0)

        const findClosestDataPoints = (data: LineData[], time: number): [LineData?, LineData?, LineData?] => {
            if(data.length === 0) {
                return [undefined, undefined, undefined];
            }
            let previous, current, next;

            const firstAfterIndex = data.findIndex(dataPoint => new Date(dataPoint.time).getTime() >= time);
            if(firstAfterIndex === -1) {
                previous = data.length > 1 ? data[data.length - 1] : undefined;
                current = data[data.length - 1];
                next = undefined;
            }
            else if(firstAfterIndex === 0) {
                previous = undefined;
                current = new Date(data[0].time).getTime() > time ? undefined : data[0];
                next = data.length > 1 ? data[1] : undefined;
            }
            else {
                const before = data[firstAfterIndex - 1];
                const after = data[firstAfterIndex];
                const beforeTime = new Date(before.time).getTime();
                const afterTime = new Date(after.time).getTime();
                if(time - beforeTime <= afterTime - time) {
                    previous = firstAfterIndex >= 2 ? data[firstAfterIndex - 2] : undefined;
                    current = before;
                    next = data[firstAfterIndex];
                }
                else {
                    previous = before;
                    current = after;
                    next = data.length > firstAfterIndex + 1 ? data[firstAfterIndex + 1] : undefined;
                }
            }
            
            return [previous, current, next];
        }

        const drawTooltip = (event: MouseEvent) => {
            if(!this._xScale || !this._container || !this._tooltip || !this._tooltipLine) return;
            if(this._lines.length > 0) {
                const longestLine = this._lines.reduce<Array<LineData>>((prev, curr) => curr.data.length >= prev.length ? curr.data : prev, []);
                const time = Math.floor(this._xScale.invert(d3.pointer(event)[0]).getTime());
                const [, currDataPoint, nextDataPoint] = findClosestDataPoints(longestLine, time);

                if(!currDataPoint) return;

                const x = new Date(currDataPoint.time);

                this._tooltipLine.attr('stroke', 'black')
                    .attr('x1', this._xScale(x))
                    .attr('x2', this._xScale(x))
                    .attr('y1', 0)
                    .attr('y2', this._height);
                this._tooltip.html(this._xFormat(new Date(currDataPoint.time)))
                    .style('display', 'block')
                    .selectAll()
                    .data(this._lines).enter()
                    .append('div')
                    .style('color', d => d.color)
                    .html(d => {
                        const [, point] = findClosestDataPoints(d.data, time);
                        return `${d.name}: ${this._yFormat(point ? point.value : 0)}`;
                    });

                if(this._tooltipCallback) {
                    const from = currDataPoint.time;
                    const to = nextDataPoint ? nextDataPoint.time : undefined;
                    const element = this._tooltipCallback(from, to);

                    if(element) {
                        this._tooltip.node()?.appendChild(element);
                    }
                }

                const tooltipWidth = this._tooltip.node()?.getBoundingClientRect()['width'] ?? 0;
                const tooltipHeight = this._tooltip.node()?.getBoundingClientRect()['height'] ?? 0;

                const [chartTopBound, chartLeftBound] = this._getContainerOffset();

                const offsetXToParent = event.clientX - chartLeftBound;
                const offsetYToParent = event.clientY - chartTopBound;

                const posX = offsetXToParent + tooltipWidth >= this._width ? offsetXToParent - tooltipWidth - 5 : offsetXToParent + 5;
                const posY = offsetYToParent + tooltipHeight >= this._height ? offsetYToParent - tooltipHeight - 5 : offsetYToParent + 5;

                this._tooltip
                    .style('top', posY + 'px')
                    .style('left', posX + 'px');
            }

        }

        const removeTooltip = () => {
            if(this._tooltip) {
                this._tooltip.style('display', 'none');
            }
            if(this._tooltipLine) {
                this._tooltipLine.attr('stroke', 'none');
            }
        }

        overlay.on('mousemove', drawTooltip);
        overlay.on('mouseout', removeTooltip);
    }

    _getContainerOffset(): [number, number] {
        if(!this._container) throw new Error('Chart container has not been initialized.');
        const containerRect = this._container.getBoundingClientRect();

        return [containerRect.top, containerRect.left];
    }
}