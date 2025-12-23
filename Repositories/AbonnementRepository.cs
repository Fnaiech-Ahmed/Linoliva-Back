using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class AbonnementRepository : IAbonnementRepository
    {
        private readonly MyDbContext _context;

        public AbonnementRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<Abonnement> CreateAsync(Abonnement abonnement)
        {
            await _context.Set<Abonnement>().AddAsync(abonnement);
            await _context.SaveChangesAsync();
            return abonnement;
        }

        public async Task<Abonnement> GetByIdAsync(string id)
        {
            return await _context.Set<Abonnement>().FindAsync(id);
        }

        // Méthode pour obtenir la dernière ligne d'abonnement ou de licence
        // Méthode de recherche dans la BD du logiciel lui même !!!
        public async Task<CodeActivation> GetLatestAbonnementOrLicenceAsync()
        {
            // Ordre décroissant par date ou par Id
            return await _context.CodeActivations
                .OrderByDescending(a => a.ActivationDate) // Vous pouvez aussi utiliser 'Id' si pertinent
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Abonnement>> GetAllAsync()
        {
            return await _context.Set<Abonnement>().ToListAsync();
        }

        public async Task UpdateAsync(Abonnement abonnement)
        {
            _context.Set<Abonnement>().Update(abonnement);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var abonnement = await GetByIdAsync(id);
            if (abonnement != null)
            {
                _context.Set<Abonnement>().Remove(abonnement);
                await _context.SaveChangesAsync();
            }
        }
    }

}
