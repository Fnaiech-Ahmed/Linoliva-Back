using tech_software_engineer_consultant_int_backend.Models;
using System.Collections.Generic;
using tech_software_engineer_consultant_int_backend.Repositories;
using tech_software_engineer_consultant_int_backend.DTO.InventaireProduitDTOs;
using Microsoft.IdentityModel.Tokens;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class InventaireProduitService : IInventaireProduitService
    {
        private readonly IInventaireProduitRepository inventaireProduitRepository;

        public InventaireProduitService(IInventaireProduitRepository repository)
        {
            inventaireProduitRepository = repository;
        }

        public async Task<bool> AddInventaireProduit(InventaireProduitCreateDTO NewInventaireProduitCreateDTO)
        {
            InventaireProduit newInventaireProduit = NewInventaireProduitCreateDTO.ToInventaireProduitEntity();
            return await inventaireProduitRepository.AddInventaireProduit(newInventaireProduit);
        }

        public async Task<List<InventaireProduitDTO>> GetInventaireProduits()
        {
            List<InventaireProduit> ListeInventaireProduits = await inventaireProduitRepository.GetInventaireProduits();

            if (ListeInventaireProduits.IsNullOrEmpty())
            {
                return new List<InventaireProduitDTO>();
            }
            else
            {
                List<InventaireProduitDTO> ListeInventaireProduitDTOs = new List<InventaireProduitDTO>();
                foreach(InventaireProduit item in ListeInventaireProduits)
                {
                    ListeInventaireProduitDTOs.Add(InventaireProduitDTO.FromInvetaireProduitEntity(item));
                }
                return ListeInventaireProduitDTOs;
            }
            
        }

        public async Task<InventaireProduitDTO> GetInventaireProduitById(int id)
        {
            InventaireProduit? inventaireProduit = await inventaireProduitRepository.GetInventaireProduitById(id);
            if (inventaireProduit == null)
            {
                return new InventaireProduitDTO();
            }else
            {
                return InventaireProduitDTO.FromInvetaireProduitEntity(inventaireProduit);
            }
            
        }


        public async Task<(bool, int)> VerifInventaireProduitByProductId(int ProductId)
        {
            InventaireProduit existingStockProduit = await inventaireProduitRepository.GetInventaireProduitByProductId(ProductId);
            if (existingStockProduit == null)
            {
                return (false, 0);
            }
            else { return (true, existingStockProduit.Quantity); }
        }

        public async Task<bool> UpdateInventaireProduit(int inventaireProduitId, InventaireProduitUpdateDTO inventaireProduitUpdateDTO)
        {
            InventaireProduit existingInventaireProduit = await inventaireProduitRepository.GetInventaireProduitById(inventaireProduitId);

            if (existingInventaireProduit != null)
            {
                InventaireProduit updatedInvetaireProduit = inventaireProduitUpdateDTO.ToInventaireProduitEntity();
                bool ResultatMJ = await inventaireProduitRepository.UpdateInventaireProduit(updatedInvetaireProduit);
                return ResultatMJ;
            }
            else
            {
                // Vous pouvez gérer le cas où l'inventaire produit n'existe pas ici.
                return false;
            }
        }

        public async Task<(bool,int)> ModifierQuantiteProduit(int inventaireProduitId, string NomProduit, int Quantite, TypeTransaction typeTransaction)
        {
            InventaireProduit existingInventaireProduit = await inventaireProduitRepository.GetInventaireProduitByProductId(inventaireProduitId);

            if (existingInventaireProduit != null)
            {
                if (typeTransaction == TypeTransaction.Achat)
                {
                    existingInventaireProduit.Quantity = Quantite + existingInventaireProduit.Quantity;
                    await inventaireProduitRepository.UpdateInventaireProduit(existingInventaireProduit);
                    return (true, existingInventaireProduit.Quantity);
                }
                else if (typeTransaction == TypeTransaction.Vente)
                {
                    if (existingInventaireProduit.Quantity > Quantite)
                    {
                        existingInventaireProduit.Quantity = existingInventaireProduit.Quantity - Quantite;
                        bool res = await inventaireProduitRepository.UpdateInventaireProduit(existingInventaireProduit);
                        if (res)
                            return (res, existingInventaireProduit.Quantity);
                        else
                            return (false, 0);
                    }
                    else if (existingInventaireProduit.Quantity == Quantite)
                    {
                        /*DialogResult result = MessageBox.Show("La Quantité restante dans le Stock du Produit: " + existingInventaireProduit.ProductName + 
                            " égale à la quantité demandée. Voulez-vous continuer la transaction ?", 
                            "Confirmation",
                            MessageBoxButtons.YesNo);
                        if(result == DialogResult.Yes)
                        {*/
                            existingInventaireProduit.Quantity = existingInventaireProduit.Quantity - Quantite;
                            await inventaireProduitRepository.UpdateInventaireProduit(existingInventaireProduit);
                            return (true, existingInventaireProduit.Quantity);
                        /*}
                        else
                        {
                            return (false, existingInventaireProduit.Quantity);
                        }*/
                    }
                    else
                    {
                        return (false, existingInventaireProduit.Quantity);
                    }
                    
                }
                else if (typeTransaction == TypeTransaction.RetraitAchat)
                {
                    existingInventaireProduit.Quantity = existingInventaireProduit.Quantity - Quantite;
                    await inventaireProduitRepository.UpdateInventaireProduit(existingInventaireProduit);
                    return (true, existingInventaireProduit.Quantity);
                }
                else if (typeTransaction == TypeTransaction.RetraitVente)
                {
                    existingInventaireProduit.Quantity = existingInventaireProduit.Quantity + Quantite;
                    bool resUpdateQuantityProduct = await inventaireProduitRepository.UpdateInventaireProduit(existingInventaireProduit);
                    if (resUpdateQuantityProduct)
                    {
                        return (true, existingInventaireProduit.Quantity);
                    }
                    else
                    {
                        return (false, 0);
                    }
                }
                else
                {
                    // Vous pouvez gérer le cas où l'opération est erronée.
                    return (false, 0);
                }
            }
            else
            {
                if (typeTransaction == TypeTransaction.Achat)
                {
                    // Vous pouvez gérer le cas où l'inventaire produit n'existe pas ici.
                    InventaireProduitCreateDTO inventaireProduit = new InventaireProduitCreateDTO();
                    inventaireProduit.ProductId = inventaireProduitId;
                    inventaireProduit.ProductName = NomProduit;
                    inventaireProduit.Quantity = Quantite;
                    bool resultatAjoutNewIP = await this.AddInventaireProduit(inventaireProduit);
                    if (resultatAjoutNewIP)
                    {
                        return (true, Quantite);
                    }
                    else { 
                        return (false, 0); 
                    }
                    
                }
                
                return (false, 0);
            }
        }

        public async Task<bool> DeleteInventaireProduit(int id)
        {
            bool ResultatSuppressionIP = await inventaireProduitRepository.DeleteInventaireProduit(id);
            return ResultatSuppressionIP;
        }
    }
}
