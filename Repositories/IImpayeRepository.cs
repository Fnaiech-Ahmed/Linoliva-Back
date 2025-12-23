using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface IImpayeRepository
    {
        Task<Impaye?> GetImpayeByIdAsync(int id);
        Task<Impaye?> UpdateImpaye(Impaye impaye);
        Task<Impaye?> AddAsyncImpaye(Impaye impaye);
    }
}
