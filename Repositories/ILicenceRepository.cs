using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface ILicenceRepository
    {
        Task<Licence> CreateAsync(Licence licence);
        Task<Licence> GetByIdAsync(string id);
        Task<IEnumerable<Licence>> GetAllAsync();
        Task UpdateAsync(Licence licence);
        Task DeleteAsync(string id);
    }

}
