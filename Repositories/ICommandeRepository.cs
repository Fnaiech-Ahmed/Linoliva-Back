using System.Collections.Generic;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface ICommandeRepository
    {
        Task<bool> AddCommande(Commande commande);
        Task<(bool IsSuccess, string Message)> UpdateCommande(Commande commande);



        Task<Commande?> GetCommandeById(int commandeId);
        Task<Commande?> GetCommandeByRef(string referenceCommande);



        Task<List<Commande>> GetAllCommandes();
        Task<List<Commande>> GetListeCommandesByOwner(string OwnerId);


        Task<bool> DeleteCommande(int commandeId);
    }
}
