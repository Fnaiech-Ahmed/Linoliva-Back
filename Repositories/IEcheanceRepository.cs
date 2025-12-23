using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface IEcheanceRepository
    {
        Task<IEnumerable<Echeance>> GetAllAsync();
        Task<Echeance> GetByIdAsync(int id);
        Task<bool> AddAsync(Echeance echeance);
        Task<Echeance?> UpdateAsync(Echeance echeance);
        Task DeleteAsync(int id);
    }
}
