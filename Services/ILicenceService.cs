using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface ILicenceService
    {
        Task<Licence> CreateLicenceAsync(string clientRef, string licenceType, string region, int maxUsersAllowed);
        Task<Licence> GetLicenceAsync(string id);
        Task<IEnumerable<Licence>> GetAllLicencesAsync();
        Task UpdateLicenceAsync(Licence licence);
        Task DeleteLicenceAsync(string id);
    }
}
