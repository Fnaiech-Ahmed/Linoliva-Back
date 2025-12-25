using tech_software_engineer_consultant_int_backend.DTO.CommandesDTOs;
using tech_software_engineer_consultant_int_backend.DTO.TransactionsDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface ICommandeService
    {
        Task<bool> AddCommande(CommandeCreateDTO commandeCreateDTO, List<TransactionCreateDto> ListTransactionsCreateDTOs);
        Task<string> GenerateNextRef();



        Task<List<CommandeDTO>> GetCommandes();
        Task<List<CommandeDTO>> GetListeCommandeByOwner(string OwnerId);

        Task<CommandeDTO?> GetCommandeById(int id);
        Task<CommandeDTO?> GetCommandeByRef(string RefCommande);
        
        
        
        
        Task<(decimal, decimal)> GetTotalCommandeById(int id);
        Task<decimal> CalculerMontantTotalTTC(Commande commande);
        Task<decimal> CalculerMontantTotalHT(Commande commande);


        Task<(bool, string)> UpdateCommande(int commandeId, CommandeUpdateDTO commandeUpdateDTO, List<Transactions> NewListTransactions);

        bool existanceByIdTransactionCommande(Transactions transaction, List<Transactions> NewListTransactions);



        Task<(bool, string)> DeleteCommande(int id);
    }
}
