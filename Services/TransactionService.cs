using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Services;
using System.Collections.Generic;
using System.Transactions;
//using tech_software_engineer_consultant_int_backend.DTOs.ProductsDTOs;
using tech_software_engineer_consultant_int_backend.Repositories;
using tech_software_engineer_consultant_int_backend.DTO.TransactionsDTOs;
using tech_software_engineer_consultant_int_backend.DTO.LotsTransactionsDTOs;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IInventaireProduitService inventaireProduitService;
        private readonly ILotService lotService;
        private readonly ILotsTransactionsService lotsTransactionsService;

        public TransactionService(ITransactionRepository _transactionRepositoryrepository, IInventaireProduitService _inventaireService, ILotService _lotService, ILotsTransactionsService _lotsTransactionsService)
        {
            transactionRepository = _transactionRepositoryrepository;
            inventaireProduitService = _inventaireService;
            lotService = _lotService;
            lotsTransactionsService = _lotsTransactionsService;
        }

        public async Task<(bool, int)> AddTransaction(TransactionCreateDto transactionCreateDto, string ReferenceCommande)
        {
            Transactions transaction = transactionCreateDto.ToTransactionsEntity();
            if (ReferenceCommande != null)
            {
                transaction.ReferenceCommande = ReferenceCommande;
            }
            else
            {
                transaction.ReferenceCommande = "à vérifier svp";
            }
            
            decimal amountHT = transaction.QuantityEntered * transaction.PrixUnitaire;
            decimal amountTTC = amountHT + (amountHT * transaction.TVA / 100);
            //MessageBox.Show("Stade de l'ajout de la transaction: " + amountTTC.ToString());

            transaction.PTHT = amountHT;
            transaction.PTTC = amountTTC;


            if (transaction.TypeTransaction == TypeTransaction.Vente)
            {
                transaction.ReferenceLot = "Néant";
            }

            // Ajouter la transaction dans la BD
            (bool resultatBool, int IdTransaction) resultatAjoutTransaction = await transactionRepository.AddTransaction(transaction);
            
            //MessageBox.Show("Id Transaction: " + IdTransaction);

            // Modifier la Qauntité restante du Produit X dans l'inventaire
            (bool, int) resultat = await inventaireProduitService.ModifierQuantiteProduit(transaction.ProductId, transaction.NomProduit,transaction.QuantityEntered, transaction.TypeTransaction);          

            if (resultat.Item1)
            {
                // Modifier la valeur de la Quantité Restante du produit dans la table des Transactions
                // resultat.Item2 reçoit la valeur de newRemainingQuantity
                await transactionRepository.UpdateTransactionRemainingQuantity(transaction.ProductId, resultat.Item2);

                // Dans le cas d'une transaction de vente:
                // Nous sélectionnons les lots d'ou nous allons prendre la quantité nécessaire
                // et faire un enregistrement des LotsTransactions dans BD 
                if (transaction.TypeTransaction == TypeTransaction.Vente)
                {
                    List<(Lot, int)> resultatVenteQuantiteLot = await lotService.VenteQuantite(transaction.ProductId, transaction.QuantityEntered);
                    for (int i = 0; i< resultatVenteQuantiteLot.Count; i++)
                    {
                        //Enregistrement du LotTransaction dans BD 
                        LotsTransactions newLotsTransactions = new LotsTransactions
                        {
                            IdTransaction = resultatAjoutTransaction.IdTransaction.ToString(),
                            RefLot = resultatVenteQuantiteLot[i].Item1.Reference,
                            Quantite = resultatVenteQuantiteLot[i].Item2
                        };
                        await lotsTransactionsService.AddLT(LotsTransactionsDTO.FromEntity(newLotsTransactions));
                    }

                    //MessageBox.Show(resultatVenteQuantiteLot.FirstOrDefault().Item1.Reference);

                    // Mettre à jour la référence du Lot                 
                    transaction.ReferenceLot = resultatVenteQuantiteLot.FirstOrDefault().Item1.Reference;
                    bool resUpdate = await UpdateRefLotTransaction(resultatAjoutTransaction.IdTransaction, transaction);
                    if (!resUpdate)
                    {
                        //MessageBox.Show("Alerte. Mise à jour échouée ! Id Transaction: " + IdTransaction.ToString());
                        return (false, 0);
                    }
                }
                
                return (true, resultatAjoutTransaction.IdTransaction);
            }
            else
            {
                //MessageBox.Show("Problème d'ajout de la transaction.");
                return (false, 0);
            }
        }

        public async Task<List<TransactionsDTO>> GetTransactionsByRefCommande(string ReferenceCommande)
        {
            List<Transactions> ListeTransactionsPrimaire = await transactionRepository.GetTransactionsByRefCommande(ReferenceCommande);

            if (ListeTransactionsPrimaire == null)
            {
                return new List<TransactionsDTO>();
            }

            List<TransactionsDTO> ListTransactions = new List<TransactionsDTO>();
            foreach (var item in ListeTransactionsPrimaire)
            {
                TransactionsDTO transactionDTO = new TransactionsDTO(item);
                ListTransactions.Add(transactionDTO);
            }

            return ListTransactions;
        }

        public async Task<TransactionsDTO> GetTransactionById(int id)
        {
            Transactions transaction = await transactionRepository.GetTransactionById(id);

            if (transaction == null)
            {
                return new TransactionsDTO();
            }

            TransactionsDTO ResultatTransactionDTO = new TransactionsDTO(transaction);

            return ResultatTransactionDTO;
        }

        public async Task<List<TransactionsDTO>> GetTransactions()
        {
            List<Transactions> transactionsList = await transactionRepository.GetTransactions();

            if (transactionsList == null)
            {
                return null;
            }
            else
            {
                List<TransactionsDTO> transactionsDTOList = new List<TransactionsDTO> ();
                foreach (Transactions transaction in transactionsList) 
                {
                    var transactionDTO = new TransactionsDTO(transaction);
                    transactionsDTOList.Add(transactionDTO);
                }
                return transactionsDTOList;

            }
            
        }

        public async Task<(bool, int)> UpdateTransaction(int transactionId, TransactionsUpdateDTO transactionUpdateDTO)
        {
            if(transactionId != 0)
            {
                Transactions existingTransaction = await transactionRepository.GetTransactionById(transactionId);
                
                if (existingTransaction != null)
                {
                    Transactions transaction = transactionUpdateDTO.ToTransactionsEntity();

                    if (transaction.TypeTransaction == TypeTransaction.Vente)
                    {
                        // ---> Phase A1: Réalimenter les lots et le Stock <---

                        // Phase A1.1: Réalimenter les lots par l'ancienne quantité
                        bool resultatsRealimentationsLots = await ReplenishLotsGroupWithOldQuantity(transactionId);


                        // Phase A1.2: Réalimenter le stock par l'ancienne quantité
                        (bool, int) resultatRealimentationStock = await inventaireProduitService.ModifierQuantiteProduit(
                                                                            existingTransaction.ProductId,          existingTransaction.NomProduit, 
                                                                            existingTransaction.QuantityEntered,    TypeTransaction.RetraitVente );


                        if (resultatsRealimentationsLots && resultatRealimentationStock.Item1)
                        {
                            // Phase A2: Supprimer les anciennes relations entre la transaction spécifiée et les lots
                            bool resultSuppressionLotsTransaction = await lotsTransactionsService.RemoveLotsTransactionByTransactionId(transactionId);

                            existingTransaction.ReferenceLot = "Néant";

                            if (resultSuppressionLotsTransaction)
                            {
                                // Phase A3: Modifier la Qauntité restante du Produit X dans l'inventaire
                                // selon la nouvelle Quantité Saisie
                                (bool, int) resultatUpdateInventaire = await inventaireProduitService.ModifierQuantiteProduit(
                                        transaction.ProductId, transaction.NomProduit, 
                                        transaction.QuantityEntered, transaction.TypeTransaction);


                                if (resultatUpdateInventaire.Item1)
                                {
                                    // Modifier la valeur de la Quantité Restante du produit dans la table des Transactions
                                    bool resUpdateTransactionRemainingQuantity = await transactionRepository.UpdateTransactionRemainingQuantity(transaction.ProductId, resultatUpdateInventaire.Item2);

                                    if (resUpdateTransactionRemainingQuantity)
                                    {
                                        // Dans ce cas: Une transaction de vente:
                                        // Nous sélectionnons les lots d'ou nous allons prendre la quantité nécessaire
                                        // et faire un enregistrement des LotsTransactions dans BD
                                        
                                        List<(Lot, int)> resultatVenteQuantiteLot = await lotService.VenteQuantite(transaction.ProductId, transaction.QuantityEntered);
                                        
                                        for (int i = 0; i < resultatVenteQuantiteLot.Count; i++)
                                        {
                                            //Enregistrement du LotTransaction dans BD
                                            LotsTransactions newLotsTransactions = new LotsTransactions
                                            {
                                                IdTransaction = transactionId.ToString(),
                                                RefLot = resultatVenteQuantiteLot[i].Item1.Reference,
                                                Quantite = resultatVenteQuantiteLot[i].Item2
                                            };

                                            bool res = await lotsTransactionsService.AddLT(LotsTransactionsDTO.FromEntity(newLotsTransactions));
                                            if (res)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                //MessageBox.Show("Un problème survenu au cours de l'enregistrement d'une quantité de la transaction!");
                                            }
                                        }

                                        // Mettre à jour la référence du Lot dans la Transaction
                                        if (resultatVenteQuantiteLot != null)
                                        {
                                            existingTransaction.ReferenceLot = resultatVenteQuantiteLot[0].Item1.Reference.ToString();
                                        }
                                        else
                                        {
                                            //MessageBox.Show("La liste des Lots sélectionés est vide. Pas de lot à affecter à la transaction", "Alerte");
                                        }
                                        
                                        
                                        //Mettre à jour la référence du Lot de la Transaction dans la BD
                                        bool resUpdate = await UpdateRefLotTransaction(transactionId, existingTransaction);
                                        if (!resUpdate)
                                        {
                                            //MessageBox.Show("Alerte. Mise à jour échouée ! Id Transaction: " + transactionId.ToString());

                                        }


                                        decimal amountHT = transaction.QuantityEntered * transaction.PrixUnitaire;
                                        decimal amountTTC = amountHT + (amountHT * transaction.TVA / 100);

                                        existingTransaction.PTHT = amountHT;
                                        existingTransaction.PTTC = amountTTC;

                                        existingTransaction.TVA = transaction.TVA;

                                        existingTransaction.QuantityEntered = transaction.QuantityEntered;
                                        existingTransaction.RemainingQuantity= resultatUpdateInventaire.Item2;
                                        
                                        // Enregistrer la transaction dans la BD
                                        transactionRepository.UpdateTransaction(existingTransaction);
                                        
                                    }
                                    else
                                    {
                                        return (false, 0);
                                    }

                                }

                                return (false, 0);                                
                            }
                        }                       

                    }
                    else if (transaction.TypeTransaction == TypeTransaction.Achat)
                    {
                        existingTransaction.ReferenceLot = transaction.ReferenceLot;
                        existingTransaction.ProductId = transaction.ProductId;
                        existingTransaction.NomProduit = transaction.NomProduit;
                        existingTransaction.QuantityEntered = transaction.QuantityEntered;


                        transactionRepository.UpdateTransaction(existingTransaction);
                        return (true, transactionId);
                    }

                    // Nous retournons False dans le cas ou la transaction n'est pas de type Vente ou achat
                    return (false, transactionId);
                }
                else
                {
                    // Vous pouvez gérer le cas où la transaction n'existe pas ici.                    
                    return (false, 0);
                }
            }
            else 
            {
                //MessageBox.Show("Cas d'ajout de Transaction");                
                return (false, 0);
            }
            
        }

        public async Task<bool> UpdateRefLotTransaction(int transactionId, Transactions transaction)
        {
            Transactions existingTransaction = await transactionRepository.GetTransactionById(transactionId);
            if (existingTransaction != null)
            {
                existingTransaction.ReferenceLot = transaction.ReferenceLot;
                bool ResultatUpdateRefLotTransaction = await transactionRepository.UpdateRefLotTransaction(existingTransaction.Id, transaction.ReferenceLot);
                return ResultatUpdateRefLotTransaction;
            }
            return false;
        }



        public async Task<bool> DeleteTransaction(int id)
        {
            Transactions existingtransaction = await transactionRepository.GetTransactionById(id);

            if (existingtransaction != null)
            {
                TypeTransaction transactionType = existingtransaction.TypeTransaction;
                if(transactionType == TypeTransaction.Achat)
                {                
                    (bool, int) resultat = await inventaireProduitService.ModifierQuantiteProduit(existingtransaction.ProductId, existingtransaction.NomProduit,existingtransaction.QuantityEntered, TypeTransaction.RetraitAchat);
                    if (resultat.Item1)
                    {
                        bool r3 = await transactionRepository.DeleteTransaction(id);

                        // Modifier la valeur de la Quantité Restante du produit dans la table des Transactions
                        bool r4 = await transactionRepository.UpdateTransactionRemainingQuantity(existingtransaction.ProductId, resultat.Item2);

                        // Phase A1.1: Réalimenter les lots par l'ancienne quantité
                        bool resultatsRealimentationsLots = await ReplenishLotsGroupWithOldQuantity(id);

                        return (r3 && r4 && resultatsRealimentationsLots);
                    }
                    else
                    {
                        //MessageBox.Show("Erreur de retrait de la quantité du stock.");
                        return false;
                    }
                }
                else if (existingtransaction.TypeTransaction == TypeTransaction.Vente) 
                {
                    (bool, int) resultat = await inventaireProduitService.ModifierQuantiteProduit(existingtransaction.ProductId, existingtransaction.NomProduit,existingtransaction.QuantityEntered, TypeTransaction.RetraitVente);
                    if (resultat.Item1)
                    {
                        bool r1 = await transactionRepository.DeleteTransaction(id);

                        // Modifier la valeur de la Quantité Restante du produit dans la table des Transactions
                        bool r2 = await transactionRepository.UpdateTransactionRemainingQuantity(existingtransaction.ProductId, resultat.Item2);

                        // Phase A1.1: Réalimenter les lots par l'ancienne quantité
                        bool resultatsRealimentationsLots = await ReplenishLotsGroupWithOldQuantity(id);

                        return (r1 && r2 && resultatsRealimentationsLots);
                    }
                    else
                    {
                        //MessageBox.Show("Erreur de retrait de la quantité du stock.");
                        return false;
                    }
                }
                else
                {
                    //MessageBox.Show("Erreur de retrait de la quantité du stock.");
                    return false;
                }
            
            }
            else
            {
                //MessageBox.Show("La transaction avec l'id: " +  id + " est introuvable.");
                return false;
            }
            
        }



        public async Task<bool> ReplenishLotsGroupWithOldQuantity(int transactionId)
        {
            List<LotsTransactionsDTO> lotsTransactionsDTOs = await lotsTransactionsService.GetLotsTransactionsByIdTransaction(transactionId);
            List<LotsTransactions> lotsTransactions = new List<LotsTransactions>();
            foreach (var transactionDTO in lotsTransactionsDTOs)
            {
                lotsTransactions.Add(transactionDTO.ToEntity());
            }
            bool resultatsRealimentationsLots = false;
            for (int i = 0; i < lotsTransactions.Count; i++)
            {
                bool resultatRealimentationLot = await lotService.ReplenishLotsWithOldQuantity(lotsTransactions[i].RefLot, lotsTransactions[i].Quantite);
                if (resultatRealimentationLot)
                {
                    resultatsRealimentationsLots = true;
                    continue;
                }
                else
                {
                    /*DialogResult dialogResult = MessageBox.Show("Le Lot avec la référence: " + lotsTransactions[i].RefLot +
                        " n'a pas pris sa quantité ! Annuler l'opération ?", "Annuler ?", MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.Yes)
                    {
                        resultatsRealimentationsLots = false;
                        for (int j = 0; j < i; j++)
                        {

                        }
                    }
                    else
                    {
                        continue;
                    }*/
                    continue;
                }
            }
            return resultatsRealimentationsLots;
        }
    }
}
