using tech_software_engineer_consultant_int_backend.DTO;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.DTO.AbonnementsDTOs;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IAbonnementService
    {
        Task<Abonnement> CreateAbonnementAsync(
            int productId,
            string subscriptionType,
            int maxDevicesAllowed,
            int maxUsersAllowed);

        Task<Abonnement?> GetAbonnementAsync(string id);

        Task<CodeActivation?> GetLatestAbonnementOrLicenceAsync();

        Task DeleteAbonnementAsync(string id);

        Task<bool> SellAbonnementAsync(int id, AbonnementSellDTO dto);



    }
}
