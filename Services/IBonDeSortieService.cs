using tech_software_engineer_consultant_int_backend.DTO.BonsDeSortieDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IBonDeSortieService
    {
        Task<List<BonDeSortieDTO>> GetBonDeSorties();
        Task<BonDeSortieDTO?> GetBonDeSortieByReference(string reference);
        Task<BonDeSortieDTO?> GetBonDeSortieById(int bonDeSortieId);
        Task<bool> AddBonDeSortie(BonDeSortieCreateDTO bonDeSortieCreateDTO);
        Task<bool> UpdateBonDeSortie(int bonDeSortieId, BonDeSortieUpdateDTO bonDeSortieUpdateDTO);
        Task<bool> DeleteBonDeSortie(int bonDeSortieId);
    }
}
