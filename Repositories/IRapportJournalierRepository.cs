using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.DTO.RapportsJournaliersDTOs;
using Microsoft.EntityFrameworkCore;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface IRapportJournalierRepository
    {
        Task<RapportJournalier> GetRapportByIdAsync(int id);
        Task<RapportJournalier> GetRapportByDateAsync(DateTime dateSansHeure);
        Task<List<RapportJournalier>> GetAllRapportsAsync();

        Task<string> CreateRapportAsync(RapportJournalier rapport);      


        Task<bool> RapportExisteDejaPourDateAsync(DateTime dateSansHeure, int? rapportId = null);
        Task<(bool, RapportJournalier)> UpdateRapport(RapportJournalier rapport);


        Task DeleteRecettesByRapportId(int rapportId);
        Task DeleteDepensesByRapportId(int rapportId);
        

        Task<List<Depense>> GetDepensesWithinDateRange(DateTime startDate, DateTime endDate);


        Task<List<Depense>> GetDepensesSingleProductWithinDateRange(DateTime startDate, DateTime endDate, string NameProduct);



        Task<List<Recette>> GetRecettesWithinDateRange(DateTime startDate, DateTime endDate);

        Task<List<Recette>> GetRecettesSingleEquipeWithinDateRange(DateTime startDate, DateTime endDate, int numEquipe);


        Task<List<RapportJournalier>> GetRapportsWithinDateRange(DateTime startDate, DateTime endDate);


    }

}
