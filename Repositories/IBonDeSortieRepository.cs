using System.Collections.Generic;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface IBonDeSortieRepository
    {
        Task<bool> AddBonDeSortie(BonDeSortie bonDeSortie);
        Task<BonDeSortie?> GetBonDeSortieById(int bonDeSortieId);
        Task<BonDeSortie?> GetBonDeSortieByReference(string reference);
        Task<List<BonDeSortie>> GetBonDeSorties();
        Task<bool> UpdateBonDeSortie(BonDeSortie bonDeSortie);
        Task<bool> DeleteBonDeSortie(int bonDeSortieId);
    }
}
