using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface IAbonnementRepository
    {
        Task<Abonnement> CreateAsync(Abonnement abonnement);
        Task<Abonnement> GetByIdAsync(string id);
        Task<CodeActivation> GetLatestAbonnementOrLicenceAsync();
        Task<IEnumerable<Abonnement>> GetAllAsync();
        Task UpdateAsync(Abonnement abonnement);
        Task DeleteAsync(string id);
    }

}
