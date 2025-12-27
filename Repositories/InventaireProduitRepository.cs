using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class InventaireProduitRepository : IInventaireProduitRepository
    {
        private readonly MyDbContext _dbContext;

        public InventaireProduitRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<InventaireProduit>> GetInventaireProduits()
        {
            try
            {
                return await _dbContext.InventaireProduits.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                return new List<InventaireProduit>();
            }
        }

        public async Task<InventaireProduit> GetInventaireProduitById(int inventaireProduitId)
        {
            try
            {
                return await _dbContext.InventaireProduits.FindAsync(inventaireProduitId);
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

        public async Task<InventaireProduit> GetInventaireProduitByProductId(int productId)
        {
            try
            {
                return await _dbContext.InventaireProduits
                    .FirstOrDefaultAsync(p => p.ProductId == productId);
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

        public async Task<bool> AddInventaireProduit(InventaireProduit inventaireProduit)
        {
            try
            {
                await _dbContext.InventaireProduits.AddAsync(inventaireProduit);
                int numRowsAffected = await _dbContext.SaveChangesAsync();
                return numRowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        public async Task<bool> UpdateInventaireProduit(InventaireProduit inventaireProduit)
        {
            try
            {
                _dbContext.InventaireProduits.Update(inventaireProduit);
                int numRowsAffected = await _dbContext.SaveChangesAsync();
                return numRowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        public async Task<bool> UpdateQuantite(int productId, int newQuantite)
        {
            var inv = await _dbContext.InventaireProduits
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (inv == null)
                return false;

            inv.Quantity = newQuantite;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteInventaireProduit(int inventaireProduitId)
        {
            try
            {
                InventaireProduit inventaireProduit = await _dbContext.InventaireProduits.FindAsync(inventaireProduitId);
                if (inventaireProduit == null)
                {
                    return false;
                }

                _dbContext.InventaireProduits.Remove(inventaireProduit);
                int numRowsAffected = await _dbContext.SaveChangesAsync();
                return numRowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        public async Task<bool> UpdateInventaireProduitQuantity(int productId, string newProductName, int newQuantity)
        {
            try
            {
                InventaireProduit inventaireProduit = await _dbContext.InventaireProduits
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (inventaireProduit == null)
                {
                    return false;
                }

                inventaireProduit.ProductName = newProductName;
                inventaireProduit.Quantity = newQuantity;

                _dbContext.InventaireProduits.Update(inventaireProduit);
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
