using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class FactureRepository : IFactureRepository
    {
        private readonly MyDbContext _dbContext;

        public FactureRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddFacture(Facture facture)
        {
            try
            {
                await _dbContext.Factures.AddAsync(facture);
                int numRowsAffected = await _dbContext.SaveChangesAsync();
                return numRowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        public async Task<Facture?> GetFactureById(int factureId)
        {
            try
            {
                Facture? facture = await _dbContext.Factures.FindAsync(factureId);
                return facture;
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

        public async Task<Facture?> GetFactureByReference(string reference)
        {
            try
            {
                Facture? facture = await _dbContext.Factures
                    .FirstOrDefaultAsync(f => f.ReferenceFacture == reference);
                return facture;
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

        public async Task<List<Facture>> GetAllFactures()
        {
            try
            {
                return await _dbContext.Factures.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                return new List<Facture>();
            }
        }

        public async Task<List<Facture>> GetListeFacturesByOwner(string OwnerId)
        {
            return await _dbContext.Factures
            .Where(c => c.ProprietaireId.ToString() == OwnerId)
            .ToListAsync();
        }



        public async Task<bool> UpdateFacture(Facture facture)
        {
            try
            {
                _dbContext.Factures.Update(facture);
                int numRowsAffected = await _dbContext.SaveChangesAsync();
                return numRowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }




        public async Task<bool> DeleteFacture(int factureId)
        {
            try
            {
                Facture? facture = await _dbContext.Factures.FindAsync(factureId);
                if (facture == null)
                {
                    return false;
                }

                _dbContext.Factures.Remove(facture);
                int numRowsAffected = await _dbContext.SaveChangesAsync();
                return numRowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }
    }
}
