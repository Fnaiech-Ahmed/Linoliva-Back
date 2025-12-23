using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.DTO.RapportsJournaliersDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class RapportJournalierRepository : IRapportJournalierRepository
    {
        private readonly MyDbContext _context;

        public RapportJournalierRepository(MyDbContext context)
        {
            _context = context;
        }

        // Trouver un rapport par son ID
        public async Task<RapportJournalier> GetRapportByIdAsync(int id)
        {
            return await _context.RapportsJournalier
                .Include(r => r.Recettes)
                .Include(r => r.Depenses)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<string> CreateRapportAsync(RapportJournalier rapport)
        {
            // 1. Extraire la partie jour/mois/année de la date pour comparaison
            DateTime dateSansHeure = rapport.Date.Date;

            // 2. Vérifier si un rapport avec cette date existe déjà
            bool rapportExiste = await _context.RapportsJournalier
                                               .AnyAsync(r => r.Date == dateSansHeure);
            if (rapportExiste)
            {
                return "Un rapport existe déjà pour cette date.";
            }

            // 3. Ajouter le nouveau rapport si la date est unique
            _context.RapportsJournalier.Add(rapport);
            await _context.SaveChangesAsync();

            return "Rapport ajouté avec succès.";
        }

        public async Task<List<RapportJournalier>> GetAllRapportsAsync()
        {
            return await _context.RapportsJournalier
                .Select(r => new RapportJournalier
                {
                    Id = r.Id,
                    Date = r.Date,
                    Recettes = r.Recettes,
                    Depenses = r.Depenses
                })
                .ToListAsync();
        }

        

        // Vérifier si un rapport avec une date particulière existe (autre que le rapport à mettre à jour)
        public async Task<bool> RapportExisteDejaPourDateAsync(DateTime dateSansHeure, int? rapportId = null)
        {
            return await _context.RapportsJournalier
                .AnyAsync(r => r.Date == dateSansHeure && (rapportId == null || r.Id != rapportId));
        }

        public async Task<RapportJournalier> GetRapportByDateAsync(DateTime dateSansHeure)
        {
            return await _context.RapportsJournalier
                .FirstOrDefaultAsync(r => r.Date == dateSansHeure);
        }

        // Mise à jour du rapport
        public async Task<(bool,RapportJournalier)> UpdateRapport(RapportJournalier rapport)
        {
            //_context.RapportsJournalier.Update(rapport);
            try
            {
                EntityEntry<RapportJournalier> entityEntry = _context.RapportsJournalier.Update(rapport);
                RapportJournalier updatedEntity = entityEntry.Entity;

                int numRowsAffected = await _context.SaveChangesAsync();

                if (numRowsAffected > 0)
                {
                    RapportJournalier? existingRapportJournalier = await _context.RapportsJournalier.FindAsync(rapport.Id);
                    return (true,existingRapportJournalier);
                    //return true;
                }
                else
                {
                    return (false,null);
                    //return false;
                }

            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
                return (false,null);
                //return true;
            }
        }


        public async Task DeleteRecettesByRapportId(int rapportId)
        {
            var recettes = await _context.Recettes
                .Where(r => r.RapportJournalierId == rapportId) // Assurez-vous que c'est la bonne relation
                .ToListAsync();

            _context.Recettes.RemoveRange(recettes);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepensesByRapportId(int rapportId)
        {
            var depenses = await _context.Depenses
                .Where(d => d.RapportJournalierId == rapportId) // Assurez-vous que c'est la bonne relation
                .ToListAsync();

            _context.Depenses.RemoveRange(depenses);
            await _context.SaveChangesAsync();
        }




        // Méthode pour récupérer les dépenses dans la plage de dates
        public async Task<List<Depense>> GetDepensesWithinDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.Depenses
                .Where(d => d.DateTransaction >= startDate && d.DateTransaction <= endDate)
                .ToListAsync();
        }


        // Méthode pour récupérer les dépenses d'un seul Produit dans la plage de dates
        public async Task<List<Depense>> GetDepensesSingleProductWithinDateRange(DateTime startDate, DateTime endDate, string NameProduct)
        {
            return await _context.Depenses
                .Where(d => d.DateTransaction >= startDate && d.DateTransaction <= endDate && d.ProductName == NameProduct)
                .ToListAsync();
        }





        public async Task<List<Recette>> GetRecettesWithinDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.Recettes
                .Where(d => d.DateTransaction >= startDate && d.DateTransaction <= endDate)
                .ToListAsync();
        }

        public async Task<List<Recette>> GetRecettesSingleEquipeWithinDateRange(DateTime startDate, DateTime endDate, int numEquipe)
        {
            return await _context.Recettes
                .Where(d => d.DateTransaction >= startDate && d.DateTransaction <= endDate && d.PosteRecetteId == numEquipe)
                .ToListAsync();
        }




        public async Task<List<RapportJournalier>> GetRapportsWithinDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.RapportsJournalier
                .Where(d => d.Date >= startDate && d.Date <= endDate)
                .ToListAsync();
        }




    }

}
