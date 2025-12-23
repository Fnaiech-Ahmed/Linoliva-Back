using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class LotRepository : ILotRepository
    {
        private readonly MyDbContext _dbContext;

        public LotRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Lot>> GetLots()
        {
            try
            {
                return await _dbContext.Lots.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                return new List<Lot>();
            }
        }

        public async Task<Lot?> GetLotById(int lotId)
        {
            try
            {
                return await _dbContext.Lots.FindAsync(lotId);
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

        public async Task<List<Lot>> GetLotsByProductId(int productId)
        {
            try
            {
                return await _dbContext.Lots
                    .Where(l => l.IDProduit == productId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                return new List<Lot>();
            }
        }

        public async Task<Lot?> GetLotByReference(string reference)
        {
            try
            {
                return await _dbContext.Lots
                    .FirstOrDefaultAsync(l => l.Reference == reference);
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

        public async Task<int> AddLot(Lot lot)
        {
            try
            {
                await _dbContext.Lots.AddAsync(lot);
                await _dbContext.SaveChangesAsync();
                return lot.Id;
            }
            catch (Exception ex)
            {
                // Log the exception
                return 0;
            }
        }

        public async Task<bool> UpdateLot(int lotId, Lot lot)
        {
            try
            {
                var existingLot = await _dbContext.Lots.FindAsync(lotId);
                if (existingLot == null)
                {
                    return false;
                }

                existingLot.Reference = lot.Reference;
                existingLot.IDProduit = lot.IDProduit;
                existingLot.Quantite = lot.Quantite;

                _dbContext.Lots.Update(existingLot);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        public async Task<bool> DeleteLot(int lotId)
        {
            try
            {
                var lot = await _dbContext.Lots.FindAsync(lotId);
                if (lot == null)
                {
                    return false;
                }

                _dbContext.Lots.Remove(lot);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }
    }
}
