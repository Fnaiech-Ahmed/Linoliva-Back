using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using tech_software_engineer_consultant_int_backend.DTO.LotsDTOs;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class LotService : ILotService
    {
        private readonly ILotRepository lotRepository;
        private readonly IInventaireProduitService inventaireProduitService;
        private readonly MyDbContext _dbContext;


        public LotService(ILotRepository repository, IInventaireProduitService _inventaireProduitService, MyDbContext dbContext)
        {
            lotRepository = repository;
            inventaireProduitService = _inventaireProduitService;
            _dbContext = dbContext;
        }

        public async Task<int> AddLot(LotCreateDTO lotCreateDTO)
        {
            // Ajouter la logique pour gérer l'ajout du lot dans la base de données
            // Vous pouvez également effectuer des validations avant d'ajouter le lot
            Lot lot = lotCreateDTO.ToLotEntity(); 
            return await lotRepository.AddLot(lot);
        }

        public async Task<List<LotDTO>> GetLots()
        {
            List<Lot> listeLots = await lotRepository.GetLots();
            if (listeLots == null)
            {
                return new List<LotDTO>();
            }
            else
            {
                List<LotDTO> listLotsDTOs = new List<LotDTO>();
                foreach (Lot lot in listeLots)
                {
                    LotDTO lotDTO = LotDTO.FromLotEntity(lot);
                    listLotsDTOs.Add(lotDTO);
                }
                return listLotsDTOs;
            }
            
        }

        public async Task<LotDTO?> GetLotById(int id)
        {
            Lot? existingLot = await lotRepository.GetLotById(id);
            if (existingLot == null)
                return new LotDTO();
            else
                return LotDTO.FromLotEntity(existingLot);
        }

        public async Task<bool> UpdateLot(int lotId, LotUpdateDTO lotUpdateDTO)
        {
            Lot? existingLot = await lotRepository.GetLotById(lotId);

            if (existingLot != null)
            {
                // Ajouter la logique pour mettre à jour les propriétés du lot existant
                Lot lot = lotUpdateDTO.ToLotEntity();
                bool resultatUpdateLot = await lotRepository.UpdateLot(lotId, lot);
                return resultatUpdateLot;
            }
            else
            {
                // Gérer la situation où le lot n'existe pas
                return false;
            }
        }

        /*public async Task<List<(Lot, int)>> VenteQuantite(int ProductId, int QuantiteSaisie) 
        {
            int x = QuantiteSaisie;
            List<Lot> existingsLotsProduct = await lotRepository.GetLotsByProductId(ProductId);

            // Tri de la liste par la propriété Date en ordre croissant
            List<Lot> listeTriee = existingsLotsProduct.OrderBy(objet => objet.Date).ToList();

            List<(Lot,int)> listeLots = new List<(Lot, int)>();
            for (int i = 0; i < listeTriee.Count; i++)
            {
                if (listeTriee[i].Quantite == 0)
                {
                    continue;
                }
                else if(listeTriee[i].Quantite > 0 && listeTriee[i].Quantite < x)
                {
                    listeLots.Add((listeTriee[i], listeTriee[i].Quantite));
                    x = QuantiteSaisie - listeTriee[i].Quantite;
                    listeTriee[i].Quantite = 0;
                    await lotRepository.UpdateLot(listeTriee[i].Id, listeTriee[i] );                    
                }
                else if (listeTriee[i].Quantite > x)
                {
                    listeLots.Add((listeTriee[i], x));
                    listeTriee[i].Quantite = listeTriee[i].Quantite - x;
                    await lotRepository.UpdateLot(listeTriee[i].Id, listeTriee[i]);
                }
            }


            return listeLots;
        }*/

        /* public async Task<List<(Lot, int)>> VenteQuantite(int ProductId, int QuantiteSaisie)
         {
             int x = QuantiteSaisie;
             var listeTriee = (await lotRepository.GetLotsByProductId(ProductId))
                              .OrderBy(l => l.Date)
                              .ToList();

             List<Lot> lotsToUpdate = new List<Lot>();
             List<(Lot, int)> listeLots = new List<(Lot, int)>();

             foreach (var lot in listeTriee)
             {
                 if (x <= 0) break;
                 if (lot.Quantite == 0) continue;

                 if (lot.Quantite < x)
                 {
                     listeLots.Add((lot, lot.Quantite));
                     x -= lot.Quantite;
                     lot.Quantite = 0;
                     lotsToUpdate.Add(lot);
                 }
                 else if (lot.Quantite == x)
                 {
                     listeLots.Add((lot, x));
                     lot.Quantite = 0;
                     lotsToUpdate.Add(lot);
                     break;
                 }
                 else // lot.Quantite > x
                 {
                     listeLots.Add((lot, x));
                     lot.Quantite -= x;
                     lotsToUpdate.Add(lot);
                     break;
                 }
             }

             // 🔥 UPDATE BATCH (1 seule fois)
             await lotRepository.UpdateLotsBatch(lotsToUpdate);

             return listeLots;
         }
         */

        /*public async Task<VenteResult> VenteQuantite(int productId, int quantiteSaisie)
        {
            // Initialisation du retour
            var result = new VenteResult();

            if (quantiteSaisie <= 0)
            {
                result.Message = "La quantité doit être supérieure à 0.";
                return result; // Success est false par défaut
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var listeTriee = await _dbContext.Lots
                    .Where(l => l.IDProduit == productId && l.Quantite > 0)
                    .OrderBy(l => l.Date)
                    .ToListAsync();

                int resteASoustraire = quantiteSaisie;

                foreach (var lot in listeTriee)
                {
                    if (resteASoustraire <= 0) break;

                    int montantAPrelever = Math.Min(lot.Quantite, resteASoustraire);

                    lot.Quantite -= montantAPrelever;
                    resteASoustraire -= montantAPrelever;

                    result.LotsImpactes.Add((lot, montantAPrelever));
                }

                // --- GESTION DU CAS "STOCK INSUFFISANT" ---
                if (resteASoustraire > 0)
                {
                    await transaction.RollbackAsync(); // On annule les modifs mémoire
                    result.Success = false;
                    result.Message = $"Stock insuffisant. Manquant : {resteASoustraire}";
                    result.LotsImpactes.Clear(); // On vide la liste car la vente ne se fait pas
                    return result;
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                result.Success = true;
                result.Message = "Vente effectuée avec succès.";
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Success = false;
                result.Message = $"Erreur technique : {ex.Message}";
                result.LotsImpactes.Clear();
                return result;
            }
        }
        */

        /*public async Task<VenteResult> VenteQuantite(int productId, int quantiteSaisie)
        {
            var result = new VenteResult();
            if (quantiteSaisie <= 0) return result;

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // 1. Tri STABLE (Date + Id) pour éviter les sauts de ligne aléatoires
                var listeLots = await _dbContext.Lots
                    .Where(l => l.IDProduit == productId && l.Quantite > 0)
                    .OrderBy(l => l.Date)
                    .ThenBy(l => l.Id)
                    .ToListAsync();

                int resteAPrelever = quantiteSaisie;
                Console.WriteLine($"--- DÉBUT VENTE : Besoin de {quantiteSaisie} unités ---");

                foreach (var lot in listeLots)
                {
                    // Vérification de sortie
                    if (resteAPrelever <= 0)
                    {
                        Console.WriteLine("-> Besoin comblé. Arrêt de la boucle.");
                        break;
                    }

                    int quantiteDisponible = lot.Quantite;
                    int montantAPrelever = Math.Min(quantiteDisponible, resteAPrelever);

                    Console.WriteLine($"Lot {lot.Reference} | Stock: {quantiteDisponible} | On prend: {montantAPrelever}");

                    // --- LA MISE À JOUR CRITIQUE ---
                    lot.Quantite -= montantAPrelever; // On réduit le stock du lot
                    resteAPrelever -= montantAPrelever; // ON RÉDUIT LE RESTE À SOUSTRAIRE

                    Console.WriteLine($"Reste à trouver après ce lot : {resteAPrelever}");

                    result.LotsImpactes.Add((lot, montantAPrelever));
                }

                if (resteAPrelever > 0)
                {
                    Console.WriteLine($"!!! ÉCHEC : Manque encore {resteAPrelever} unités.");
                    await transaction.RollbackAsync();
                    result.Success = false;
                    result.Message = "Stock insuffisant.";
                    return result;
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                result.Success = true;
                Console.WriteLine("--- TRANSACTION VALIDÉE ---");
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }
        }
        */

        public async Task<VenteResult> VenteQuantite(int productId, int quantiteSaisie)
        {
            var result = new VenteResult();
            if (quantiteSaisie <= 0) return result;

            // Suppression du BeginTransaction ici
            try
            {
                var listeLots = await _dbContext.Lots
                    .Where(l => l.IDProduit == productId && l.Quantite > 0)
                    .OrderBy(l => l.Date)
                    .ThenBy(l => l.Id)
                    .ToListAsync();

                int resteAPrelever = quantiteSaisie;

                foreach (var lot in listeLots)
                {
                    if (resteAPrelever <= 0) break;

                    int montantAPrelever = Math.Min(lot.Quantite, resteAPrelever);
                    lot.Quantite -= montantAPrelever;
                    resteAPrelever -= montantAPrelever;

                    result.LotsImpactes.Add((lot, montantAPrelever));
                }

                if (resteAPrelever > 0)
                {
                    result.Success = false;
                    result.Message = "Stock insuffisant.";
                    return result;
                }

                // On sauvegarde les changements dans le context partagé
                await _dbContext.SaveChangesAsync();
                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<(bool Success, int LotId, string Message)> AchatQuantite(LotCreateDTO lotCreateDTO)
        {
            try
            {
                // Vérifier si la référence existe déjà
                Lot? existingLot = await lotRepository.GetLotByReference(lotCreateDTO.Reference);
                if (existingLot != null)
                {
                    existingLot.Quantite += lotCreateDTO.Quantite;

                    bool updated = await lotRepository.UpdateLot(existingLot.Id, existingLot);

                    if (!updated)
                        return (false, 0, "Impossible de mettre à jour ce lot.");

                    await inventaireProduitService.UpdateQuantiteProduit(existingLot.IDProduit);
                    

                    return (true, existingLot.Id, "Lot réapprovisionné avec succès.");
                }

                // Création d'un nouveau lot
                Lot newLot = lotCreateDTO.ToLotEntity();
                int lotId = await lotRepository.AddLot(newLot);

                if (lotId <= 0)
                    return (false, 0, "Erreur lors de la création du lot.");

                await inventaireProduitService.UpdateQuantiteProduit(newLot.IDProduit);

                return (true, lotId, "Lot créé avec succès.");
            }
            catch (Exception ex)
            {
                return (false, 0, $"Erreur : {ex.Message}");
            }
        }

        public async Task<bool> ReplenishLotsWithOldQuantity(string RefLot, int Quantite)
        {
            Lot? existingLot = await lotRepository.GetLotByReference(RefLot);
            if (existingLot != null)
            {
                existingLot.Quantite = existingLot.Quantite + Quantite;
                await lotRepository.UpdateLot(existingLot.Id, existingLot);
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteLot(int id)
        {
            // Ajouter la logique pour supprimer le lot de la base de données
            return await lotRepository.DeleteLot(id);
        }
    }
}
