using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class DossierService:IDossierService
    {
        private readonly MyDbContext _context;

        public DossierService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Dossier>> GetDossiersAsync()
        {
            return await _context.Dossiers.ToListAsync();
        }

        public async Task<Dossier?> GetDossierAsync(int id)
        {
            return await _context.Dossiers.FindAsync(id);
        }

        public async Task<int> CreateDossierAsync(Dossier dossier)
        {
            _context.Dossiers.Add(dossier);
            await _context.SaveChangesAsync();
            return dossier.Id;
        }

        public async Task UpdateDossierAsync(int id, Dossier dossier)
        {
            var existingDossier = await _context.Dossiers.FindAsync(id);

            if (existingDossier == null)
            {
                throw new ArgumentException("Dossier not found.");
            }

            //existingDossier.Name = dossier.Name;
            //existingDossier.Id = dossier.Id;
            //existingDossier.Email = dossier.Email;

            await _context.SaveChangesAsync();
        }

        public async Task AffectFoldertoPortfolio(int idFolder, int idPortfolio)
        {
            var existingDossier = await _context.Dossiers.FindAsync(idFolder);
            var existingPortfolio = await _context.Portefeuilles.FindAsync(idPortfolio);
            if (existingDossier == null) {
                throw new ArgumentException("Dossier not found.");
            }
            if (existingPortfolio == null)
            {
                throw new ArgumentException("Portefeuille not found.");
            }
            existingDossier.PortefeuilleId = idPortfolio;
            existingDossier.Portefeuille = existingPortfolio;

            existingPortfolio.ListeDossiers.Add(existingDossier);
            existingPortfolio.NbrDossiers += 1;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDossierAsync(int id)
        {
            var dossier = await _context.Dossiers.FindAsync(id);

            if (dossier == null)
            {
                throw new ArgumentException("Dossier not found.");
            }

            _context.Dossiers.Remove(dossier);
            await _context.SaveChangesAsync();
        }

    }
}