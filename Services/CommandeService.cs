//using iText.StyledXmlParser.Jsoup.Nodes;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.DTO.TransactionsDTOs;
using tech_software_engineer_consultant_int_backend.Repositories;
using System;
using System.Collections.Generic;
using tech_software_engineer_consultant_int_backend.DTO.CommandesDTOs;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Core.Types;
using tech_software_engineer_consultant_int_backend.DTO.BLDTO;



namespace tech_software_engineer_consultant_int_backend.Services
{
    public class CommandeService : ICommandeService
    {
        private readonly ICommandeRepository commandeRepository;
        private readonly ISequenceRepository<Sequence> sequenceRepository;
        private ITransactionService transactionService;

        public CommandeService(ICommandeRepository repository, ITransactionService _transactionService, ISequenceRepository<Sequence> _sequenceRepository)
        {
            commandeRepository = repository;
            transactionService = _transactionService;
            sequenceRepository = _sequenceRepository;
        }

        public async Task<bool> AddCommande(CommandeCreateDTO commandeCreateDTO, List<TransactionCreateDto> ListTransactionsCreateDTOs)
        {
            List<Transactions> ListTransactions = new List<Transactions>();
            Commande commande = commandeCreateDTO.ToCommandeEntity();
            commande.ReferenceCommande = await this.GenerateNextRef();

            foreach(TransactionCreateDto item in ListTransactionsCreateDTOs)
            {
                Transactions newTransaction = item.ToTransactionsEntity();
                (bool, int) resultAjoutTransaction = await transactionService.AddTransaction(item, commande.ReferenceCommande);                

                if (resultAjoutTransaction.Item1 == false)
                {
                    // Si l'ajout de la transaction échoue, on ignore cette transaction
                    // à ajouter une étape d'annulation des transactions déjà ajoutées si nécessaire svp !!! 
                    continue;
                }
                else
                {
                    ListTransactions.Add(item.ToTransactionsEntity());
                }
            }
            
            for (int i = 0; i < ListTransactions.Count; i++)
            {
                ListTransactions[i].ReferenceCommande = commande.ReferenceCommande;
                // Ajouter la Transaction au Système
               // await transactionService.AddTransaction(ListTransactionsCreateDTOs[i]);
            }

            List<TransactionsDTO> newListTransactionsDTOs = await transactionService.GetTransactionsByRefCommande(commande.ReferenceCommande);
            List<Transactions> newListTransactions = new List<Transactions>();
            foreach(TransactionsDTO transactionDTO in newListTransactionsDTOs)
            {
                ListTransactions.Add(transactionDTO.ToTransactionsEntity());
            }

            List<int> NewListIdsTransactions = new List<int>();
            for (int i = 0;i < newListTransactions.Count;i++)
            {
                NewListIdsTransactions.Add(newListTransactions[i].Id);
            }
            commande.ListIdsTransactions = NewListIdsTransactions;

            commande.MontantTotalTTC = await this.CalculerMontantTotalTTC(commande);
            commande.MontantTotalHT = await this.CalculerMontantTotalHT(commande);

            bool resultatAjoutCommande = await commandeRepository.AddCommande(commande);

            return resultatAjoutCommande;
        }

        public async Task<string> GenerateNextRef()
        {
            var sequence = sequenceRepository.GetSequenceByName("Commande");

            if (sequence == null)
            {
                sequence = new Sequence { Name = "Commande", NextValue = 1 };

                await sequenceRepository.AddSequence(sequence);
            }
            else
            {
                sequence.NextValue++;

                await sequenceRepository.UpdateSequence(sequence);
            }

            return $"C-TSE-C-INT-{sequence.NextValue:D5}";
        }




        public async Task<List<CommandeDTO>> GetCommandes()
        {
            List<Commande> ListCommandes = await commandeRepository.GetAllCommandes();

            if (ListCommandes.IsNullOrEmpty())
            {
                return new List<CommandeDTO>();
            }
            else
            {
                List<CommandeDTO> ListCommandesDTOs = new List<CommandeDTO>();
                foreach(Commande commande in ListCommandes)
                {
                    CommandeDTO commandeDTO = CommandeDTO.FromCommandeEntity(commande);
                    ListCommandesDTOs.Add(commandeDTO);
                }
                return ListCommandesDTOs;
            }
            
        }

