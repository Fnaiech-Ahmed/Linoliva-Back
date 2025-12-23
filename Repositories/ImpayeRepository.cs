using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class ImpayeRepository : IImpayeRepository
    {
        private readonly MyDbContext _context;

        public ImpayeRepository(MyDbContext myDbContext) 
        {
           _context = myDbContext;
        }

        public async Task<Impaye?> GetImpayeByIdAsync(int id)
        {
            Impaye? impaye = await _context.Impayes.FindAsync(id);
            return impaye;
        }

        public async Task<Impaye?> UpdateImpaye(Impaye impaye)
        {
            try
            {
                EntityEntry<Impaye> entityEntry = _context.Impayes.Update(impaye);
                Impaye updatedEntity = entityEntry.Entity;

                int numRowsAffected = await _context.SaveChangesAsync();

                if (numRowsAffected > 0)
                {
                    Impaye? existingImpaye = await _context.Impayes.FindAsync(impaye.id);
                    return existingImpaye;
                }
                else
                {
                    return new Impaye();
                }

            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
                return new Impaye();
            }
        }


        public async Task<Impaye?> AddAsyncImpaye(Impaye impaye)
        {
            try
            {
                EntityEntry<Impaye> entityEntry = await _context.Impayes.AddAsync(impaye);
                Impaye impayeAdded = entityEntry.Entity;

                int numRowsAdded = await _context.SaveChangesAsync(); 
                if (numRowsAdded > 0)
                {
                    Impaye? existingImpaye = await _context.Impayes.FindAsync(impaye.id);
                    return existingImpaye;
                }
                else
                {
                    return new Impaye();
                }
            }
            catch (Exception ex)
            {
                return new Impaye();
            }
        }
    }
}
