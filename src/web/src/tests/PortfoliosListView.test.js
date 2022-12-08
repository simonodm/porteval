import React from 'react';
import { fireEvent, findByRole, screen, waitFor, findAllByRole, findAllByTestId } from '@testing-library/react';
import PortfolioListView from '../components/views/PortfolioListView';
import { renderWithProviders } from './utils';
import { testInstruments, testPortfolios, testPositions, testTransactions } from './mocks/testData';
import { getPriceString } from '../utils/string';

const expandPortfolios = async () => {
    const portfoliosTable = await screen.findByLabelText("Portfolios table");
    const portfolioExpanders = await findAllByTestId(portfoliosTable, "expander");
    portfolioExpanders.forEach(expander => fireEvent.click(expander));
}

const expandPositions = async () => {
    await expandPortfolios();

    const positionsTables = await screen.findAllByLabelText(/Portfolio .* positions table/i);
    positionsTables.forEach(async table => {
        const expanders = await findAllByTestId(table, "expander");
        expanders.forEach(expander => fireEvent.click(expander));
    });
}

describe('Portfolio list view', () => {
    test("renders portfolios", async () => {
        renderWithProviders(<PortfolioListView />);

        const firstPortfolioName = await screen.findByRole("cell", { name: testPortfolios[0].name });
        const secondPortfolioName = await screen.findByRole("cell", { name: testPortfolios[1].name });

        expect(firstPortfolioName).toBeVisible();
        expect(secondPortfolioName).toBeVisible();
    });

    test("renders portfolio positions when expanded", async () => {
        renderWithProviders(<PortfolioListView />);

        await expandPortfolios();

        const positionTables = await screen.findAllByLabelText(/Portfolio .* positions table/i);
        expect(positionTables).toHaveLength(2);
    });

    test("renders position transactions when expanded", async () => {
        renderWithProviders(<PortfolioListView />);

        await expandPositions();

        testTransactions.forEach(async expectedTransaction => {
            await screen.findByText(expectedTransaction.amount);
            await screen.findAllByText(getPriceString(expectedTransaction.price, testPositions[0].instrument.currencyCode));
            await screen.findByText(expectedTransaction.note);
        });        
    });

    test("renders create portfolio button", async () => {
        renderWithProviders(<PortfolioListView />);

        screen.getByRole("button", { name: /create.*portfolio/i });
    });

    test("create portfolio button opens create portfolio form", async () => {
        renderWithProviders(<PortfolioListView />);

        const button = screen.getByRole("button", { name: /create.*portfolio/i });
        fireEvent.click(button);

        await screen.findByRole("heading", { name: /create.*portfolio/i });
        await screen.findByLabelText("Create portfolio form");
    });

    test("renders expand all button", async () => {
        renderWithProviders(<PortfolioListView />);

        screen.getByRole("button", { name: /expand.*all/i });
    });

    test("renders collapse all button", async () => {
        renderWithProviders(<PortfolioListView />);

        screen.getByRole("button", { name: /expand.*all/i });
    });

    test("expand all button expands all rows", async () => {
        renderWithProviders(<PortfolioListView />);

        await waitFor(() => {
            const portfolioRows = screen.getAllByTestId("datarow");
            expect(portfolioRows).toHaveLength(2);
        });

        const button = screen.getByRole("button", { name: /expand.*all/i });
        fireEvent.click(button);

        await waitFor(() => {
            const rows = screen.getAllByTestId("datarow");      
            expect(rows).toHaveLength(6);
            rows.forEach(row => expect(row).toBeVisible());            
        });        
    });

    test("collapse all button collapses all rows", async () => {
        renderWithProviders(<PortfolioListView />);

        const expandButton = screen.getByRole("button", { name: /expand.*all/i });
        const collapseButton = screen.getByRole("button", { name: /collapse.*all/i });

        fireEvent.click(expandButton);          
        fireEvent.click(collapseButton);

        const visibleRows = await screen.findAllByTestId("datarow", { hidden: false });

        expect(visibleRows).toHaveLength(2);
    });

    test("renders open position button for each portfolio", async () => {
        renderWithProviders(<PortfolioListView />);

        const openPositionButtons = await screen.findAllByRole("button", { name: /open.*position/i });

        expect(openPositionButtons).toHaveLength(2);
    });

    test("open position button opens open position form", async () => {
        renderWithProviders(<PortfolioListView />);

        const openPositionButtons = await screen.findAllByRole("button", { name: /open.*position/i });
        fireEvent.click(openPositionButtons[0]);

        await screen.findByRole("heading", { name: /open.*position/i });
        await screen.findByLabelText("Open position form");
    });

    test("renders add transaction button for each position", async () => {
        renderWithProviders(<PortfolioListView/>);

        await expandPortfolios();

        await waitFor(() => {
            const addTransactionButtons = screen.getAllByRole("button", { name: /add.*transaction/i });
            expect(addTransactionButtons).toHaveLength(2);
        });
    });

    test("renders edit portfolio button for each portfolio", async () => {
        renderWithProviders(<PortfolioListView/>);

        await waitFor(() => {
            const editPortfolioButtons = screen.getAllByRole("button", { name: /edit/i });
            expect(editPortfolioButtons).toHaveLength(2);
        });
    });

    test("renders edit button for each position", async () => {
        renderWithProviders(<PortfolioListView/>);

        await expandPortfolios();

        const firstPortfolioPositionsTable = await screen.findByLabelText(`Portfolio ${testPortfolios[0].id} positions table`);
        const secondPortfolioPositionsTable = await screen.findByLabelText(`Portfolio ${testPortfolios[1].id} positions table`);

        await findByRole(firstPortfolioPositionsTable, "button", { name: /edit/i });
        await findByRole(secondPortfolioPositionsTable, "button", { name: /edit/i });
    })

    test("renders chart button for each portfolio", async () => {
        renderWithProviders(<PortfolioListView/>);

        await waitFor(() => {
            const chartButtons = screen.getAllByRole("button", { name: /chart/i });
            expect(chartButtons).toHaveLength(2);
        });
    });

    test("renders remove button for each portfolio", async () => {
        renderWithProviders(<PortfolioListView/>);

        await waitFor(() => {
            const chartButtons = screen.getAllByRole("button", { name: /remove/i });
            expect(chartButtons).toHaveLength(2);
        });
    })

    test("renders chart button for each position", async () => {
        renderWithProviders(<PortfolioListView/>);

        await expandPortfolios();

        const firstPortfolioPositionsTable = await screen.findByLabelText(`Portfolio ${testPortfolios[0].id} positions table`);
        const secondPortfolioPositionsTable = await screen.findByLabelText(`Portfolio ${testPortfolios[1].id} positions table`);

        await findByRole(firstPortfolioPositionsTable, "button", { name: /chart/i });
        await findByRole(secondPortfolioPositionsTable, "button", { name: /chart/i });
    });

    test("renders remove button for each position", async () => {
        renderWithProviders(<PortfolioListView/>);

        await expandPortfolios();

        const firstPortfolioPositionsTable = await screen.findByLabelText(`Portfolio ${testPortfolios[0].id} positions table`);
        const secondPortfolioPositionsTable = await screen.findByLabelText(`Portfolio ${testPortfolios[1].id} positions table`);

        await findByRole(firstPortfolioPositionsTable, "button", { name: /remove/i });
        await findByRole(secondPortfolioPositionsTable, "button", { name: /remove/i });
    });

    test("renders remove button for each transaction", async () => {
        renderWithProviders(<PortfolioListView />);

        await expandPositions();

        const firstPositionTransactionsTable = await screen.findByLabelText(`Position ${testPositions[0].id} transactions table`);
        const secondPositionTransactionsTable = await screen.findByLabelText(`Position ${testPositions[1].id} transactions table`);

        await findByRole(firstPositionTransactionsTable, "button", { name: /remove/i });
        await findByRole(secondPositionTransactionsTable, "button", { name: /remove/i });
    });
})