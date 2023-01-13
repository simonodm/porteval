import { parseISO } from 'date-fns';
import { rest } from 'msw';
import { Instrument, InstrumentPrice, InstrumentSplit, PaginatedResponse } from '../../types';
import { TestState, DEFAULT_TEST_STATE } from './testData';

let currentState: TestState = JSON.parse(JSON.stringify(DEFAULT_TEST_STATE));

export const resetState = () => {
    currentState = JSON.parse(JSON.stringify(DEFAULT_TEST_STATE));
}

const getCurrencies = () => {
    return rest.get('/api/currencies', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json(currentState.currencies)
        );
    });
}

const getPortfolios = () => {
    return rest.get('/api/portfolios', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json(currentState.portfolios)
        );
    });
}

const getPortfolio = () => {
    return rest.get('/api/portfolios/:id', (req, res, ctx) => {
        const { id } = req.params;

        return res(
            ctx.status(200),
            ctx.json(currentState.portfolios.find(p => p.id === parseInt(id as string)))
        );
    })
}

const deletePortfolio = () => {
    return rest.delete('/api/portfolios/:id', (req, res, ctx) => {
        const { id } = req.params;

        currentState.portfolios = currentState.portfolios.filter(p => p.id !== parseInt(id as string));

        return res(
            ctx.status(200)
        );
    })
}

const getAllPortfoliosStats = () => {
    return rest.get('/api/portfolios/stats', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json(currentState.portfolioStatistics)
        );
    });
}

const getPortfolioStats = () => {
    return rest.get('/api/portfolios/:id/stats', (req, res, ctx) => {
        const { id } = req.params;

        return res(
            ctx.status(200),
            ctx.json(currentState.portfolioStatistics.find(s => s.id === parseInt(id as string)))
        );
    })
}

const getPortfolioValue = () => {
    return rest.get('/api/portfolios/:id/value', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json({
                value: 111,
                currencyCode: 'USD',
                time: '2022-01-01T00:00:00Z'
            })
        )
    });
}

const getPortfolioPriceChart = () => {
    return rest.get('/api/portfolios/:id/value/chart', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json([{
                value: 0,
                time: '2022-01-01'
            }])
        );
    })
}

const getPortfolioPositions = () => {
    return rest.get('/api/portfolios/:id/positions', (req, res, ctx) => {
        const { id } = req.params;

        return res(
            ctx.status(200),
            ctx.json(currentState.positions.filter(p => p.portfolioId === parseInt(id as string)))
        );
    });
}

const getPortfolioPositionsStats = () => {
    return rest.get('/api/portfolios/:id/positions/stats', (req, res, ctx) => {
        const { id } = req.params;

        return res(
            ctx.status(200),
            ctx.json(
                currentState.positionStatistics
                    .filter(p =>
                        currentState.positions.find(pos => pos.id === p.id)?.portfolioId === parseInt(id as string)))
        );
    })
}

const deletePosition = () => {
    return rest.delete('/api/positions/:id', (req, res, ctx) => {
        const { id } = req.params;

        currentState.positions = currentState.positions.filter(p => p.id !== parseInt(id as string));

        return res(
            ctx.status(200)
        );
    })
}

const getPositionStats = () => {
    return rest.get('/api/positions/:id/stats', (req, res, ctx) => {
        const { id } = req.params;

        return res(
            ctx.status(200),
            ctx.json(currentState.positionStatistics.filter(p => p.id === parseInt(id as string)))
        );
    });
}

const getTransactions = () => {
    return rest.get('/api/transactions', (req, res, ctx) => {
        const portfolioId = req.url.searchParams.get('portfolioId');
        const positionId = req.url.searchParams.get('positionId');
        const instrumentId = req.url.searchParams.get('instrumentId');

        return res(
            ctx.status(200),
            ctx.json(currentState.transactions.filter(t =>
                (portfolioId === null || t.portfolioId === parseInt(portfolioId)) &&
                (positionId === null || t.positionId === parseInt(positionId)) &&
                (instrumentId === null || t.instrument.id === parseInt(instrumentId))
            ))
        );
    })
}

const deleteTransaction = () => {
    return rest.delete('/api/transactions/:id', (req, res, ctx) => {
        const { id } = req.params;

        currentState.transactions = currentState.transactions.filter(t => t.id !== parseInt(id as string));

        return res(
            ctx.status(200)
        );
    })
}

const getInstrumentsPage = () => {
    return rest.get('/api/instruments', (req, res, ctx) => {
        const pageParam = req.url.searchParams.get('page');
        const limitParam = req.url.searchParams.get('limit');

        const page = pageParam !== null ? parseInt(pageParam) : 1;
        const limit = limitParam !== null ? parseInt(limitParam) : 300;

        const data = currentState.instruments.slice((page - 1) * limit, page * limit + 1);

        return res(
            ctx.status(200),
            ctx.json<PaginatedResponse<Instrument>>({
                page,
                limit,
                count: data.length,
                totalCount: currentState.instruments.length,
                data 
            })
        );
    });
}

