using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class BonDeSortieRepository : IBonDeSortieRepository
    {
        private readonly MyDbContext _dbContext;

        public BonDeSortieRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddBonDeSortie(BonDeSortie bonDeSortie)
        {
            try
            {
                // Ensure ListeFacturesSerialized is properly set
                if (bonDeSortie.ListeFactures == null)
                {
                    bonDeSortie.ListeFactures = new List<Facture>();
                }

                bonDeSortie.ListeFacturesSerialized = bonDeSortie.SerializeListeFactures(bonDeSortie.ListeFactures);

                await _dbContext.BonDeSorties.AddAsync(bonDeSortie);
                int numRowsAffected = await _dbContext.SaveChangesAsync();
                return numRowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }



        public async Task<BonDeSortie?> GetBonDeSortieById(int bonDeSortieId)
        {
            try
            {
                BonDeSortie? bonDeSortie = await _dbContext.BonDeSorties.FindAsync(bonDeSortieId);
                return bonDeSortie;
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

        public async Task<BonDeSortie?> GetBonDeSortieByReference(string reference)
        {
            try
            {
                BonDeSortie? bonDeSortie = await _dbContext.BonDeSorties
                    .FirstOrDefaultAsync(b => b.ReferenceBS == reference);
                return bonDeSortie;
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

        public async Task<List<BonDeSortie>> GetBonDeSorties()
        {
            try
            {
                return await _dbContext.BonDeSorties.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                return new List<BonDeSortie>();
            }
        }

        public async Task<bool> UpdateBonDeSortie(BonDeSortie bonDeSortie)
        {
            try
            {
                // Ensure ListeFacturesSerialized is properly set
                if (bonDeSortie.ListeFactures == null)
                {
                    bonDeSortie.ListeFactures = new List<Facture>();
                }

                bonDeSortie.ListeFacturesSerialized = bonDeSortie.SerializeListeFactures(bonDeSortie.ListeFactures);

                _dbContext.BonDeSorties.Update(bonDeSortie);
                int numRowsAffected = await _dbContext.SaveChangesAsync();
                return numRowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        public async Task<bool> DeleteBonDeSortie(int bonDeSortieId)
        {
            try
            {
                BonDeSortie? bonDeSortie = await _dbContext.BonDeSorties.FindAsync(bonDeSortieId);
                if (bonDeSortie == null)
                {
                    return false;
                }

                _dbContext.BonDeSorties.Remove(bonDeSortie);
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
