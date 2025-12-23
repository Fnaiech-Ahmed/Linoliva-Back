using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.DTO.LotsTransactionsDTOs;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class LotsTransactionsService : ILotsTransactionsService
    {
        private readonly ILotsTransactionsRepository _LTRepository;

        public LotsTransactionsService(ILotsTransactionsRepository lotsTransactionsRepository)
        {
            _LTRepository = lotsTransactionsRepository;
        }

        public async Task<bool> AddLT(LotsTransactionsDTO newLotsTransactionsDto)
        {
            var newLotsTransactions = newLotsTransactionsDto.ToEntity();
            await _LTRepository.AddLotsTransaction(newLotsTransactions);
            return true;
        }

        public async Task<List<LotsTransactionsDTO>> GetLotsTransactionsByIdTransaction(int idTransaction)
        {
            var lotsTransactions = await _LTRepository.GetLotsTransactionsByIdTransaction(idTransaction.ToString());
            return lotsTransactions.Select(LotsTransactionsDTO.FromEntity).ToList();
        }

        public async Task<bool> RemoveLotsTransactionByTransactionId(int transactionId)
        {
            return await _LTRepository.RemoveLotsTransactionByTransactionId(transactionId.ToString());
        }
    }
}
