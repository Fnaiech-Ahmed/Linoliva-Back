using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IDossierService
    {
        Task<List<Dossier>> GetDossiersAsync();
        Task<Dossier?> GetDossierAsync(int id);
        Task<int> CreateDossierAsync(Dossier dossier);
        Task UpdateDossierAsync(int id, Dossier dossier);
        Task AffectFoldertoPortfolio(int DossierId, int portfolioId);
        Task DeleteDossierAsync(int id);
    }
}