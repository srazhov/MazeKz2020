using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebMaze.DbStuff.Model.UserAccount;
using WebMaze.DbStuff.Repository;

namespace WebMaze.Services
{
    public class TransactionService
    {
        private TransactionRepository transactionRepository;

        public TransactionService(TransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository;
        }

        public async Task<List<Transaction>> GetTransactionsAsync()
        {
            return await transactionRepository.GetAllAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(long transactionId)
        {
            return await transactionRepository.GetByIdAsync(transactionId);
        }

        public async Task<List<Transaction>> GetUserTransactionsAsync(string userLogin)
        {
            return await transactionRepository.GetUserTransactionsAsync(userLogin).ToListAsync();
        }

        public async Task<OperationResult> CreateTransaction(Transaction transaction)
        {
            if (transaction.Sender.Balance < transaction.Amount)
            {
                return OperationResult.Failed("Insufficient account balance.");
            }

            transaction.Sender.Balance -= transaction.Amount;
            transaction.Recipient.Balance += transaction.Amount;

            await transactionRepository.SaveAsync(transaction);

            return OperationResult.Success();
        }

        public async Task UpdateTransaction(Transaction transaction)
        {
            await transactionRepository.SaveAsync(transaction);
        }
        public async Task<bool> TransactionExistsAsync(int id)
        {
            return await transactionRepository.Exists(id);
        }

        public async Task DeleteTransactionAsync(int id)
        {
            await transactionRepository.DeleteAsync(id);
        }
    }
}
