using tech_software_engineer_consultant_int_backend.DTO.BLDTO;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IBLService
    {
        Task<bool> AddBonDeLivraison(BLCreateDTO bonDeLivraisonDTO);



        Task<List<BLDTO>> GetBonDeLivraisons();

        Task<List<BLDTO>> GetListeBonsDeLivraisonByOwner(string ownerId);


        Task<List<BonDeLivraison>> GetBonDeLivraisonsByRefs(List<string> refsBLs);
        Task<BLDTO?> GetBonDeLivraisonById(int id);
        Task<BonDeLivraison?> GetBonDeLivraisonByIdAndOwner(int id, string ownerId);
        Task<BLDTO?> GetBonDeLivraisonByReference(string reference);
        Task<BonDeLivraison?> GetBonDeLivraisonByReferenceAndOwner(string reference, string ownerId);
        Task<bool> UpdateBonDeLivraison(int bonDeLivraisonId, BLUpdateDTO bonDeLivraisonUpdateDTO);
        Task<bool> DeleteBonDeLivraison(int id);


        string GenerateNextRef();
        decimal CalculerMontantTotalTTC(BonDeLivraison bonDeLivraison);
        decimal CalculerMontantTotalHT(BonDeLivraison bonDeLivraison);
        decimal CalculerNetHT(BonDeLivraison bonDeLivraison);
    }
}
