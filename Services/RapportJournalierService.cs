using tech_software_engineer_consultant_int_backend.DTO.RapportsJournaliersDTOs;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using tech_software_engineer_consultant_int_backend.DTO.RecettesDTOs;
using tech_software_engineer_consultant_int_backend.DTO.DepensesDTOs;
using NuGet.Protocol.Core.Types;
using Humanizer;
using tech_software_engineer_consultant_int_backend.DTO.ProductDTOs;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SignalR;
using tech_software_engineer_consultant_int_backend.Hubs;
using Twilio.TwiML.Fax;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class RapportJournalierService : IRapportJournalierService
    {
        private readonly IRapportJournalierRepository _rapportJournalierRepository;
        private readonly MyDbContext _myDbContext;
        //private readonly IHubContext<NotificationHub> _hubContext;

        public RapportJournalierService(
            IRapportJournalierRepository rapportJournalierRepository
            , MyDbContext myDbContext
            //, IHubContext<NotificationHub> hubContext
            )
        {
            _rapportJournalierRepository = rapportJournalierRepository;
            _myDbContext = myDbContext;
            //_hubContext = hubContext;
        }

        public async Task<RapportJournalier> GetRapportByIdAsync(int id)
        {
            return await _rapportJournalierRepository.GetRapportByIdAsync(id);
        }

        public async Task<string> CreateRapportAsync(RapportJournalierDto dto)
        {
            var rapport = new RapportJournalier
            {
                Date = dto.Date,
                Recettes = dto.Recettes.Select(r => new Recette
                {
                    Montant = r.Montant,
                    DateTransaction = dto.Date,
                    PosteRecetteId = r.PosteRecetteId
                }).ToList(),
                Depenses = dto.Depenses.Select(d => new Depense
                {
                    Montant = d.Montant,
                    DateTransaction = dto.Date,
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    UnitPrice = d.UnitPrice,
                    Quantity = d.Quantity,                    
                }).ToList()
            };

            string resultMessageAdd = await _rapportJournalierRepository.CreateRapportAsync(rapport);

            return resultMessageAdd;
        }


        public async Task<List<RapportJournalierDto>> GetAllRapportsAsync()
        {
            // Récupérer tous les rapports journaliers
            List<RapportJournalier> rapportJournaliers = await _rapportJournalierRepository.GetAllRapportsAsync();

            // Vérifier si la liste est nulle ou vide
            if (rapportJournaliers == null || rapportJournaliers.Count == 0)
            {
                return new List<RapportJournalierDto>(); // Retourner une liste vide si aucun rapport n'est trouvé
            }

            // Initialiser la liste des DTOs
            var rapportJournaliersDtos = new List<RapportJournalierDto>();

            // Mapper chaque RapportJournalier vers RapportJournalierDto
            foreach (var rapport in rapportJournaliers)
            {
                var dto = new RapportJournalierDto
                {
                    Id = rapport.Id,
                    Date = rapport.Date,
                    Recettes = rapport.Recettes.Select(r => RecetteDto.ToDto(r)).ToList(), // Transformation de List<Recette> en List<RecetteDto>
                    Depenses = rapport.Depenses.Select(d => DepenseDto.ToDto(d)).ToList()  // Transformation de List<Depense> en List<DepenseDto>
                };

                rapportJournaliersDtos.Add(dto);
            }

            return rapportJournaliersDtos; // Retourner la liste des DTOs
        }





        public async Task<(bool, string, RapportJournalier)> MettreAJourRapportAsync(RapportJournalierDto rapport)
        {
            // Vérifier si le rapport existe
            var rapportExistant = await _rapportJournalierRepository.GetRapportByDateAsync(rapport.Date);
            if (rapportExistant == null)
            {
                return (false, "Rapport introuvable.", new RapportJournalier());
            }

            // Commencer une transaction
            using (var transaction = await _myDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Supprimer les anciennes recettes et dépenses
                    await _rapportJournalierRepository.DeleteRecettesByRapportId(rapportExistant.Id);
                    await _rapportJournalierRepository.DeleteDepensesByRapportId(rapportExistant.Id);

                    // Mettre à jour les propriétés
                    rapportExistant.Date = rapport.Date; // Mettre à jour la date si nécessaire

                    rapportExistant.Recettes = rapport.Recettes.Select(r => new Recette
                    {
                        Montant = r.Montant,
                        DateTransaction = rapport.Date,
                        PosteRecetteId = r.PosteRecetteId
                    }).ToList();

                    rapportExistant.Depenses = rapport.Depenses.Select(d => new Depense
                    {
                        Montant = d.Montant,
                        DateTransaction = rapport.Date,
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        UnitPrice = d.UnitPrice,
                        Quantity = d.Quantity,
                    }).ToList();

                    // Mettre à jour l'entité dans la base de données
                    (bool, RapportJournalier) updateResult = await _rapportJournalierRepository.UpdateRapport(rapportExistant);

                    if (!updateResult.Item1)
                    {
                        throw new Exception("Erreur lors de la mise à jour du rapport.");
                    }

                    // Commit de la transaction
                    await transaction.CommitAsync();

                    return (true, "Rapport mis à jour avec succès.", updateResult.Item2);
                }
                catch (Exception ex)
                {
                    // Rollback de la transaction en cas d'erreur
                    await transaction.RollbackAsync();
                    return (false, ex.Message, new RapportJournalier());
                }
            }
        }





        public async Task<List<ProductYearKey>> GetDepensesParProduitParAnnee(DateTime startDate, DateTime endDate, List<string> ListNamesProducts)
        {
            // Vérifiez que startDate est antérieure à endDate
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }

            List<Depense> depenses = new List<Depense>();

            if (ListNamesProducts.IsNullOrEmpty())
            {
                throw new ArgumentException("La Liste des produits sélectionnés est vide.");
            }
            else if (ListNamesProducts[0] == "all")
            {
                // Récupérer les dépenses dans la plage de dates
                depenses = await _rapportJournalierRepository.GetDepensesWithinDateRange(startDate, endDate);
            }
            else
            {
                foreach (string NameProduct in ListNamesProducts)
                {
                    if (NameProduct.IsNullOrEmpty())
                    {
                        continue;
                    }

                    var depensesSingleProduct = await _rapportJournalierRepository.GetDepensesSingleProductWithinDateRange(startDate, endDate, NameProduct);

                    if (depensesSingleProduct != null)
                    {
                        depenses.AddRange(depensesSingleProduct);
                    }
                }
            }

            //Déclarer une nouvelle liste de ProductKeys
            List<ProductYearKey> productYearKeys = new List<ProductYearKey>();

            // Traitement des dépenses pour calculer par produit et par année
            foreach (var depense in depenses)
            {
                // Calculer le numéro de l'année
                int year = (depense.DateTransaction.Year - startDate.Year) + 1; // +1 pour avoir l'année 1

                bool existanceProduct = productYearKeys.Any(p => p.ProductName.Equals(depense.ProductName, StringComparison.OrdinalIgnoreCase));

                if (existanceProduct)
                {
                    // Si le produit existe, récupérer l'instance correspondante
                    var productYearKey = productYearKeys.First(p => p.ProductName.Equals(depense.ProductName, StringComparison.OrdinalIgnoreCase));

                    // Vérifier si l'annee existe déjà dans le dictionnaire
                    var yearMontants = productYearKey.YearsMontants.FirstOrDefault(w => w.ContainsKey(year));

                    if (yearMontants != null)
                    {
                        // Si l'année existe, additionner le montant à celui déjà présent
                        yearMontants[year] += depense.Montant;
                    }
                    else
                    {
                        // Si l'année n'existe pas, créer un nouvel enregistrement pour cette année
                        productYearKey.YearsMontants.Add(new Dictionary<int, decimal> { { year, depense.Montant } });
                    }
                }
                else
                {
                    // Si le produit n'existe pas, créer un nouveau ProductWeekKey
                    var newProductYearKey = new ProductYearKey
                    {
                        ProductName = depense.ProductName,
                        YearsMontants = new List<Dictionary<int, decimal>>
                        {
                            new Dictionary<int, decimal> { { year, depense.Montant } }
                        }
                    };

                    // Ajouter le nouveau produit à la liste
                    productYearKeys.Add(newProductYearKey);
                }

            }

            // Retourner la liste contenant les résultats
            return productYearKeys;
        }


        public async Task<List<ProductMonthKey>> GetDepensesParProduitParMois(DateTime startDate, DateTime endDate, List<string> ListNamesProducts)
        {
            // Vérifiez que startDate est antérieure à endDate
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }

            List<Depense> depenses = new List<Depense>();

            if (ListNamesProducts.IsNullOrEmpty())
            {
                throw new ArgumentException("La Liste des produits sélectionnés est vide.");
            }
            else if (ListNamesProducts[0] == "all")
            {
                // Récupérer les dépenses dans la plage de dates
                depenses = await _rapportJournalierRepository.GetDepensesWithinDateRange(startDate, endDate);
            }
            else
            {
                foreach (string NameProduct in ListNamesProducts)
                {
                    if (string.IsNullOrEmpty(NameProduct))
                    {
                        continue;
                    }

                    var depensesSingleProduct = await _rapportJournalierRepository.GetDepensesSingleProductWithinDateRange(startDate, endDate, NameProduct);

                    if (depensesSingleProduct != null)
                    {
                        depenses.AddRange(depensesSingleProduct);
                    }
                }
            }

            // Déclarer une nouvelle liste de ProductMonthKeys
            List<ProductMonthKey> productMonthKeys = new List<ProductMonthKey>();

            // Traitement des dépenses pour calculer par produit et par mois
            foreach (var depense in depenses)
            {
                // Calculer le mois et l'année au format "Mois-Année"
                string monthKey = $"{depense.DateTransaction.Month}-{depense.DateTransaction.Year}"; // Format "Mois-Année"

                // Vérifier si le produit existe déjà
                bool existanceProduct = productMonthKeys.Any(p => p.ProductName.Equals(depense.ProductName, StringComparison.OrdinalIgnoreCase));

                if (existanceProduct)
                {
                    // Si le produit existe, récupérer l'instance correspondante
                    var productMonthKey = productMonthKeys.First(p => p.ProductName.Equals(depense.ProductName, StringComparison.OrdinalIgnoreCase));

                    // Vérifier si le mois existe déjà dans le dictionnaire
                    var monthMontants = productMonthKey.MonthsMontants.FirstOrDefault(w => w.ContainsKey(monthKey));

                    if (monthMontants != null)
                    {
                        // Si le mois existe, additionner le montant à celui déjà présent
                        monthMontants[monthKey] += depense.Montant;
                    }
                    else
                    {
                        // Si le mois n'existe pas, créer un nouvel enregistrement pour ce mois
                        productMonthKey.MonthsMontants.Add(new Dictionary<string, decimal> { { monthKey, depense.Montant } });
                    }
                }
                else
                {
                    // Si le produit n'existe pas, créer un nouveau ProductMonthKey
                    var newProductMonthKey = new ProductMonthKey
                    {
                        ProductName = depense.ProductName,
                        MonthsMontants = new List<Dictionary<string, decimal>>
                {
                    new Dictionary<string, decimal> { { monthKey, depense.Montant } }
                }
                    };

                    // Ajouter le nouveau produit à la liste
                    productMonthKeys.Add(newProductMonthKey);
                }
            }

            // Retourner la liste contenant les résultats
            return productMonthKeys;
        }



        public async Task<List<ProductWeekKey>> GetDepensesParProduitParSemaine(DateTime startDate, DateTime endDate, List<string> ListNamesProducts)
        {
            // Vérifiez que startDate est antérieure à endDate
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }

            List<Depense> depenses = new List<Depense>();

            if (ListNamesProducts.IsNullOrEmpty())
            {
                throw new ArgumentException("La Liste des produits sélectionnés est vide.");
            }
            else if (ListNamesProducts[0] == "all")
            {
                // Récupérer les dépenses dans la plage de dates
                depenses = await _rapportJournalierRepository.GetDepensesWithinDateRange(startDate, endDate);
            }
            else
            {
                foreach (string nameProduct in ListNamesProducts)
                {
                    // Vérifie si le nom du produit est null ou vide
                    if (string.IsNullOrEmpty(nameProduct))
                    {
                        continue;
                    }

                    // Récupère les dépenses pour le produit dans la plage de dates
                    var depensesSingleProduct = await _rapportJournalierRepository.GetDepensesSingleProductWithinDateRange(startDate, endDate, nameProduct);

                    // Vérifie si des dépenses ont été trouvées
                    if (depensesSingleProduct != null)
                    {
                        // Ajoute les dépenses à la liste existante
                        depenses.AddRange(depensesSingleProduct);

                        Console.WriteLine("Nouvelle dépense trouvée: " + depensesSingleProduct.ToString());
                        Console.WriteLine("Nouvelle taille de la liste des dépenses est : " + depenses.Count.ToString());
                    }
                }
            }

            // Déclarer une nouvelle liste de ProductKeys
            List<ProductWeekKey> productWeekKeys = new List<ProductWeekKey>();

            // Traitement des dépenses pour calculer par produit et par semaine
            foreach (var depense in depenses)
            {
                // Calculer le numéro de la semaine et l'année
                int weekOfYear = CalculRangSemaineServices.GetWeekOfYear(depense.DateTransaction);
                string weekKey = $"{weekOfYear}-{depense.DateTransaction.Year}"; // Format "Semaine-Année"

                // Vérifier si le produit existe déjà
                bool existanceProduct = productWeekKeys.Any(p => p.ProductName.Equals(depense.ProductName, StringComparison.OrdinalIgnoreCase));

                if (existanceProduct)
                {
                    // Si le produit existe, récupérer l'instance correspondante
                    var productWeekKey = productWeekKeys.First(p => p.ProductName.Equals(depense.ProductName, StringComparison.OrdinalIgnoreCase));

                    // Vérifier si la semaine existe déjà dans le dictionnaire
                    var weekMontants = productWeekKey.WeeksMontants.FirstOrDefault(w => w.ContainsKey(weekKey));

                    if (weekMontants != null)
                    {
                        // Si la semaine existe, additionner le montant à celui déjà présent
                        weekMontants[weekKey] += depense.Montant;
                    }
                    else
                    {
                        // Si la semaine n'existe pas, créer un nouvel enregistrement pour cette semaine
                        productWeekKey.WeeksMontants.Add(new Dictionary<string, decimal> { { weekKey, depense.Montant } });
                    }

                    Console.WriteLine("Ancien produit clé a été mis à jour dans la liste, sous le nom: " + productWeekKey.ProductName.ToString());
                }
                else
                {
                    // Si le produit n'existe pas, créer un nouveau ProductWeekKey
                    var newProductWeekKey = new ProductWeekKey
                    {
                        ProductName = depense.ProductName,
                        WeeksMontants = new List<Dictionary<string, decimal>>
                        {
                            new Dictionary<string, decimal> { { weekKey, depense.Montant } }
                        }
                    };

                    // Ajouter le nouveau produit à la liste
                    productWeekKeys.Add(newProductWeekKey);

                    Console.WriteLine("Nouveau produit clé a été ajouté dans la liste, sous le nom: " + newProductWeekKey.ProductName.ToString());
                }
            }

            // Retourner la liste contenant les résultats
            return productWeekKeys;
        }


        public async Task<List<ProductDayKey>> GetDepensesParProduitParJour(DateTime startDate, DateTime endDate, List<string> ListNamesProducts)
        {
            if (startDate > endDate)
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");

            if (ListNamesProducts.IsNullOrEmpty())
                throw new ArgumentException("La liste des produits sélectionnés est vide.");

            List<Depense> depenses;

            if (ListNamesProducts[0] == "all")
            {
                depenses = await _rapportJournalierRepository.GetDepensesWithinDateRange(startDate, endDate);
            }
            else
            {
                depenses = new List<Depense>();
                foreach (var name in ListNamesProducts.Where(n => !string.IsNullOrWhiteSpace(n)))
                {
                    var result = await _rapportJournalierRepository.GetDepensesSingleProductWithinDateRange(startDate, endDate, name);
                    if (result != null)
                        depenses.AddRange(result);
                }
            }

            // Regrouper les dépenses par produit et par date (chaîne yyyy-MM-dd)
            var grouped = depenses
                .GroupBy(d => d.ProductName)
                .Select(g => new ProductDayKey
                {
                    ProductName = g.Key,
                    DaysMontants = g
                        .GroupBy(d => d.DateTransaction.ToString("yyyy-MM-dd"))
                        .ToDictionary(
                            grp => grp.Key,
                            grp => grp.Sum(d => d.Montant)
                        )
                })
                .ToList();

            return grouped;
        }





        public async Task<List<RecetteEqpYearKey>> GetRecettesParEquipeParAnnee(DateTime startDate, DateTime endDate, List<int> ListNumsEquipes)

        {
            // Vérifiez que startDate est antérieure à endDate
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }

            List<Recette> recettes = new List<Recette>();

            if (ListNumsEquipes.IsNullOrEmpty())
            {
                throw new ArgumentException("La Liste des équipes sélectionnés est vide.");
            }
            else if (ListNumsEquipes[0] == 0)
            {
                // Récupérer les recettes dans la plage de dates
                recettes = await _rapportJournalierRepository.GetRecettesWithinDateRange(startDate, endDate);
            }
            else
            {
                foreach (int NumEquipe in ListNumsEquipes)
                {
                    var recettesSingleEquipe = await _rapportJournalierRepository.GetRecettesSingleEquipeWithinDateRange(startDate, endDate, NumEquipe);

                    if (recettesSingleEquipe != null)
                    {
                        recettes.AddRange(recettesSingleEquipe);
                    }
                }
            }

            //Déclarer une nouvelle liste de RecetteEqpYearKeys
            List<RecetteEqpYearKey> recetteEqpYearKeys = new List<RecetteEqpYearKey>();

            // Traitement des dépenses pour calculer par produit et par mois
            foreach (var recette in recettes)
            {
                // Calculer le numéro de l'annee
                int year = (recette.DateTransaction.Year - startDate.Year) + 1; // +1 pour avoir l'annee 1

                bool existanceRecette = recetteEqpYearKeys.Any(r => r.PosteRecetteId == recette.PosteRecetteId);

                if (existanceRecette)
                {
                    // Si la recette existe, récupérer l'instance correspondante
                    var recetteEqpYearKey = recetteEqpYearKeys.First(r => r.PosteRecetteId == recette.PosteRecetteId);

                    // Vérifier si l'année existe déjà dans le dictionnaire
                    var yearMontants = recetteEqpYearKey.YearsMontants.FirstOrDefault(w => w.ContainsKey(year));

                    if (yearMontants != null)
                    {
                        // Si l'année existe, additionner le montant à celui déjà présent
                        yearMontants[year] += recette.Montant;
                    }
                    else
                    {
                        // Si le mois n'existe pas, créer un nouvel enregistrement pour ce mois
                        recetteEqpYearKey.YearsMontants.Add(new Dictionary<int, decimal> { { year, recette.Montant } });
                    }
                }
                else
                {
                    // Si la recette n'existe pas, créer un nouveau RecetteEqpYearKey
                    var newRecetteEqpYearKey = new RecetteEqpYearKey
                    {
                        PosteRecetteId = recette.PosteRecetteId,
                        YearsMontants = new List<Dictionary<int, decimal>>
                        {
                            new Dictionary<int, decimal> { { year, recette.Montant } }
                        }
                    };

                    // Ajouter la nouvelle recette à la liste
                    recetteEqpYearKeys.Add(newRecetteEqpYearKey);
                }

            }

            // Retourner la liste contenant les résultats
            return recetteEqpYearKeys;
        }

        public async Task<List<RecetteEqpMonthKey>> GetRecettesParEquipeParMois(DateTime startDate, DateTime endDate, List<int> ListNumsEquipes)
        {
            // Vérifiez que startDate est antérieure à endDate
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }

            List<Recette> recettes = new List<Recette>();

            if (ListNumsEquipes.IsNullOrEmpty())
            {
                throw new ArgumentException("La Liste des équipes sélectionnés est vide.");
            }
            else if (ListNumsEquipes[0] == 0)
            {
                // Récupérer les recettes dans la plage de dates
                recettes = await _rapportJournalierRepository.GetRecettesWithinDateRange(startDate, endDate);
            }
            else
            {
                foreach (int NumEquipe in ListNumsEquipes)
                {
                    var recettesSingleEquipe = await _rapportJournalierRepository.GetRecettesSingleEquipeWithinDateRange(startDate, endDate, NumEquipe);

                    if (recettesSingleEquipe != null)
                    {
                        recettes.AddRange(recettesSingleEquipe);
                    }
                }
            }

            //Déclarer une nouvelle liste de RecetteEqpMonthKeys
            List<RecetteEqpMonthKey> recetteEqpMonthKeys = new List<RecetteEqpMonthKey>();

            // Traitement des dépenses pour calculer par produit et par mois
            foreach (var recette in recettes)
            {
                // Calculer le numéro de mois
                //int month = (recette.DateTransaction.Month - startDate.Month) + 1; // +1 pour avoir le mois 1
                string monthKey = $"{recette.DateTransaction.Month}-{recette.DateTransaction.Year}"; // Format "Mois-Année"

                bool existanceRecette = recetteEqpMonthKeys.Any(r => r.PosteRecetteId == recette.PosteRecetteId);

                if (existanceRecette)
                {
                    // Si la recette existe, récupérer l'instance correspondante
                    var recetteEqpMonthKey = recetteEqpMonthKeys.First(r => r.PosteRecetteId == recette.PosteRecetteId);

                    // Vérifier si le jour existe déjà dans le dictionnaire
                    var monthMontants = recetteEqpMonthKey.MonthsMontants.FirstOrDefault(w => w.ContainsKey(monthKey));

                    if (monthMontants != null)
                    {
                        // Si le mois existe, additionner le montant à celui déjà présent
                        monthMontants[monthKey] += recette.Montant;
                    }
                    else
                    {
                        // Si le mois n'existe pas, créer un nouvel enregistrement pour ce mois
                        recetteEqpMonthKey.MonthsMontants.Add(new Dictionary<string, decimal> { { monthKey, recette.Montant } });
                    }
                }
                else
                {
                    // Si la recette n'existe pas, créer un nouveau RecetteEqpMonthKey
                    var newRecetteEqpMonthKey = new RecetteEqpMonthKey
                    {
                        PosteRecetteId = recette.PosteRecetteId,
                        MonthsMontants = new List<Dictionary<string, decimal>>
                        {
                            new Dictionary<string, decimal> { { monthKey, recette.Montant } }
                        }
                    };

                    // Ajouter la nouvelle recette à la liste
                    recetteEqpMonthKeys.Add(newRecetteEqpMonthKey);
                }

            }

            // Retourner la liste contenant les résultats
            return recetteEqpMonthKeys;
        }

        public async Task<List<RecetteEqpWeekKey>> GetRecettesParEquipeParSemaine(DateTime startDate, DateTime endDate, List<int> ListNumsEquipes)
        {
            // Vérifiez que startDate est antérieure à endDate
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }

            List<Recette> recettes = new List<Recette>();

            if (ListNumsEquipes.IsNullOrEmpty())
            {
                throw new ArgumentException("La Liste des équipes sélectionnées est vide.");
            }
            else if (ListNumsEquipes[0] == 0)
            {
                // Récupérer les recettes dans la plage de dates
                recettes = await _rapportJournalierRepository.GetRecettesWithinDateRange(startDate, endDate);
            }
            else
            {
                foreach (int NumEquipe in ListNumsEquipes)
                {
                    var recettesSingleEquipe = await _rapportJournalierRepository.GetRecettesSingleEquipeWithinDateRange(startDate, endDate, NumEquipe);

                    if (recettesSingleEquipe != null)
                    {
                        recettes.AddRange(recettesSingleEquipe);
                    }
                }
            }

            //Déclarer une nouvelle liste de RecetteEqpWeekKeys
            List<RecetteEqpWeekKey> recetteEqpWeekKeys = new List<RecetteEqpWeekKey>();

            // Traitement des dépenses pour calculer par produit et par semaine
            foreach (var recette in recettes)
            {
                // Calculer le numéro de semaine
                //int semaine = (recette.DateTransaction.DayOfYear - startDate.DayOfYear) / 7 + 1; // +1 pour avoir la semaine 1
                // Calculer le numéro de la semaine et l'année
                int weekOfYear = CalculRangSemaineServices.GetWeekOfYear(recette.DateTransaction);
                string weekKey = $"{weekOfYear}-{recette.DateTransaction.Year}"; // Format "Semaine-Année"

                bool existanceRecette = recetteEqpWeekKeys.Any(r => r.PosteRecetteId == recette.PosteRecetteId);

                if (existanceRecette)
                {
                    // Si la recette existe, récupérer l'instance correspondante
                    var recetteEqpWeekKey = recetteEqpWeekKeys.First(r => r.PosteRecetteId == recette.PosteRecetteId);

                    // Vérifier si le jour existe déjà dans le dictionnaire
                    var weekMontants = recetteEqpWeekKey.WeeksMontants.FirstOrDefault(w => w.ContainsKey(weekKey));

                    if (weekMontants != null)
                    {
                        // Si le jour existe, additionner le montant à celui déjà présent
                        weekMontants[weekKey] += recette.Montant;
                    }
                    else
                    {
                        // Si le jour n'existe pas, créer un nouvel enregistrement pour ce jour
                        recetteEqpWeekKey.WeeksMontants.Add(new Dictionary<string, decimal> { { weekKey, recette.Montant } });
                    }
                }
                else
                {
                    // Si la recette n'existe pas, créer un nouveau RecetteEqpWeekKey
                    var newRecetteEqpWeekKey = new RecetteEqpWeekKey
                    {
                        PosteRecetteId = recette.PosteRecetteId,
                        WeeksMontants = new List<Dictionary<string, decimal>>
                        {
                            new Dictionary<string, decimal> { { weekKey, recette.Montant } }
                        }
                    };

                    // Ajouter la nouvelle recette à la liste
                    recetteEqpWeekKeys.Add(newRecetteEqpWeekKey);
                }

            }

            //Calculer Gains
            var VecteurColonneGains = await this.RetournerGainParWeek(startDate, endDate);
            if (VecteurColonneGains != null)
            {
                recetteEqpWeekKeys.Add(VecteurColonneGains);
            }

            // Retourner la liste contenant les résultats
            return recetteEqpWeekKeys;
        }

        public async Task<List<RecetteEqpDayKey>> GetRecettesParEquipeParJour(DateTime startDate, DateTime endDate, List<int> ListNumsEquipes)
        {
            // Vérifiez que startDate est antérieure à endDate
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }

            List<Recette> recettes = new List<Recette>();

            if (ListNumsEquipes.IsNullOrEmpty())
            {
                throw new ArgumentException("La Liste des équipes sélectionnés est vide.");
            }
            else if (ListNumsEquipes[0] == 0)
            {
                // Récupérer les recettes dans la plage de dates
                recettes = await _rapportJournalierRepository.GetRecettesWithinDateRange(startDate, endDate);
            }
            else
            {
                foreach (int NumEquipe in ListNumsEquipes)
                {
                    var recettesSingleEquipe = await _rapportJournalierRepository.GetRecettesSingleEquipeWithinDateRange(startDate, endDate, NumEquipe);

                    if (recettesSingleEquipe != null)
                    {
                        recettes.AddRange(recettesSingleEquipe);
                    }
                }
            }

            //Déclarer une nouvelle liste de RecetteEqpDayKeys
            List<RecetteEqpDayKey> recetteEqpDayKeys = new List<RecetteEqpDayKey>();

            // Traitement des dépenses pour calculer par produit et par jour
            foreach (var recette in recettes)
            {
                // Calculer le numéro de jour
                int day = (recette.DateTransaction.DayOfYear - startDate.DayOfYear) + 1; // +1 pour avoir le jour 1

                bool existanceRecette = recetteEqpDayKeys.Any(r => r.PosteRecetteId == recette.PosteRecetteId);

                if (existanceRecette)
                {
                    // Si la recette existe, récupérer l'instance correspondante
                    var recetteEqpDayKey = recetteEqpDayKeys.First(r => r.PosteRecetteId == recette.PosteRecetteId);

                    // Vérifier si le jour existe déjà dans le dictionnaire
                    var dayMontants = recetteEqpDayKey.DaysMontants.FirstOrDefault(w => w.ContainsKey(day));

                    if (dayMontants != null)
                    {
                        // Si le jour existe, additionner le montant à celui déjà présent
                        dayMontants[day] += recette.Montant;
                    }
                    else
                    {
                        // Si le jour n'existe pas, créer un nouvel enregistrement pour ce jour
                        recetteEqpDayKey.DaysMontants.Add(new Dictionary<int, decimal> { { day, recette.Montant } });
                    }
                }
                else
                {
                    // Si la recette n'existe pas, créer un nouveau RecetteEqpDayKey
                    var newRecetteEqpDayKey = new RecetteEqpDayKey
                    {
                        PosteRecetteId = recette.PosteRecetteId,
                        DaysMontants = new List<Dictionary<int, decimal>>
                        {
                            new Dictionary<int, decimal> { { day, recette.Montant } }
                        }
                    };

                    // Ajouter la nouvelle recette à la liste
                    recetteEqpDayKeys.Add(newRecetteEqpDayKey);
                }

            }

            //Calculer Gains
            var VecteurColonneGains = await this.RetournerGainParDay(startDate, endDate);
            if (VecteurColonneGains != null) {
                recetteEqpDayKeys.Add(VecteurColonneGains);
            }

            // Retourner la liste contenant les résultats
            return recetteEqpDayKeys;
        }


        private async Task<RecetteEqpDayKey> RetournerGainParDay(DateTime startDate, DateTime endDate)
        {
            // Vérifiez que startDate est antérieure à endDate
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }

            // Récupérer les recettes dans la plage de dates
            var rapportsJournaliers = await _rapportJournalierRepository.GetRapportsWithinDateRange(startDate, endDate);
            var recettes = await _rapportJournalierRepository.GetRecettesWithinDateRange(startDate, endDate);
            var depenses = await _rapportJournalierRepository.GetDepensesWithinDateRange(startDate, endDate);

            if (rapportsJournaliers == null || recettes == null || depenses == null)
            {
                throw new InvalidOperationException("Les données des rapports ou recettes ou depenses ne peuvent pas être nulles.");
            }


            var VecteurColonneGains = new RecetteEqpDayKey
            {
                PosteRecetteId = 4,
                DaysMontants = new List<Dictionary<int, decimal>>()
            };

            foreach (var rapportJournalier in rapportsJournaliers)
            {
                // Calculer la somme des recettes de ce rapport
                decimal TotalRecettesRapport = recettes
                    .Where(r => r.RapportJournalierId == rapportJournalier.Id) // Filtrer par RapportId
                    .Sum(r => r.Montant); // Faire la somme des montants

                // Calculer la somme des depenses pour le rapport sélectionné
                decimal TotalDepensesRapport = depenses
                    .Where(r => r.RapportJournalierId == rapportJournalier.Id) // Filtrer par RapportId
                    .Sum(r => r.Montant); // Faire la somme des montants
                var montantGainJour = TotalRecettesRapport - TotalDepensesRapport;

                // Calculer le numéro de jour
                int day = (rapportJournalier.Date.DayOfYear - startDate.DayOfYear) + 1; // +1 pour avoir le jour 1

                VecteurColonneGains.DaysMontants.Add(new Dictionary<int, decimal> { { day, montantGainJour } });
            }

            return VecteurColonneGains;
        
        }


        private async Task<RecetteEqpWeekKey> RetournerGainParWeek(DateTime startDate, DateTime endDate)
        {
            // Vérifiez que startDate est antérieure à endDate
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }

            // Récupérer les recettes dans la plage de dates
            var rapportsJournaliers = await _rapportJournalierRepository.GetRapportsWithinDateRange(startDate, endDate);
            var recettes = await _rapportJournalierRepository.GetRecettesWithinDateRange(startDate, endDate);
            var depenses = await _rapportJournalierRepository.GetDepensesWithinDateRange(startDate, endDate);

            if (rapportsJournaliers == null || recettes == null || depenses == null)
            {
                throw new InvalidOperationException("Les données des rapports ou recettes ou depenses ne peuvent pas être nulles.");
            }


            var VecteurColonneGains = new RecetteEqpWeekKey
            {
                PosteRecetteId = 4,
                WeeksMontants = new List<Dictionary<string, decimal>>()
            };

            foreach (var rapportJournalier in rapportsJournaliers)
            {
                // Calculer la somme des recettes de ce rapport
                decimal TotalRecettesRapport = recettes
                    .Where(r => r.RapportJournalierId == rapportJournalier.Id) // Filtrer par RapportId
                    .Sum(r => r.Montant); // Faire la somme des montants

                // Calculer la somme des depenses pour le rapport sélectionné
                decimal TotalDepensesRapport = depenses
                    .Where(r => r.RapportJournalierId == rapportJournalier.Id) // Filtrer par RapportId
                    .Sum(r => r.Montant); // Faire la somme des montants
                var montantGainJour = TotalRecettesRapport - TotalDepensesRapport;

                // Calculer le numéro de jour                
                // Calculer le numéro de la semaine et l'année
                int weekOfYear = CalculRangSemaineServices.GetWeekOfYear(rapportJournalier.Date);
                string weekKey = $"{weekOfYear}-{rapportJournalier.Date.Year}"; // Format "Semaine-Année"


                // Vérifier si la semaine existe déjà dans le Vecteur Colonne des Gains
                var weekMontants = VecteurColonneGains.WeeksMontants.FirstOrDefault(w => w.ContainsKey(weekKey));

                if (weekMontants != null)
                {
                    // Si la semaine existe, additionner le montant à celui déjà présent
                    weekMontants[weekKey] += montantGainJour;
                }
                else
                {
                    // Si le jour n'existe pas, créer un nouvel enregistrement pour ce jour
                    VecteurColonneGains.WeeksMontants.Add(new Dictionary<string, decimal> { { weekKey, montantGainJour } });
                }
                
            }

            return VecteurColonneGains;

        }






        public async Task<List<ProductQYearKey>> GetQuantitesParProduitParAnnee(DateTime startDate, DateTime endDate, List<string> ListNamesProducts)
        {
            // Vérifiez que startDate est antérieure à endDate
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }

            List<Depense> depenses = new List<Depense>();

            if (ListNamesProducts.IsNullOrEmpty())
            {
                throw new ArgumentException("La Liste des produits sélectionnés est vide.");
            }
            else if (ListNamesProducts[0] == "all")
            {
                // Récupérer les dépenses dans la plage de dates
                depenses = await _rapportJournalierRepository.GetDepensesWithinDateRange(startDate, endDate);
            }
            else
            {
                foreach (string NameProduct in ListNamesProducts)
                {
                    if (NameProduct.IsNullOrEmpty())
                    {
                        continue;
                    }

                    var depensesSingleProduct = await _rapportJournalierRepository.GetDepensesSingleProductWithinDateRange(startDate, endDate, NameProduct);

                    if (depensesSingleProduct != null)
                    {
                        depenses.AddRange(depensesSingleProduct);
                    }
                }
            }

            //Déclarer une nouvelle liste de ProductQKeys
            List<ProductQYearKey> productQYearKeys = new List<ProductQYearKey>();

            // Traitement des dépenses pour calculer par produit et par année
            foreach (var depense in depenses)
            {
                // Calculer le numéro de l'année
                int year = (depense.DateTransaction.Year - startDate.Year) + 1; // +1 pour avoir l'année 1

                bool existanceProduct = productQYearKeys.Any(p => p.ProductName.Equals(depense.ProductName, StringComparison.OrdinalIgnoreCase));

                if (existanceProduct)
                {
                    // Si le produit existe, récupérer l'instance correspondante
                    var productQYearKey = productQYearKeys.First(p => p.ProductName.Equals(depense.ProductName, StringComparison.OrdinalIgnoreCase));

                    // Vérifier si l'annee existe déjà dans le dictionnaire
                    var yearQuantites = productQYearKey.YearsQuantites.FirstOrDefault(y => y.ContainsKey(year));

                    if (yearQuantites != null)
                    {
                        // Si l'année existe, additionner la quantite à celui déjà présent
                        yearQuantites[year] += depense.Quantity;
                    }
                    else
                    {
                        // Si l'année n'existe pas, créer un nouvel enregistrement pour cette année
                        productQYearKey.YearsQuantites.Add(new Dictionary<int, decimal> { { year, depense.Quantity } });
                    }
                }
                else
                {
                    // Si le produit n'existe pas, créer un nouveau ProductQYearKey
                    var newProductQYearKey = new ProductQYearKey
                    {
                        ProductName = depense.ProductName,
                        YearsQuantites = new List<Dictionary<int, decimal>>
                        {
                            new Dictionary<int, decimal> { { year, depense.Quantity } }
                        }
                    };

                    // Ajouter le nouveau produit à la liste
                    productQYearKeys.Add(newProductQYearKey);
                }

            }

            // Retourner la liste contenant les résultats
            return productQYearKeys;
        }


        public async Task<List<ProductQMonthKey>> GetQuantitesParProduitParMois(DateTime startDate, DateTime endDate, List<string> ListNamesProducts)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }

            List<Depense> depenses = new List<Depense>();

            if (ListNamesProducts.IsNullOrEmpty())
            {
                throw new ArgumentException("La Liste des produits sélectionnés est vide.");
            }
            else if (ListNamesProducts[0] == "all")
            {
                depenses = await _rapportJournalierRepository.GetDepensesWithinDateRange(startDate, endDate);
            }
            else
            {
                foreach (string NameProduct in ListNamesProducts)
                {
                    if (NameProduct.IsNullOrEmpty())
                    {
                        continue;
                    }

                    var depensesSingleProduct = await _rapportJournalierRepository.GetDepensesSingleProductWithinDateRange(startDate, endDate, NameProduct);

                    if (depensesSingleProduct != null)
                    {
                        depenses.AddRange(depensesSingleProduct);
                    }
                }
            }

            List<ProductQMonthKey> productQMonthKeys = new List<ProductQMonthKey>();

            foreach (var depense in depenses)
            {
                // Calculer le numéro de mois et l'année
                string monthKey = $"{depense.DateTransaction.Month}-{depense.DateTransaction.Year}"; // Format "Mois-Année"

                bool existanceProduct = productQMonthKeys.Any(p => p.ProductName.Equals(depense.ProductName, StringComparison.OrdinalIgnoreCase));

                if (existanceProduct)
                {
                    var productQMonthKey = productQMonthKeys.First(p => p.ProductName.Equals(depense.ProductName, StringComparison.OrdinalIgnoreCase));

                    var monthQuantites = productQMonthKey.MonthsQuantites.FirstOrDefault(m => m.ContainsKey(monthKey));

                    if (monthQuantites != null)
                    {
                        monthQuantites[monthKey] += depense.Quantity;
                    }
                    else
                    {
                        productQMonthKey.MonthsQuantites.Add(new Dictionary<string, decimal> { { monthKey, depense.Quantity } });
                    }
                }
                else
                {
                    var newProductQMonthKey = new ProductQMonthKey
                    {
                        ProductName = depense.ProductName,
                        MonthsQuantites = new List<Dictionary<string, decimal>>
                {
                    new Dictionary<string, decimal> { { monthKey, depense.Quantity } }
                }
                    };

                    productQMonthKeys.Add(newProductQMonthKey);
                }
            }

            return productQMonthKeys;
        }


        public async Task<List<ProductQWeekKey>> GetQuantitesParProduitParSemaine(DateTime startDate, DateTime endDate, List<string> ListNamesProducts)
        {
            // Vérifiez que startDate est antérieure à endDate
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }

            List<Depense> depenses = new List<Depense>();

            if (ListNamesProducts.IsNullOrEmpty())
            {
                throw new ArgumentException("La Liste des produits sélectionnés est vide.");
            }
            else if (ListNamesProducts[0] == "all")
            {
                // Récupérer les dépenses dans la plage de dates
                depenses = await _rapportJournalierRepository.GetDepensesWithinDateRange(startDate, endDate);
            }
            else
            {
                foreach (string nameProduct in ListNamesProducts)
                {
                    if (string.IsNullOrEmpty(nameProduct))
                    {
                        continue;
                    }

                    var depensesSingleProduct = await _rapportJournalierRepository.GetDepensesSingleProductWithinDateRange(startDate, endDate, nameProduct);

                    if (depensesSingleProduct != null)
                    {
                        depenses.AddRange(depensesSingleProduct);
                    }
                }
            }

            List<ProductQWeekKey> productQWeekKeys = new List<ProductQWeekKey>();

            foreach (var depense in depenses)
            {
                // Calculer le numéro de la semaine et l'année
                int weekOfYear = CalculRangSemaineServices.GetWeekOfYear(depense.DateTransaction);
                string weekKey = $"{weekOfYear}-{depense.DateTransaction.Year}"; // Format "Semaine-Année"

                bool existanceProduct = productQWeekKeys.Any(p => p.ProductName.Equals(depense.ProductName, StringComparison.OrdinalIgnoreCase));

                if (existanceProduct)
                {
                    var productQWeekKey = productQWeekKeys.First(p => p.ProductName.Equals(depense.ProductName, StringComparison.OrdinalIgnoreCase));

                    var weekQuantites = productQWeekKey.WeeksQuantites.FirstOrDefault(w => w.ContainsKey(weekKey));

                    if (weekQuantites != null)
                    {
                        weekQuantites[weekKey] += depense.Quantity;
                    }
                    else
                    {
                        productQWeekKey.WeeksQuantites.Add(new Dictionary<string, decimal> { { weekKey, depense.Quantity } });
                    }
                }
                else
                {
                    var newProductQWeekKey = new ProductQWeekKey
                    {
                        ProductName = depense.ProductName,
                        WeeksQuantites = new List<Dictionary<string, decimal>>
                {
                    new Dictionary<string, decimal> { { weekKey, depense.Quantity } }
                }
                    };

                    productQWeekKeys.Add(newProductQWeekKey);
                }
            }

            return productQWeekKeys;
        }


        public async Task<List<ProductQDayKey>> GetQuantitesParProduitParJour(DateTime startDate, DateTime endDate, List<string> ListNamesProducts)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }

            if (ListNamesProducts.IsNullOrEmpty())
            {
                throw new ArgumentException("La liste des produits sélectionnés est vide.");
            }

            List<Depense> depenses = new List<Depense>();

            if (ListNamesProducts[0] == "all")
            {
                depenses = await _rapportJournalierRepository.GetDepensesWithinDateRange(startDate, endDate);
            }
            else
            {
                foreach (string NameProduct in ListNamesProducts)
                {
                    if (string.IsNullOrWhiteSpace(NameProduct))
                        continue;

                    var depensesSingleProduct = await _rapportJournalierRepository.GetDepensesSingleProductWithinDateRange(startDate, endDate, NameProduct);

                    if (depensesSingleProduct != null)
                    {
                        depenses.AddRange(depensesSingleProduct);
                    }
                }
            }

            List<ProductQDayKey> productQDayKeys = new List<ProductQDayKey>();

            foreach (var depense in depenses)
            {
                string dateKey = depense.DateTransaction.ToString("yyyy-MM-dd");

                var productQ = productQDayKeys.FirstOrDefault(p =>
                    p.ProductName.Equals(depense.ProductName, StringComparison.OrdinalIgnoreCase));

                if (productQ == null)
                {
                    productQ = new ProductQDayKey
                    {
                        ProductName = depense.ProductName,
                        DaysQuantites = new Dictionary<string, decimal>()
                    };
                    productQDayKeys.Add(productQ);
                }

                if (productQ.DaysQuantites.ContainsKey(dateKey))
                {
                    productQ.DaysQuantites[dateKey] += depense.Quantity;
                }
                else
                {
                    productQ.DaysQuantites[dateKey] = depense.Quantity;
                }
            }

            return productQDayKeys;
        }

    }

}
