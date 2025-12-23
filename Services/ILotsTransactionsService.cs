using System.Collections.Generic;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.DTO.LotsTransactionsDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface ILotsTransactionsService
    {
        Task<bool> AddLT(LotsTransactionsDTO newLotsTransactionsDto);
        Task<List<LotsTransactionsDTO>> GetLotsTransactionsByIdTransaction(int idTransaction);
        Task<bool> RemoveLotsTransactionByTransactionId(int transactionId);
    }
}
