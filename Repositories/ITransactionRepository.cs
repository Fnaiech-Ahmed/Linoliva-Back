using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<Transactions>> GetTransactions();
        Task<Transactions> GetTransactionById(int transactionId);
        Task<(bool, int)> AddTransaction(Transactions transaction);
        Task<List<Transactions>> GetTransactionsByRefCommande(string referenceCommande);
        void UpdateTransaction(Transactions transaction);
        Task<bool> UpdateTransactionRemainingQuantity(int productId, int newRemainingQuantity);
        Task<bool> UpdateRefLotTransaction(int transactionId, string ReferenceLot);
        Task<bool> DeleteTransaction(int transactionId);
    }
}
