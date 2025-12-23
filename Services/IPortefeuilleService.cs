using tech_software_engineer_consultant_int_backend.DTO.PortfoliosDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IPortefeuilleService
    {
        Task<List<Portefeuille>> GetPortefeuillesAsync();
        Task<Portefeuille?> GetPortefeuilleAsync(int id);
        Task<(bool, int, List<(string, string)>)> CreatePortefeuilleAsync(PortefeuilleCreateDTO portefeuilleCreateDTO, List<string> listeREFsRecouvreurs);
        Task UpdatePortefeuilleAsync(int id, Portefeuille portefeuille);
        Task<(bool, string)> UpdatePortefeuilleUsersAsync(int id, Portefeuille portefeuille);
        Task DeletePortefeuilleAsync(int id);
    }
}
