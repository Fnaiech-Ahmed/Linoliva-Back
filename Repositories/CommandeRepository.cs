using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class CommandeRepository : ICommandeRepository
    {
        private readonly MyDbContext _dbContext;

        public CommandeRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddCommande(Commande commande)
        {
            try
            {
                await _dbContext.Commandes.AddAsync(commande);
                int numRowsAffected = await _dbContext.SaveChangesAsync();
                return numRowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }




        public async Task<Commande?> GetCommandeById(int commandeId)
        {
            try
            {
                Commande? commande = await _dbContext.Commandes.FindAsync(commandeId);
                return commande;
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

        public async Task<Commande?> GetCommandeByRef(string referenceCommande)
        {
            try
            {
                Commande? commande = await _dbContext.Commandes
                    .FirstOrDefaultAsync(c => c.ReferenceCommande == referenceCommande);
                return commande;
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

        public async Task<List<Commande>> GetAllCommandes()
        {
            try
            {
                return await _dbContext.Commandes.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                return new List<Commande>();
            }
        }

        public async Task<List<Commande>> GetListeCommandesByOwner(string OwnerId)
        {
            return await _dbContext.Commandes
            .Where(c => c.ProprietaireId.ToString() == OwnerId)
            .ToListAsync();
        }



        public async Task<(bool IsSuccess, string Message)> UpdateCommande(Commande commande)
        {
            try
            {
                _dbContext.Commandes.Update(commande);
                int numRowsAffected = await _dbContext.SaveChangesAsync();

                if (numRowsAffected > 0)
                {
                    return (true, "La commande a été mise à jour avec succès.");
                }
                else
                {
                    return (false, "Aucune modification n'a été enregistrée.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                // Vous pouvez aussi logguer l'erreur ici
                return (false, $"Erreur lors de la mise à jour : {ex.Message}");
            }
        }




        public async Task<bool> DeleteCommande(int commandeId)
        {
            try
            {
                Commande? commande = await _dbContext.Commandes.FindAsync(commandeId);
                if (commande == null)
                {
                    return false;
                }

                _dbContext.Commandes.Remove(commande);
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