const getInstrumentPrices = () => {
    return rest.get('/api/instruments/:id/prices', (req, res, ctx) => {
        const { id } = req.params;

        const pageParam = req.url.searchParams.get('page');
        const limitParam = req.url.searchParams.get('limit');

        const page = pageParam !== null ? parseInt(pageParam) : 1;
        const limit = limitParam !== null ? parseInt(limitParam) : 300;

        const data = currentState.instrumentPrices
            .filter(p => p.instrumentId === parseInt(id as string)).slice((page - 1) * limit, page * limit + 1);

        return res(
            ctx.status(200),
            ctx.json<PaginatedResponse<InstrumentPrice>>({
                page,
                limit,
                count: data.length,
                totalCount: currentState.instruments.length,
                data 
            })
        );
    })
}

const getInstrument = () => {
    return rest.get('/api/instruments/:id', (req, res, ctx) => {
        const { id } = req.params;

        return res(
            ctx.status(200),
            ctx.json(currentState.instruments.find(i => i.id === parseInt(id as string)))
        );
    })
}

const getInstrumentPriceChart = () => {
    return rest.get('/api/instruments/:id/prices/chart', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json([])
        );
    });
}

const getInstrumentPriceAt = () => {
    return rest.get('/api/instruments/:id/prices/at', (req, res, ctx) => {
        const { id } = req.params;
        const timeParam = req.url.searchParams.get('time');

        const time = timeParam === null ? Date.now() : parseISO(timeParam)

        return res(
            ctx.status(200),
            ctx.json(
                currentState.instrumentPrices
                    .find(p => p.instrumentId === parseInt(id as string) && parseISO(p.time) <= time)
            )
        );
    })
}

const deleteInstrumentPrice = () => {
    return rest.delete('/api/instruments/:instrumentId/prices/:priceId', (req, res, ctx) => {
        const { instrumentId, priceId } = req.params;

        currentState.instrumentPrices = currentState.instrumentPrices
            .filter(p => p.instrumentId !== parseInt(instrumentId as string) && p.id !== parseInt(priceId as string));

        return res(
            ctx.status(200)
        );
    })
}

const getInstrumentSplits = () => {
    return rest.get('/api/instruments/:id/splits', (req, res, ctx) => {
        const { id } = req.params;

        const data = currentState.instrumentSplits.filter(s => s.instrumentId === parseInt(id as string));
        return res(
            ctx.status(200),
            ctx.json(data)
        );
    });
}

const deleteInstrument = () => {
    return rest.delete('/api/instruments/:id', (req, res, ctx) => {
        const { id } = req.params;

        currentState.instruments = currentState.instruments.filter(i => i.id !== parseInt(id as string));

        return res(
            ctx.status(200)
        );
    });
}

const putInstrumentSplit = () => {
    return rest.put('/api/instruments/:instrumentId/splits/:splitId', async (req, res, ctx) => {
        const { splitId } = req.params;

        const split = await req.json() as InstrumentSplit;

        if(split.status === 'rollbackRequested') {
            const existingSplit = currentState.instrumentSplits.find(s => s.id === parseInt(splitId as string));
            if(existingSplit) {
                existingSplit.status = 'rollbackRequested';
            }
        }

        return res(
            ctx.status(200),
            ctx.json(split)
        );
    });
}

const getCurrency = () => {
    return rest.get('/api/currencies/:code', (req, res, ctx) => {
        const { code } = req.params;

        return res(
            ctx.status(200),
            ctx.json(currentState.currencies.find(p => p.code === code))
        );
    })
}

const getLatestCurrencyExchangeRates = () => {
    return rest.get('/api/currencies/:code/exchange_rates/latest', (req, res, ctx) => {
        const { code } = req.params;

        return res(
            ctx.status(200),
            ctx.json(currentState.exchangeRates.filter(er => er.currencyFromCode === code))
        );
    })
}

const getCharts = () => {
    return rest.get('/api/charts', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json(currentState.charts)
        );
    })
}

const deleteChart = () => {
    return rest.delete('/api/charts/:id', (req, res, ctx) => {
        const { id } = req.params;

        currentState.charts = currentState.charts.filter(c => c.id !== parseInt(id as string));
        
        return res(
            ctx.status(200)
        );
    })
}

const getDataImports = () => {
    return rest.get('/api/imports', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json(currentState.dataImports)
        )
    });
}

const getDashboardLayout = () => {
    return rest.get('/api/dashboard', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json(currentState.dashboardLayout)
        )
    });
}

const getExchanges = () => {
    return rest.get('/api/exchanges', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json(currentState.exchanges)
        );
    });
}

export const handlers = [
    getCurrencies(),
    getPortfolios(),
    getAllPortfoliosStats(),
    getPortfolio(),
    getPortfolioStats(),
    getPortfolioValue(),
    getPortfolioPriceChart(),
    getPortfolioPositions(),
    getPortfolioPositionsStats(),
    getPositionStats(),
    deletePortfolio(),
    deletePosition(),
    getTransactions(),
    deleteTransaction(),
    getInstrumentsPage(),
    getInstrument(),
    deleteInstrument(),
    getInstrumentPriceChart(),
    getInstrumentPriceAt(),
    getInstrumentPrices(),
    getInstrumentSplits(),
    putInstrumentSplit(),
    deleteInstrumentPrice(),
    getCurrency(),
    getLatestCurrencyExchangeRates(),
    getCharts(),
    getDataImports(),
    getDashboardLayout(),
    deleteChart(),
    getExchanges()
];