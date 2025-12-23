using tech_software_engineer_consultant_int_backend.DTO.ImpayesDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IImpayeService
    {
        Task<List<Impaye>> GetImpayesAsync();
        Task<Impaye> GetImpayeAsync(int id);
        Task<int> CreateImpayeAsync(ImpayeCreateDTO impaye);
        Task UpdateImpayeAsync(int id, Impaye impaye);
        Task AffectImpayertoDossier(int idImpayer, int idDossier);
        Task<Impaye> AffectImpayertoEcheance(int idImpayer, int idEcheance);
        Task DeleteImpayeAsync(int id);
    }
}