        public async Task<List<CommandeDTO>> GetListeCommandeByOwner(string OwnerId)
        {
            List<Commande> commandes = await commandeRepository.GetListeCommandesByOwner(OwnerId);

            List<CommandeDTO> commandeDTOs = commandes.Select(commande => CommandeDTO.FromCommandeEntity(commande)).ToList();
            return commandeDTOs;
        }




        public async Task<CommandeDTO?> GetCommandeById(int id) 
        {
            Commande? existingCommande = await commandeRepository.GetCommandeById(id);
            if (existingCommande == null) {
                return new CommandeDTO();
            } else
            {
                return CommandeDTO.FromCommandeEntity(existingCommande);
            }            
        }
        public async Task<CommandeDTO?> GetCommandeByRef(string RefCommande)
        {
            Commande? existingCommande = await commandeRepository.GetCommandeByRef(RefCommande);
            if (existingCommande == null)
            {
                return new CommandeDTO();
            }
            return CommandeDTO.FromCommandeEntity(existingCommande);
        }

        public async Task<(decimal, decimal)> GetTotalCommandeById(int id)
        {
            Commande? existingCommande = await commandeRepository.GetCommandeById(id);

            if (existingCommande != null)
            {
                decimal Total_HT = 0;
                decimal Total_TTC = 0;

                List<TransactionsDTO> ListeTransactionsDTOs = await transactionService.GetTransactionsByRefCommande(existingCommande.ReferenceCommande);//existingCommande.Transactions;
                List<Transactions> Transactions = new List<Transactions>();
                foreach(TransactionsDTO transactionsDTO in ListeTransactionsDTOs)
                {
                    Transactions.Add(transactionsDTO.ToTransactionsEntity());
                }

                if (Transactions != null)
                {
                    var productList = Transactions.Select(t => new
                    {
                        Id = t.ProductId,
                        Name = t.NomProduit,
                        Price = t.PrixUnitaire,
                        Quantity = t.QuantityEntered,
                        TVA = t.TVA
                    }).ToList();

                    foreach (var transaction in productList)
                    {
                        Total_HT += transaction.Quantity * transaction.Price;
                        Total_TTC += transaction.Quantity * transaction.Price + transaction.Quantity * transaction.Price * transaction.TVA /100;
                    }
                    
                    return (Total_HT,Total_TTC);
                }
                else return (0, 0);
            }

            return (0,0);
            
        }





        public async Task<decimal> CalculerMontantTotalTTC(Commande commande)
        {
            if (commande != null)
            {
                decimal AmountTotal_TTC = 0;

                List<TransactionsDTO> TransactionsDTOs = await transactionService.GetTransactionsByRefCommande(commande.ReferenceCommande);//commande.Transactions;
                List<Transactions> Transactions = new List<Transactions>();
                foreach(TransactionsDTO transactionDTO in TransactionsDTOs)
                {
                    Transactions.Add(transactionDTO.ToTransactionsEntity());
                }

                if (Transactions != null)
                {
                    var TransactionsAmountsTTCList = Transactions.Select(t => t.PTTC).ToList();

                    foreach (var amountTTC in TransactionsAmountsTTCList)
                    {
                        AmountTotal_TTC = AmountTotal_TTC + amountTTC;
                    }

                    return AmountTotal_TTC; // Retourne le montant total TTC calculé
                }
                else
                {
                    return 0; // Aucune transaction, retourne zéro
                }
            }

            return 0; // Commande nulle, retourne zéro
        }

        public async Task<decimal> CalculerMontantTotalHT(Commande commande)
        {
            if (commande != null)
            {
                decimal AmountTotal_HT = 0;

                List<TransactionsDTO> TransactionsDTOs = await transactionService.GetTransactionsByRefCommande(commande.ReferenceCommande); //commande.Transactions;
                List<Transactions> Transactions = new List<Transactions>();
                foreach (TransactionsDTO trans in TransactionsDTOs)
                {
                    Transactions.Add(trans.ToTransactionsEntity());
                }

                if (Transactions != null)
                {
                    var TransactionsAmountsTTCList = Transactions.Select(t => (t.QuantityEntered, t.PrixUnitaire)).ToList();

                    foreach ( (var _QuantityEntered, var _PrixUnitaire) in TransactionsAmountsTTCList)
                    {
                        AmountTotal_HT = AmountTotal_HT + (_QuantityEntered * _PrixUnitaire);
                    }

                    return AmountTotal_HT; // Retourne le montant total HT calculé
                }
                else
                {
                    return 0; // Aucune transaction, retourne zéro
                }
            }

            return 0; // Commande nulle, retourne zéro
        }




