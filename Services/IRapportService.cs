using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IRapportService
    {
        Task<List<Rapport>> GetRapportsAsync();
        Task<Rapport> GetRapportAsync(int id);
        Task<int> CreateRapportAsync(Rapport rapport);
        Task UpdateRapportAsync(int id, Rapport Rapport);
        Task AssignReportToPortfolio(int idReport, int idPortfolio);
        Task DeleteRapportAsync(int id);
    }
}
