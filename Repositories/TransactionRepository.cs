using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.Xml;
//using tech_software_engineer_consultant_int_backend.DTOs.ProductsDTOs;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly MyDbContext _dbContext;


        public TransactionRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Transactions>> GetTransactions()
        {
            List<Transactions> transactions = new List<Transactions>();

            transactions = await _dbContext.Transactions.ToListAsync();

            return transactions;
        }




        public async Task<(bool,int)> AddTransaction(Transactions transaction)
        {
            try
            {
                EntityEntry<Transactions> entityEntry = await _dbContext.Transactions.AddAsync(transaction);
                Transactions addedEntity = entityEntry.Entity;

                int numRowsAffected = await _dbContext.SaveChangesAsync();

                if (numRowsAffected > 0 && addedEntity.Id > 0)
                {
                    return (true, addedEntity.Id) ;
                }
                return (false, 0);
            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
                return (false,0);
            }
        }



        public async Task<Transactions> GetTransactionById(int transactionId)
        {
            try
            {
                Transactions? transaction = await _dbContext.Transactions
                    .FirstOrDefaultAsync(t => t.Id == transactionId);

                return transaction;
            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
                return null;
            }
        }


        public async Task<List<Transactions>> GetTransactionsByRefCommande(string referenceCommande)
        {
            try
            {
                List<Transactions>? transactions = await _dbContext.Transactions
                    .Where(t => t.ReferenceCommande == referenceCommande)
                    .ToListAsync();

                return transactions;
            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
                return null;
            }
        }



        public void UpdateTransaction(Transactions transaction)
        {
            
        }

        public async Task<bool> UpdateTransactionRemainingQuantity(int productId, int newRemainingQuantity)
        {
            try
            {
                // Récupérer les transactions à mettre à jour de manière asynchrone
                var transactions = await _dbContext.Transactions
                    .Where(t => t.ProductId == productId)
                    .ToListAsync();

                // Mettre à jour le champ RemainingQuantity
                foreach (var transaction in transactions)
                {
                    transaction.RemainingQuantity = newRemainingQuantity;
                }

                // Sauvegarder les modifications de manière asynchrone
                int numRowsAffected = await _dbContext.SaveChangesAsync();

                return numRowsAffected == transactions.Count;
            }
            catch (Exception ex)
            {
                // Log de l'exception (facultatif)
                // Logger.LogError(ex, "Error updating transaction remaining quantity.");
                return false;
            }
        }




        public async Task<bool> UpdateRefLotTransaction(int transactionId, string ReferenceLot)
        {
            try
            {
                // Récupérer la transactions à mettre à jour
                Transactions? transaction = await _dbContext.Transactions
                    .FirstOrDefaultAsync(t => t.Id == transactionId);

                if (transaction == null)
                {
                    return false;
                }

                // Mettre à jour le champ ReferenceLot                
                transaction.ReferenceLot = ReferenceLot;

                // Sauvegarder les modifications de manière asynchrone
                int numRowsAffected = await _dbContext.SaveChangesAsync();

                return numRowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log de l'exception (facultatif)
                // Logger.LogError(ex, "Error updating transaction remaining quantity.");
                return false;
            }
        }

        public async Task<bool> DeleteTransaction(int transactionId)
        {
            try
            {
                // Récupérer la transactions à mettre à jour
                Transactions? existingTransaction = await _dbContext.Transactions
                    .FirstOrDefaultAsync(t => t.Id == transactionId);

                if (existingTransaction == null)
                {
                    return false;
                }

                // Mettre à jour le champ ReferenceLot                
                _dbContext.Transactions.Remove(existingTransaction);

                // Sauvegarder les modifications de manière asynchrone
                int numRowsAffected = await _dbContext.SaveChangesAsync();

                return numRowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log de l'exception (facultatif)
                // Logger.LogError(ex, "Error updating transaction remaining quantity.");
                return false;
            }
        }

    }
}
