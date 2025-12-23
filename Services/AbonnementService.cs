using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;
using tech_software_engineer_consultant_int_backend.DTO.AbonnementsDTOs;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class AbonnementService : IAbonnementService
    {
        private readonly IAbonnementRepository _repository;

        public AbonnementService(IAbonnementRepository repository)
        {
            _repository = repository;
        }

        // -------------------------------------------
        // 1. CREATION D’ABONNEMENT
        // -------------------------------------------
        public async Task<Abonnement> CreateAbonnementAsync(
            int productId,
            string subscriptionType,
            int maxDevicesAllowed,
            int maxUsersAllowed)
        {
            var abonnement = new Abonnement
            {
                ProductId = productId,
                SubscriptionType = subscriptionType,
                IsActive = true,
                MaxDevicesAllowed = maxDevicesAllowed,
                MaxUsersAllowed = maxUsersAllowed,

                // Dates
                ActivationDate = DateTime.UtcNow,
                ExpirationDate = CalculateEndDate(DateTime.UtcNow, subscriptionType),
                RenewalDate = CalculateEndDate(DateTime.UtcNow, subscriptionType),

                // Code
                ActivationCodeReference = Guid.NewGuid().ToString("N"),
                ActivationCode = GenerateActivationCode(),

                // Other
                RenewalAttempts = 0,
                IsRecurring = false,
            };

            return await _repository.CreateAsync(abonnement);
        }

        // -------------------------------------------
        // 2. GET
        // -------------------------------------------
        public async Task<Abonnement?> GetAbonnementAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Abonnement>> GetAllAbonnementsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<CodeActivation?> GetLatestAbonnementOrLicenceAsync()
        {
            return await _repository.GetLatestAbonnementOrLicenceAsync();
        }

        // -------------------------------------------
        // 3. UPDATE
        // -------------------------------------------
        public async Task UpdateAbonnementAsync(Abonnement abonnement)
        {
            await _repository.UpdateAsync(abonnement);
        }

        // -------------------------------------------
        // 4. DELETE
        // -------------------------------------------
        public async Task DeleteAbonnementAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        // -------------------------------------------
        // 5. SELL ABONNEMENT
        // -------------------------------------------
        public async Task<bool> SellAbonnementAsync(int id, AbonnementSellDTO dto)
        {
            var abonnement = await _repository.GetByIdAsync(id.ToString());
            if (abonnement == null)
                return false;

            abonnement.IsRecurring = dto.IsRecurring;
            abonnement.IsActive = dto.IsActive;
            abonnement.PaymentMethod = dto.PaymentMethod;

            if (dto.ActivationDate.HasValue)
            {
                abonnement.ActivationDate = dto.ActivationDate;
                abonnement.RenewalDate = CalculateRenewalDate(dto.ActivationDate.Value, abonnement.SubscriptionType);
                abonnement.ExpirationDate = CalculateEndDate(dto.ActivationDate.Value, abonnement.SubscriptionType);
            }

            await _repository.UpdateAsync(abonnement);
            return true;
        }

        // -------------------------------------------
        // PRIVATE HELPERS
        // -------------------------------------------
        private string GenerateActivationCode()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }

        private DateTime CalculateEndDate(DateTime start, string type)
        {
            return type switch
            {
                "Mensuel" => start.AddMonths(1),
                "Hebdomadaire" => start.AddDays(7),
                "Bimensuel" => start.AddMonths(2),
                "Trimestrielle" => start.AddMonths(3),
                "Annuel" => start.AddYears(1),
                "Biannuel" => start.AddYears(2),
                _ => start.AddMonths(1)
            };
        }

        private DateTime CalculateRenewalDate(DateTime start, string type)
        {
            return CalculateEndDate(start, type);
        }
    }
}
