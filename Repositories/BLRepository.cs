using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class BLRepository : IBLRepository
    {
        private readonly MyDbContext _dbContext;

        public BLRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<BonDeLivraison>> GetBonDeLivraisons()
        {
            List<BonDeLivraison> bonDeLivraisons = new List<BonDeLivraison>();

            bonDeLivraisons = await _dbContext.BonDeLivraisons.ToListAsync();

            return bonDeLivraisons;
        }

        public async Task<List<BonDeLivraison>> GetListeBonsDeLivraisonByOwner(string ownerId)
        {
            return await _dbContext.BonDeLivraisons
            .Where(b => b.ProprietaireId.ToString() == ownerId)
            .ToListAsync();
        }





        public async Task<BonDeLivraison?> GetBonDeLivraisonById(int bonDeLivraisonId)
        {
            try
            {
                BonDeLivraison? bonDeLivraison = await _dbContext.BonDeLivraisons.FindAsync(bonDeLivraisonId);
                return bonDeLivraison;
            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                return null;
            }
        }


        public async Task<BonDeLivraison?> GetBonDeLivraisonByReferenceAndOwner(string reference, string ownerId)
        {
            return await _dbContext.BonDeLivraisons
                                 .FirstOrDefaultAsync(bl => bl.Reference == reference && bl.ProprietaireId.ToString() == ownerId);
        }

        public async Task<BonDeLivraison?> GetBonDeLivraisonByIdAndOwner(int id, string ownerId)
        {
            return await _dbContext.BonDeLivraisons
                                 .FirstOrDefaultAsync(bl => bl.Id == id && bl.ProprietaireId.ToString() == ownerId);
        }


        public async Task<BonDeLivraison?> GetBonDeLivraisonByReference(string reference)
        {
            try
            {
                BonDeLivraison? BL = await _dbContext.BonDeLivraisons
                    .FirstOrDefaultAsync(BL => BL.Reference == reference);

                return BL;
            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                return null;
            }
        }


        public async Task<List<BonDeLivraison>> GetByReferences(List<string> refsBLs)
        {
            // Récupérer les BonDeLivraison en filtrant par les références fournies
            return await _dbContext.BonDeLivraisons
                .Where(bl => refsBLs.Contains(bl.Reference))
                .ToListAsync();
        }



        public async Task<bool> AddBonDeLivraison(BonDeLivraison bonDeLivraison)
        {
            try
            {
                EntityEntry<BonDeLivraison> entityEntry = await _dbContext.BonDeLivraisons.AddAsync(bonDeLivraison);
                BonDeLivraison addedEntity = entityEntry.Entity;

                int numRowsAffected = await _dbContext.SaveChangesAsync();
                return numRowsAffected > 0 && addedEntity.Id > 0;
            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
                return false;
            }
        }




        public async Task<bool> DeleteBonDeLivraison(int bonDeLivraisonId)
        {
            try
            {
                BonDeLivraison? existingBonDeLivraison = await _dbContext.BonDeLivraisons.FindAsync(bonDeLivraisonId);

                if (existingBonDeLivraison == null)
                {
                    return false;
                }
                else
                {
                    _dbContext.BonDeLivraisons.Remove(existingBonDeLivraison);
                    int nbreLigneaffectee = await _dbContext.SaveChangesAsync();

                    if (nbreLigneaffectee > 0)
                    {
                        return true;
                    }
                    else { return false; }
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateBonDeLivraison(BonDeLivraison bonDeLivraison)
        {
            try
            {
                BonDeLivraison? existingBonDeLivraison = await _dbContext.BonDeLivraisons.FindAsync(bonDeLivraison.Id);

                if (existingBonDeLivraison == null)
                {
                    return false;
                }
                else
                {
                    existingBonDeLivraison.Reference = bonDeLivraison.Reference;
                    existingBonDeLivraison.TitreClient = bonDeLivraison.TitreClient;
                    existingBonDeLivraison.NomClient = bonDeLivraison.NomClient;
                    existingBonDeLivraison.AdresseClient = bonDeLivraison.AdresseClient;
                    existingBonDeLivraison.MFClient = bonDeLivraison.MFClient;
                    existingBonDeLivraison.GSMClient = bonDeLivraison.GSMClient;
                    existingBonDeLivraison.Commandes = bonDeLivraison.Commandes; // This will update the JSON property
                    existingBonDeLivraison.MontantTotalHTBL = bonDeLivraison.MontantTotalHTBL;
                    existingBonDeLivraison.RemiseBL = bonDeLivraison.RemiseBL;
                    existingBonDeLivraison.NetHT = bonDeLivraison.NetHT;
                    existingBonDeLivraison.TVA = bonDeLivraison.TVA;
                    existingBonDeLivraison.MontantTotalTTCBL = bonDeLivraison.MontantTotalTTCBL;

                    _dbContext.BonDeLivraisons.Update(existingBonDeLivraison);
                    int nbrRowsAffected = await _dbContext.SaveChangesAsync();
                    return nbrRowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                return false;
            }
        }


        private string SerializeCoordonneesClient(List<(Titre titre, string Nom, string Adresse, string MF)> coordonnees)
        {
            return JsonConvert.SerializeObject(coordonnees);
        }

        private string SerializeCommandes(List<Commande> commandes)
        {
            return JsonConvert.SerializeObject(commandes);
        }

        private List<(Titre titre, string Nom, string Adresse, string MF)> DeserializeCoordonneesClient(string json)
        {
            return JsonConvert.DeserializeObject<List<(Titre titre, string Nom, string Adresse, string MF)>>(json);
        }

        private List<Commande> DeserializeCommandes(string json)
        {
            return JsonConvert.DeserializeObject<List<Commande>>(json);
        }
    }
}
