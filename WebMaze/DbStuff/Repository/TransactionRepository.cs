using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebMaze.DbStuff.Model.UserAccount;

namespace WebMaze.DbStuff.Repository
{
    public class TransactionRepository : AsyncBaseRepository<Transaction>
    {
        public TransactionRepository(WebMazeContext context) : base(context)
        {
        }

        public IQueryable<Transaction> GetUserTransactionsAsync(string userLogin)
        {
            return DbSet.Where(transaction =>
               transaction.Sender.Login == userLogin || transaction.Recipient.Login == userLogin);
        }
    }
}
