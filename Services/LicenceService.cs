using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class LicenceService : ILicenceService
    {
        private readonly ILicenceRepository _repository;

        public LicenceService(ILicenceRepository repository)
        {
            _repository = repository;
        }

        public async Task<Licence> CreateLicenceAsync(string clientRef, string licenceType, string region, int maxUsersAllowed)
        {
            var licence = new Licence
            {
                ClientRef = clientRef,
                LicenceType = licenceType,
                IsActive = true,
                MaxUsersAllowed = maxUsersAllowed,
                ActivationCode = GenerateActivationCode(),
                ActivationDate = DateTime.Now,
                ActivationAttempts = 0
            };

            return await _repository.CreateAsync(licence);
        }

        public async Task<Licence> GetLicenceAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Licence>> GetAllLicencesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task UpdateLicenceAsync(Licence licence)
        {
            await _repository.UpdateAsync(licence);
        }

        public async Task DeleteLicenceAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        private string GenerateActivationCode()
        {
            // Logique pour générer un code d'activation
            return Guid.NewGuid().ToString();
        }
    }

}
