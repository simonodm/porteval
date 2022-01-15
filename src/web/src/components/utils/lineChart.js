import * as d3 from 'd3';

export default function createChart() {
    return new D3LineChart();
}

class D3LineChart {
    _data = null;
    _settings = null;
    _svg = null;

    _xKey = 0
    _xScale = null;
    _xParser = d => d;
    _xFormat = d => d;
    _yKey = 1
    _yScale = null;
    _yParser = d => d;
    _yFormat = d => d;
    _xInterval = d3.timeDay.every(1);

    _width = 0;
    _height = 0;

    _lines = [];
    _container = null;
    
    _tooltip = null;
    _tooltipLine = null;

    _margins = { top: 0, left: 0, bottom: 0, right: 0 };
    _fontSize = 10;

    withData(data) {
        this._data = data;
        return this;
    }

    withXKey(xKey) {
        this._xKey = xKey;
        return this;
    }

    withYKey(yKey) {
        this._yKey = yKey;
        return this;
    }

    withXParser(xParser) {
        this._xParser = xParser;
        return this;
    }

    withYParser(yParser) {
        this._yParser = yParser;
        return this;
    }

    withXFormat(xFormat) {
        this._xFormat = xFormat;
        return this;
    }

    withYFormat(yFormat) {
        this._yFormat = yFormat;
        return this;
    }

    withLineSettings(settings) {
        this._settings = settings;
        return this;
    }

    withInterval(intervalFunction) {
        this._xInterval = intervalFunction;
        return this;
    }

    appendTo(container) {
        this._generateMargins();
        this._container = container;
        this._width = container.offsetWidth - this._margins.left - this._margins.right;
        this._height = container.offsetHeight -  this._margins.top - this._margins.bottom;
        this._svg = d3.select(container)
            .append('svg')
            .attr('transform', `translate(${this._margins.left}, ${this._margins.top})`)
            .attr('viewbox', [0, 0, this._width, this._height])
            .style('width', this._width)
            .style('height', this._height)
            .style('overflow', 'visible')
            .style('font-size', this._fontSize)
            .append('g');
        this._generateScales();
        this._generateLines();
        this._drawGridLines();
        this._drawDataLines();
        this._drawAxes();
        this._setupTooltip();
        return this;
    }

    _generateMargins() {
        // finds the longest value string among all data points in all lines
        const maxValueLength = this._data
            .map(
                lineData => 
                    lineData
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

    _getAllValuesFromKey(dataPointKey, callbackFn) {
        let result = [];
        this._data.forEach(dataLine => {
            dataLine.forEach(dataPoint => {
                result.push(callbackFn(dataPoint[dataPointKey]));
            });
        });
        return result;
    }

    _generateScales() {
        this._xScale = d3.scaleTime()
            .domain(d3.extent(this._getAllValuesFromKey(this._xKey, this._xParser)))
            .range([0, this._width]);
        this._yScale = d3.scaleLinear()
            .domain(d3.extent(this._getAllValuesFromKey(this._yKey, this._yParser)))
            .range([this._height, 0]);
    }

    _generateLines() {
        if(this._xScale && this._yScale) {
            this._data.forEach(() => {
                let d3Line = d3.line()
                    .x(d => this._xScale(this._xParser(d[this._xKey])))
                    .y(d => this._yScale(this._yParser(d[this._yKey])))
                this._lines = this._lines.concat(d3Line);
            });
        }
    }

    _drawAxes() {
        this._svg.append('g')
            .attr('class', 'x-axis')
            .attr('transform', `translate(0, ${this._height})`)
            .style('font-size', this._fontSize)
            .call(
                d3.axisBottom(this._xScale)
                    .ticks(this._xInterval ? Math.min(this._xInterval(), this._width / CHART_TICK_WIDTH) : this._width / CHART_TICK_WIDTH)
                    .tickSize(10)
                    .tickFormat(this._xFormat)
            )
        this._svg.append('g')
            .attr('class', 'y-axis')
            .style('font-size', this._fontSize)
            .call(
                d3.axisLeft(this._yScale)
                    .ticks(this._height / CHART_TICK_WIDTH)
                    .tickFormat(this._yFormat)
            )
    }

    _drawGridLines() {
        this._svg.append('g')
            .attr('class', 'grid')
            .attr('transform', `translate(0,${this._height})`)
            .call(
                d3.axisBottom(this._xScale)
                    .ticks(this._xInterval ? Math.min(this._xInterval(), this._width / CHART_TICK_WIDTH) : this._width / CHART_TICK_WIDTH)
                    .tickSize(-this._height)
                    .tickFormat('')
            )
            .call(
                g => g.selectAll('line')
                    .style('stroke', '#efefef')
                    .style('stroke-dasharray', '0')
            );
        this._svg.append('g')
            .attr('class', 'grid')
            .style('stroke-dasharray', '0')
            .call(
                d3.axisLeft(this._yScale)
                    .ticks(this._height / CHART_TICK_WIDTH)
                    .tickSize(-this._width)
                    .tickFormat('')
            ).call(
                g => g.selectAll('line')
                    .style('stroke', '#efefef')
                    .style('stroke-dasharray', '0')
            );
    }

    _drawDataLines() {
        this._lines.forEach((line, index) => {
            this._svg.append('path')
                .datum(this._data[index])
                .attr('fill', 'none')
                .style('stroke', this._settings[index].color)
                .style('stroke-width', this._settings[index].width)
                .style('stroke-dasharray', this._settings[index].strokeDashArray)
                .attr('class', 'line')
                .attr('d', line)
        });
    }

    _setupTooltip() {
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

        const findClosestDataPointInRange = (data, time) => {
            if(data.length === 0) {
                return null;
            }
            const firstAfterIndex = data.findIndex(dataPoint => this._xParser(dataPoint[this._xKey]).getTime() >= time);
            if(firstAfterIndex === -1) {
                return data[data.length - 1];
            }
            if(firstAfterIndex === 0) {
                return this._xParser(data[0][this._xKey]).getTime() > time ? null : data[0];
            }
            var before = data[firstAfterIndex - 1];
            var after = data[firstAfterIndex];
            var beforeTime = this._xParser(before[this._xKey]).getTime();
            var afterTime = this._xParser(after[this._xKey]).getTime();
            if(time - beforeTime <= afterTime - time) {
                return before;
            }
            return after;
        }

        const drawTooltip = (event) => {
            if(this._data.length > 0) {
                const longestData = this._data.reduce((prev, curr) => curr.length >= prev.length ? curr : prev, []);
                const time = Math.floor(this._xScale.invert(d3.pointer(event)[0]));
                const dataPoint = findClosestDataPointInRange(longestData, time);

                if(!dataPoint) return;

                const x = this._xParser(dataPoint[this._xKey]);

                const offsetX = event.offsetX + 45;
                const offsetY = event.offsetY + 45

                this._tooltipLine.attr('stroke', 'black')
                    .attr('x1', this._xScale(x))
                    .attr('x2', this._xScale(x))
                    .attr('y1', 0)
                    .attr('y2', this._height);
                this._tooltip.html(this._xFormat(this._xParser(dataPoint[this._xKey])))
                    .style('left', offsetX + 'px')
                    .style('top', offsetY + 'px')
                    .style('display', 'block')
                    .selectAll()
                    .data(this._data).enter()
                    .append('div')
                    .style('color', (d, i) => this._settings[i].color)
                    .html(d => {
                        const point = findClosestDataPointInRange(d, time);
                        return this._yFormat(this._yParser(point ? point[this._yKey] : 0));
                    });
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
}