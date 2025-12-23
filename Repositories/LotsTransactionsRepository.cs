using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class LotsTransactionsRepository : ILotsTransactionsRepository
    {
        private readonly MyDbContext _dbContext;

        public LotsTransactionsRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<LotsTransactions>> GetLotsTransactions()
        {
            return await _dbContext.LotsTransactions.ToListAsync();
        }

        public async Task<LotsTransactions?> GetLotsTransactionById(int id)
        {
            return await _dbContext.LotsTransactions.FindAsync(id);
        }

        public async Task<List<LotsTransactions>> GetLotsTransactionsByIdTransaction(string idTransaction)
        {
            return await _dbContext.LotsTransactions
                                   .Where(lt => lt.IdTransaction == idTransaction)
                                   .ToListAsync();
        }

        public async Task<int> AddLotsTransaction(LotsTransactions lotsTransaction)
        {
            _dbContext.LotsTransactions.Add(lotsTransaction);
            await _dbContext.SaveChangesAsync();
            return lotsTransaction.Id;
        }

        public async Task<bool> RemoveLotsTransactionByTransactionId(string idTransaction)
        {
            var lotsTransactions = await _dbContext.LotsTransactions
                                                   .Where(lt => lt.IdTransaction == idTransaction)
                                                   .ToListAsync();

            if (lotsTransactions.Count == 0)
            {
                return false;
            }

            _dbContext.LotsTransactions.RemoveRange(lotsTransactions);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
