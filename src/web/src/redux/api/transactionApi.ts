import { Transaction } from '../../types';
import { CreateTransactionParameters } from './apiTypes';
import { truncateEntityNote } from './apiUtils';
import { portEvalApi } from './portEvalApi';

const transactionApi = portEvalApi.injectEndpoints({
    endpoints: (build) => ({
        getPositionTransactions: build.query<Array<Transaction>, { positionId: number }>({
            query: ({ positionId }) =>
                `transactions?positionId=${positionId}`,
            providesTags: (result) =>
                result
                    ? [
                        ...result.map(({ id }) => ({ type: 'Transaction' as const, id: id })),
                        { type: 'Transactions' }
                      ]
                    : []
        }),
        getPortfolioTransactions: build.query<Array<Transaction>, { portfolioId: number }>({
            query: ({ portfolioId }) =>
                `transactions?portfolioId=${portfolioId}`,
            providesTags: (result) =>
                result
                    ? [
                        ...result.map(({ id }) => ({ type: 'Transaction' as const, id: id })),
                        { type: 'Transactions' }
                      ]
                    : []
        }),
        getInstrumentTransactions: build.query<Array<Transaction>, { instrumentId: number }>({
            query: ({ instrumentId }) =>
                `transactions?instrumentId=${instrumentId}`,
            providesTags: (result) =>
                result
                    ? [
                        ...result.map(({ id }) => ({ type: 'Transaction' as const, id: id })),
                        { type: 'Transactions' }
                      ]
                    : []
        }),
        getTransaction: build.query<Transaction, { transactionId: number }>({
            query: ({ transactionId }) =>
                `transactions/${transactionId}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'Transaction', id: arg.transactionId }]
                    : []
        }),
        addTransaction: build.mutation<Transaction, CreateTransactionParameters>({
            query: (data) => ({
                url: 'transactions',
                method: 'POST',
                body: truncateEntityNote(data)
            }),
            invalidatesTags: (result, error, arg) =>
                !error && result
                    ? [
                        { type: 'Transactions', id: arg.positionId },
                        { type: 'PortfolioCalculations', id: result.portfolioId },
                        { type: 'PositionCalculations', id: arg.positionId }
                      ]
                    : []
        }),
        updateTransaction: build.mutation<Transaction, Transaction>({
            query: (data) => ({
                url: `transactions/${data.id}`,
                method: 'PUT',
                body: truncateEntityNote(data)
            }),
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [{ type: 'Transaction', id: arg.id }]
                    : []
        }),
        deleteTransaction: build.mutation<void, Transaction>({
            query: ({ id }) => ({
                url: `transactions/${id}`,
                method: 'DELETE'
            }),
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [
                        { type: 'Transactions', id: arg.positionId }, 
                        { type: 'Transaction', id: arg.id },
                        { type: 'PortfolioCalculations', id: arg.portfolioId },
                        { type: 'PositionCalculations', id: arg.positionId }
                      ]
                    : []
        })
    })
});

export const {
    useGetPositionTransactionsQuery,
    useGetPortfolioTransactionsQuery,
    useGetInstrumentTransactionsQuery,
    useGetTransactionQuery,
    useAddTransactionMutation,
    useUpdateTransactionMutation,
    useDeleteTransactionMutation
} = transactionApi