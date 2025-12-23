using tech_software_engineer_consultant_int_backend.DTO.RapportsJournaliersDTOs;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.DTO.ProductDTOs;
using tech_software_engineer_consultant_int_backend.DTO.RecettesDTOs;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IRapportJournalierService
    {
        Task<RapportJournalier> GetRapportByIdAsync(int id);
        Task<string> CreateRapportAsync(RapportJournalierDto dto);
        Task<List<RapportJournalierDto>> GetAllRapportsAsync();


        Task<(bool, string, RapportJournalier)> MettreAJourRapportAsync(RapportJournalierDto rapport);


        Task<List<ProductYearKey>> GetDepensesParProduitParAnnee(DateTime startDate, DateTime endDate, List<string> ListNamesProducts);
        Task<List<ProductMonthKey>> GetDepensesParProduitParMois(DateTime startDate, DateTime endDate, List<string> ListNamesProducts);
        Task<List<ProductWeekKey>> GetDepensesParProduitParSemaine(DateTime startDate, DateTime endDate, List<string> ListNamesProducts);
        Task<List<ProductDayKey>> GetDepensesParProduitParJour(DateTime startDate, DateTime endDate, List<string> ListNamesProducts);




        Task<List<RecetteEqpYearKey>> GetRecettesParEquipeParAnnee(DateTime startDate, DateTime endDate, List<int> ListNumEquipes);
        Task<List<RecetteEqpMonthKey>> GetRecettesParEquipeParMois(DateTime startDate, DateTime endDate, List<int> ListNumEquipes);
        Task<List<RecetteEqpWeekKey>> GetRecettesParEquipeParSemaine(DateTime startDate, DateTime endDate, List<int> ListNumEquipes);
        Task<List<RecetteEqpDayKey>> GetRecettesParEquipeParJour(DateTime startDate, DateTime endDate, List<int> ListNumEquipes);




        Task<List<ProductQYearKey>> GetQuantitesParProduitParAnnee(DateTime startDate, DateTime endDate, List<string> ListNamesProducts);
        Task<List<ProductQMonthKey>> GetQuantitesParProduitParMois(DateTime startDate, DateTime endDate, List<string> ListNamesProducts);
        Task<List<ProductQWeekKey>> GetQuantitesParProduitParSemaine(DateTime startDate, DateTime endDate, List<string> ListNamesProducts);
        Task<List<ProductQDayKey>> GetQuantitesParProduitParJour(DateTime startDate, DateTime endDate, List<string> ListNamesProducts);


    }

}