        /* public async Task<(bool, string)> UpdateCommande(int commandeId, CommandeUpdateDTO commandeUpdateDTO, List<Transactions> NewListTransactions)
         {
             Commande? existingCommande = await commandeRepository.GetCommandeById(commandeId);

             if (existingCommande != null)
             {
                 Commande commande = commandeUpdateDTO.ToCommandeEntity();
                 List<TransactionsDTO> existingTransactionsCommandeDTOs = await transactionService.GetTransactionsByRefCommande(existingCommande.ReferenceCommande);
                 List<Transactions> existingTransactionsCommande = new List<Transactions>();
                 foreach (TransactionsDTO transactionDTO in existingTransactionsCommandeDTOs)
                 {
                     existingTransactionsCommande.Add(transactionDTO.ToTransactionsEntity());
                 }

                 for (int i = 0; i < NewListTransactions.Count; i++)
                 {
                     //commande.Transactions[i].ReferenceCommande = commande.ReferenceCommande;
                     if (transactionService.GetTransactionById(NewListTransactions[i].Id) == null)
                     {
                         TransactionCreateDto NewTransactionCreateDto = new TransactionCreateDto(NewListTransactions[i]);
                         await transactionService.AddTransaction(NewTransactionCreateDto, commande.ReferenceCommande);
                     }
                     else
                     {
                         TransactionsUpdateDTO NewTransactionUpdateDTO = TransactionsUpdateDTO.FromTransactionsEntity(NewListTransactions[i]);
                         await transactionService.UpdateTransaction(NewListTransactions[i].Id, NewTransactionUpdateDTO);
                     }
                 }

                 // Vérifier s'il y a une transaction supprimée de la commande lors de la demande de sa mise à jour
                 // et la supprimer de la BD
                 foreach (var existingTransaction in existingTransactionsCommande)
                 {
                     if (existanceByIdTransactionCommande(existingTransaction, NewListTransactions) == false)
                     {
                         await transactionService.DeleteTransaction(existingTransaction.Id);
                     }
                 }

                 List<TransactionsDTO> ListeTransactionsTrouveesDTOs = await transactionService.GetTransactionsByRefCommande(commande.ReferenceCommande);
                 List<Transactions> ListeTransactionsTrouvees = new List<Transactions>();
                 foreach (TransactionsDTO transactionDTO in ListeTransactionsTrouveesDTOs)
                 {
                     ListeTransactionsTrouvees.Add(transactionDTO.ToTransactionsEntity());
                 }
                 //commande.ListIdsTransactions.Clear();
                 commande.ListIdsTransactions = new List<int>();
                 for (int i = 0; i < ListeTransactionsTrouvees.Count; i++)
                 {
                     commande.ListIdsTransactions.Add(ListeTransactionsTrouvees[i].Id);
                 }

                 commande.MontantTotalTTC = await this.CalculerMontantTotalTTC(commande);
                 commande.MontantTotalHT = await this.CalculerMontantTotalHT(commande);

                 //return await commandeRepository.UpdateCommande(commande);
                 var result = await commandeRepository.UpdateCommande(commande);

                 if (result.IsSuccess)
                 {
                     Console.WriteLine(result.Message);
                     return (true, result.Message);
                 }
                 else
                 {
                     Console.WriteLine("Erreur : " + result.Message);
                     return (false, result.Message);
                 }

             }
             else
             {
                 // Vous pouvez gérer le cas où la commande n'existe pas ici.
                 return (false, "Aucune Commande trouvée.. Veuillez contacter le service administration ou technique. Merci. ");
             }
         }

         */



