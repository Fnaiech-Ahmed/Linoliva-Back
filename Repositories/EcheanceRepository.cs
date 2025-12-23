using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class EcheanceRepository : IEcheanceRepository
    {
        private readonly MyDbContext _context;

        public EcheanceRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Echeance>> GetAllAsync()
        {
            return await _context.Echeances.ToListAsync();
        }

        public async Task<Echeance> GetByIdAsync(int id)
        {
            return await _context.Echeances.FindAsync(id);
        }

        public async Task<bool> AddAsync(Echeance echeance)
        {
            try
            {
                EntityEntry<Echeance> entityEntry = await _context.Echeances.AddAsync(echeance);
                Echeance addedEntity = entityEntry.Entity;

                int numRowsAffected = await _context.SaveChangesAsync();
                return numRowsAffected > 0 && addedEntity.Id > 0;
            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
                return false;
            }
            
        }

        public async Task<Echeance?> UpdateAsync(Echeance echeance)
        {
            try
            {
                EntityEntry<Echeance> entityEntry = _context.Echeances.Update(echeance);
                Echeance updatedEcheance = entityEntry.Entity;

                int numRowsAffected = await _context.SaveChangesAsync();

                if (numRowsAffected > 0)
                {
                    Echeance? existingEcheance = await _context.Echeances.FindAsync(echeance.Id);
                    return existingEcheance;
                }
                else 
                { 
                    return new Echeance(); 
                }
            }
            catch (Exception ex)
            {
                return new Echeance();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var echeance = await _context.Echeances.FindAsync(id);
            if (echeance != null)
            {
                _context.Echeances.Remove(echeance);
                await _context.SaveChangesAsync();
            }
        }
    }
}
