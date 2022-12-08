import { parseISO } from "date-fns";
import { rest } from "msw";
import { Instrument, PaginatedResponse } from "../../types";
import { testCurrencies, testInstruments, testPortfolios, testPortfolioStats, testPositions, testPositionStats, testPrices, testTransactions } from "./testData";

const getCurrencies = () => {
    return rest.get('/api/currencies', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json(testCurrencies)
        );
    });
}

const getPortfolios = () => {
    return rest.get('/api/portfolios', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json(testPortfolios)
        );
    });
}

const getAllPortfoliosStats = () => {
    return rest.get('/api/portfolios/stats', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json(testPortfolioStats)
        );
    });
}

const getPortfolioPositions = () => {
    return rest.get('/api/portfolios/:id/positions', (req, res, ctx) => {
        const { id } = req.params;

        return res(
            ctx.status(200),
            ctx.json(testPositions.filter(p => p.portfolioId === parseInt(id as string)))
        );
    });
}

const getPortfolioPositionsStats = () => {
    return rest.get('/api/portfolios/:id/positions/stats', (req, res, ctx) => {
        const { id } = req.params;

        return res(
            ctx.status(200),
            ctx.json(testPositionStats.filter(p => testPositions.find(pos => pos.id == p.id)?.portfolioId === parseInt(id as string)))
        );
    })
}

const getPositionStats = () => {
    return rest.get('/api/positions/:id/stats', (req, res, ctx) => {
        const { id } = req.params;

        return res(
            ctx.status(200),
            ctx.json(testPositionStats.filter(p => p.id === parseInt(id as string)))
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
            ctx.json(testTransactions.filter(t =>
                (portfolioId === null || t.portfolioId === parseInt(portfolioId)) &&
                (positionId === null || t.positionId === parseInt(positionId)) &&
                (instrumentId === null || t.instrument.id === parseInt(instrumentId))
            ))
        );
    })
}

const getInstrumentsPage = () => {
    return rest.get('/api/instruments', (req, res, ctx) => {
        const pageParam = req.url.searchParams.get('page');
        const limitParam = req.url.searchParams.get('limit');

        const page = pageParam !== null ? parseInt(pageParam) : 1;
        const limit = limitParam !== null ? parseInt(limitParam) : 300;

        const data = testInstruments.slice((page - 1) * limit, page * limit + 1);

        return res(
            ctx.status(200),
            ctx.json<PaginatedResponse<Instrument>>({
                page,
                limit,
                count: data.length,
                totalCount: testInstruments.length,
                data 
            })
        );
    });
}

const getInstrument = () => {
    return rest.get('/api/instruments/:id', (req, res, ctx) => {
        const { id } = req.params;

        return res(
            ctx.status(200),
            ctx.json(testInstruments.find(i => i.id == parseInt(id as string)))
        );
    })
}

const getInstrumentPriceAt = () => {
    return rest.get('/api/instruments/:id/prices/at', (req, res, ctx) => {
        const { id } = req.params;
        const timeParam = req.url.searchParams.get('time');

        const time = timeParam === null ? Date.now() : parseISO(timeParam)

        return res(
            ctx.status(200),
            ctx.json(testPrices.find(p => p.instrumentId === parseInt(id as string) && parseISO(p.time) <= time))
        );
    })
}

export const handlers = [
    getCurrencies(),
    getPortfolios(),
    getAllPortfoliosStats(),
    getPortfolioPositions(),
    getPortfolioPositionsStats(),
    getPositionStats(),
    getTransactions(),
    getInstrumentsPage(),
    getInstrument(),
    getInstrumentPriceAt()
];