        public async Task<(bool, string)> UpdateCommande(
    int commandeId,
    CommandeUpdateDTO commandeUpdateDTO,
    List<Transactions> newTransactions)
        {
            try
            {
                // 1️⃣ Récupération de la commande existante
                var existingCommande = await commandeRepository.GetCommandeById(commandeId);
                if (existingCommande == null)
                    return (false, $"Commande {commandeId} introuvable.");

                // 2️⃣ Création de la nouvelle instance via le DTO
                var commande = commandeUpdateDTO.ToCommandeEntity();
                commande.Id = existingCommande.Id;
                commande.ReferenceCommande = existingCommande.ReferenceCommande;
                commande.ListIdsTransactions = new List<int>();

                // 3️⃣ Détacher l’ancienne instance pour éviter conflit EF Core
                commandeRepository.Detach(existingCommande);

                // 4️⃣ Récupérer toutes les transactions existantes de la commande en un seul appel
                var existingTransactions = (await transactionService
                        .GetTransactionsByRefCommande(existingCommande.ReferenceCommande))
                        .Select(t => t.ToTransactionsEntity())
                        .ToList();

                // 5️⃣ Créer des dictionnaires pour comparaison rapide
                var existingDict = existingTransactions.ToDictionary(t => t.Id, t => t);
                var newDict = newTransactions.ToDictionary(t => t.Id, t => t);

                // 6️⃣ Transactions à ajouter / mettre à jour
                var tasks = new List<Task>();
                foreach (var tr in newTransactions)
                {
                    if (tr.Id == 0 || !existingDict.ContainsKey(tr.Id))
                    {
                        // Nouvelle transaction
                        var createDto = new TransactionCreateDto(tr);
                        tasks.Add(transactionService.AddTransaction(createDto, commande.ReferenceCommande));
                    }
                    else
                    {
                        // Mise à jour
                        var updateDto = TransactionsUpdateDTO.FromTransactionsEntity(tr);
                        tasks.Add(transactionService.UpdateTransaction(tr.Id, updateDto));
                    }
                }

                await Task.WhenAll(tasks);

                // 7️⃣ Transactions à supprimer
                var toDelete = existingTransactions
                    .Where(t => !newDict.ContainsKey(t.Id))
                    .ToList();

                foreach (var tr in toDelete)
                    await transactionService.DeleteTransaction(tr.Id);

                // 8️⃣ Mise à jour de la liste des IDs
                var updatedTransactions = (await transactionService
                        .GetTransactionsByRefCommande(commande.ReferenceCommande))
                        .Select(t => t.Id)
                        .ToList();

                commande.ListIdsTransactions = updatedTransactions;

                // 9️⃣ Recalcul des montants
                commande.MontantTotalTTC = await CalculerMontantTotalTTC(commande);
                commande.MontantTotalHT = await CalculerMontantTotalHT(commande);

                // 🔟 Update final dans la BD
                var result = await commandeRepository.UpdateCommande(commande);

                if (result.IsSuccess)
                {
                    Console.WriteLine($"Commande {commande.ReferenceCommande} mise à jour avec succès.");
                    return (true, result.Message);
                }
                else
                {
                    Console.WriteLine($"Erreur mise à jour commande : {result.Message}");
                    return (false, result.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception lors de la mise à jour de la commande : {ex.Message}");
                return (false, "Erreur interne. Veuillez contacter le support.");
            }
        }



        public bool existanceByIdTransactionCommande (Transactions transaction, List<Transactions> NewListTransactions)
        {
            foreach (Transactions newTransaction in NewListTransactions)
            {
                if (transaction.Id == newTransaction.Id)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<(bool,string)> DeleteCommande(int id)
        {
            Commande? existingCommande = await commandeRepository.GetCommandeById(id);

            if (existingCommande != null)
            {
                bool resultatDeleteTransCommande = true;
                for (int i = 0; i< existingCommande.ListIdsTransactions.Count; i++)
                {
                    bool resultatDeleteTransaction = await transactionService.DeleteTransaction(existingCommande.ListIdsTransactions[i]);
                    resultatDeleteTransCommande = resultatDeleteTransaction && resultatDeleteTransCommande;
                }
                
                bool resultatDeleteCommande = await commandeRepository.DeleteCommande(id);
                if (resultatDeleteCommande && resultatDeleteTransCommande)
                {
                    return (resultatDeleteCommande && resultatDeleteTransCommande, "Commande et ses transactions supprimées avec succès.");
                }
                else
                {
                    return (false, "Problème survenu lors de la suppression.");
                }
                
            }
            else
            {
                return (true,"Commande non trouvée avec cet ID.");
            }
        }
    }
}
