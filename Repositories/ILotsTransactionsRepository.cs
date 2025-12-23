using System.Collections.Generic;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface ILotsTransactionsRepository
    {
        Task<List<LotsTransactions>> GetLotsTransactions();
        Task<LotsTransactions?> GetLotsTransactionById(int id);
        Task<List<LotsTransactions>> GetLotsTransactionsByIdTransaction(string idTransaction);
        Task<int> AddLotsTransaction(LotsTransactions lotsTransaction);
        Task<bool> RemoveLotsTransactionByTransactionId(string idTransaction);
    }
}
