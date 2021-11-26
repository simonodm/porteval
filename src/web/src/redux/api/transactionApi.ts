import { Transaction } from '../../types';
import { CreateTransactionParameters } from './apiTypes';
import { portEvalApi } from './portEvalApi';

const transactionApi = portEvalApi.injectEndpoints({
    endpoints: (build) => ({
        getTransactions: build.query<Array<Transaction>, { portfolioId: number, positionId: number }>({
            query: ({ positionId }) =>
                `positions/${positionId}/transactions`,
            providesTags: (result, error, arg) =>
                result
                    ? [
                        ...result.map(({ id }) => ({ type: 'Transaction' as const, id: id })),
                        { type: 'Transactions', id: arg.positionId }
                      ]
                    : []
        }),
        getTransaction: build.query<Transaction, { positionId: number, transactionId: number }>({
            query: ({ positionId, transactionId }) =>
                `positions/${positionId}/transactions/${transactionId}`,
            providesTags: (result, error, arg) =>
                result
                    ? [{ type: 'Transaction', id: arg.transactionId }]
                    : []
        }),
        addTransaction: build.mutation<Transaction, CreateTransactionParameters>({
            query: (data) => ({
                url: `positions/${data.positionId}/transactions`,
                method: 'POST',
                body: data
            }),
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [
                        { type: 'Transactions', id: arg.positionId },
                        { type: 'PortfolioCalculations', id: arg.portfolioId },
                        { type: 'PositionCalculations', id: arg.positionId }
                      ]
                    : []
        }),
        updateTransaction: build.mutation<Transaction, Transaction>({
            query: (data) => ({
                url: `positions/${data.positionId}/transactions/${data.id}`,
                method: 'PUT',
                body: data
            }),
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [{ type: 'Transaction', id: arg.id }]
                    : []
        }),
        deleteTransaction: build.mutation<void, Transaction>({
            query: ({ positionId, id }) => ({
                url: `positions/${positionId}/transactions/${id}`,
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
    useGetTransactionsQuery,
    useGetTransactionQuery,
    useAddTransactionMutation,
    useUpdateTransactionMutation,
    useDeleteTransactionMutation
} = transactionApi