using tech_software_engineer_consultant_int_backend.DTO.TransactionsDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface ITransactionService
    {
        Task<(bool, int)> AddTransaction(TransactionCreateDto transactionCreateDto, string ReferenceCommande);
        Task<List<TransactionsDTO>> GetTransactionsByRefCommande(string ReferenceCommande);
        Task<TransactionsDTO> GetTransactionById(int id);
        Task<List<TransactionsDTO>> GetTransactions();


        Task<bool> UpdateRefLotTransaction(int transactionId, Transactions transaction);
        Task<(bool, int)> UpdateTransaction(int transactionId, TransactionsUpdateDTO transactionUpdateDTO);

        Task<bool> DeleteTransaction(int id);
    }
}
