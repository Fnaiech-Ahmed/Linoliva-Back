using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface IBLRepository
    {
        Task<List<BonDeLivraison>> GetBonDeLivraisons();
        Task<List<BonDeLivraison>> GetListeBonsDeLivraisonByOwner(string ownerId);
        Task<List<BonDeLivraison>> GetByReferences(List<string> refsBLs);



        Task<BonDeLivraison?> GetBonDeLivraisonByReference(string reference);
        Task<BonDeLivraison?> GetBonDeLivraisonById(int bonDeLivraisonId);

        Task<BonDeLivraison?> GetBonDeLivraisonByIdAndOwner(int id, string ownerId);
        Task<BonDeLivraison?> GetBonDeLivraisonByReferenceAndOwner(string reference, string ownerId);
        


        Task<bool> AddBonDeLivraison(BonDeLivraison bonDeLivraison);
        Task<bool> UpdateBonDeLivraison(BonDeLivraison bonDeLivraison);



        Task<bool> DeleteBonDeLivraison(int bonDeLivraisonId);
        
    }
}
