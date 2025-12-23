using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.DTO.ProductDTOs;
using tech_software_engineer_consultant_int_backend.DTO.RapportsJournaliersDTOs;
using tech_software_engineer_consultant_int_backend.DTO.RecettesDTOs;
using tech_software_engineer_consultant_int_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using Twilio.TwiML.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RapportJournalierController : ControllerBase
    {
        private readonly IRapportJournalierService _rapportJournalierService;
        private readonly ITokenAuthenticationService _tokenAuthService;
        //private readonly INotificationService _notificationService;
        //private readonly IFirebaseNotificationService _firebaseNotificationService;

        public RapportJournalierController(
            //INotificationService notificationService, 
            IRapportJournalierService rapportJournalierService, 
            ITokenAuthenticationService tokenAuthService
            // , IFirebaseNotificationService firebaseNotificationService
            )
        {
            _rapportJournalierService = rapportJournalierService;
            _tokenAuthService = tokenAuthService;
            //_notificationService = notificationService;
            //_firebaseNotificationService = firebaseNotificationService;
        }


        private bool IsUserAuthorized(params string[] requiredPolicies)
        {
            // Récupérer le jeton d'authentification de l'en-tête de la requête
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            foreach (var policy in requiredPolicies)
            {
                if (_tokenAuthService.IsUserAuthorized(token, policy))
                {
                    return true;
                }
            }
            return false;
        }


        [HttpPost("add-RapportJournalier")]
        public async Task<IActionResult> CreateRapport(RapportJournalierDto dto)
        {
            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à ajouter des rapports journaliers.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Appel du service pour ajouter le rapport
            string result = await _rapportJournalierService.CreateRapportAsync(dto);

            if (result == "Un rapport existe déjà pour cette date.")
            {
                return Conflict(new { message = result });
            }

            // Récupérer tous les tokens enregistrés
            /*
            var deviceTokens = await _notificationService.GetAllDeviceTokensAsync();
            if (deviceTokens != null && deviceTokens.Any())
            {
                //return NotFound("Aucun token trouvé.");
                // Envoyer la notification à chaque token
                foreach (var deviceToken in deviceTokens)
                {
                    await _firebaseNotificationService.SendPushNotificationAsync(deviceToken.Token, "Nouveau Rapport", "Un nouveau rapport a été enregistré.");
                }               
            }
            */

            /*
            if(result == "Rapport ajouté avec succès.")
            {
                await _firebaseNotificationService.SendPushNotificationToAllConnectedUsersAsync("Nouveau Rapport", "Un nouveau rapport a été enregistré.");
            }  
            */

            return Ok(new { message = result });
        }



        [HttpPut("mettre-a-jour-Rapport-Journalier")]
        public async Task<IActionResult> MettreAJourRapportJournalier([FromBody] RapportJournalierDto rapport)
        {
            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à modifier des rapports journaliers.");
            }

            // Appel du service pour mettre à jour le rapport
            (bool, string, RapportJournalier) result = await _rapportJournalierService.MettreAJourRapportAsync(rapport);

            if (result.Item1)
                return Ok(new { message = result.Item2 });
            else
                return StatusCode(500, new { message = result.Item2 });
        }



        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        [HttpGet("get-RapportJournalier-byId/{id}")]
        public async Task<IActionResult> GetRapportById(int id)
        {
            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }
            
            var rapport = await _rapportJournalierService.GetRapportByIdAsync(id);
            if (rapport == null)
            {
                return NotFound();
            }
            return Ok(rapport);
        }
        

        [HttpGet("get-all-RapportsJournaliers")]
        public async Task<IActionResult> GetAllRapports()
        {
            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter les rapports journaliers.");
            }

            var rapports = await _rapportJournalierService.GetAllRapportsAsync();
            return Ok(rapports);
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        [HttpGet("depenses/produits/par-jour")]
        public async Task<IActionResult> GetDepensesParProduitParJour([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] List<string> ListNamesProducts)
        {
            // Vérifiez que la date de début est antérieure à la date de fin
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            try
            {
                List<ProductDayKey> rapport = await _rapportJournalierService.GetDepensesParProduitParJour(startDate, endDate, ListNamesProducts);
                if (rapport == null || !rapport.Any())
                {
                    return Ok("Aucune dépense trouvée pour cette période.");
                }
                return Ok(rapport);
            }
            catch (Exception ex)
            {
                // Logger l'exception ici (par exemple avec un service de logging)
                // Log.Error(ex, "Erreur lors de la récupération des dépenses par produit par semaine");
                return StatusCode(500, "Une erreur interne est survenue : " + ex.Message);
            }
        }



        [HttpPost("depenses/produits/par-day")]
        public async Task<IActionResult> GetDepensesParProduitParDay(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromBody] List<string> ListNamesProducts)
        {
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            try
            {
                List<ProductDayKey> rapport = await _rapportJournalierService.GetDepensesParProduitParJour(startDate, endDate, ListNamesProducts);
                if (rapport == null || !rapport.Any())
                {
                    return Ok("Aucune dépense trouvée pour cette période.");
                }
                return Ok(rapport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Une erreur interne est survenue : " + ex.Message);
            }
        }




        [HttpGet("depenses/produits/par-semaine")]
        public async Task<IActionResult> GetDepensesParProduitParSemaine([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] List<string> ListNamesProducts)
        {
            // Vérifiez que la date de début est antérieure à la date de fin
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            try
            {
                List<ProductWeekKey> rapport = await _rapportJournalierService.GetDepensesParProduitParSemaine(startDate, endDate, ListNamesProducts);
                if (rapport == null || !rapport.Any())
                {
                    return Ok("Aucune dépense trouvée pour cette période.");
                }
                return Ok(rapport);
            }
            catch (Exception ex)
            {
                // Logger l'exception ici (par exemple avec un service de logging)
                // Log.Error(ex, "Erreur lors de la récupération des dépenses par produit par semaine");
                return StatusCode(500, "Une erreur interne est survenue : " + ex.Message);
            }
        }


        [HttpPost("depenses/produits/par-week")]
        public async Task<IActionResult> GetDepensesParProduitParWeek([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromBody] List<string> listNamesProducts)
        {
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (listNamesProducts == null || !listNamesProducts.Any())
            {
                return BadRequest("La liste des noms de produits ne doit pas être vide.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            try
            {
                List<ProductWeekKey> rapport = await _rapportJournalierService.GetDepensesParProduitParSemaine(startDate, endDate, listNamesProducts);

                if (rapport == null || !rapport.Any())
                {
                    return Ok(new List<ProductWeekKey>()); // Retourne une liste vide au lieu d'un string pour rester cohérent
                }

                return Ok(rapport);
            }
            catch (Exception ex)
            {
                // Tu peux logger ici avec un logger si besoin
                return StatusCode(500, "Une erreur interne est survenue : " + ex.Message);
            }
        }






        //Endpoint pour Récupérer les Dépenses Groupées Par Mois
        [HttpGet("depenses/produits/par-mois")]
        public async Task<IActionResult> GetDepensesParProduitParMois(DateTime startDate, DateTime endDate, [FromQuery] List<string> ListNamesProducts)
        {
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            var rapport = await _rapportJournalierService.GetDepensesParProduitParMois(startDate, endDate, ListNamesProducts);
            if (rapport == null)
            {
                return NotFound();
            }
            return Ok(rapport);
        }


        [HttpPost("depenses/produits/par-month")]
        public async Task<IActionResult> GetDepensesParProduitParMonth([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromBody] List<string> listNamesProducts)
        {
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (listNamesProducts == null || !listNamesProducts.Any())
            {
                return BadRequest("La liste des noms de produits ne doit pas être vide.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            try
            {
                var rapport = await _rapportJournalierService.GetDepensesParProduitParMois(startDate, endDate, listNamesProducts);

                if (rapport == null || !rapport.Any())
                {
                    return Ok(new List<ProductMonthKey>()); // On retourne une liste vide
                }

                return Ok(rapport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Une erreur interne est survenue : " + ex.Message);
            }
        }





        //Endpoint pour Récupérer les Dépenses Groupées Par Annee
        [HttpGet("depenses/produits/par-annee")]
        public async Task<IActionResult> GetDepensesParProduitParAnnee(DateTime startDate, DateTime endDate, [FromQuery] List<string> ListNamesProducts)
        {
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            var rapport = await _rapportJournalierService.GetDepensesParProduitParAnnee(startDate, endDate, ListNamesProducts);
            if (rapport == null)
            {
                return NotFound();
            }
            return Ok(rapport);
        }





        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //Endpoint pour Récupérer les Recettes Groupées Par Jour
        [HttpGet("recettes/equipe/par-jour")]
        public async Task<IActionResult> GetRecettesParEquipeParJour([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] List<int> ListNumEquipes)
        {
            // Vérifiez que la date de début est antérieure à la date de fin
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            try
            {
                List<RecetteEqpDayKey> rapport = await _rapportJournalierService.GetRecettesParEquipeParJour(startDate, endDate, ListNumEquipes);
                if (rapport == null || !rapport.Any())
                {
                    return Ok("Aucune recette trouvée pour cette période.");
                }
                return Ok(rapport);
            }
            catch (Exception ex)
            {
                // Logger l'exception ici (par exemple avec un service de logging)
                // Log.Error(ex, "Erreur lors de la récupération des dépenses par produit par semaine");
                return StatusCode(500, "Une erreur interne est survenue : " + ex.Message);
            }
        }




        //Endpoint pour Récupérer les Recettes Groupées Par Semaine
        [HttpGet("recettes/equipes/par-semaine")]
        public async Task<IActionResult> GetRecettesParEquipeParSemaine([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] List<int> ListNumEquipes)
        {
            // Vérifiez que la date de début est antérieure à la date de fin
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            try
            {
                List<RecetteEqpWeekKey> rapport = await _rapportJournalierService.GetRecettesParEquipeParSemaine(startDate, endDate, ListNumEquipes);
                if (rapport == null || !rapport.Any())
                {
                    return Ok("Aucune recette trouvée pour cette période.");
                }
                return Ok(rapport);
            }
            catch (Exception ex)
            {
                // Logger l'exception ici (par exemple avec un service de logging)
                // Log.Error(ex, "Erreur lors de la récupération des dépenses par produit par semaine");
                return StatusCode(500, "Une erreur interne est survenue : " + ex.Message);
            }
        }




        //Endpoint pour Récupérer les Dépenses Groupées Par Mois
        [HttpGet("recettes/equipes/par-mois")]
        public async Task<IActionResult> GetRecettesParEquipeParMois(DateTime startDate, DateTime endDate, [FromQuery] List<int> ListNumEquipes)
        {
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            var rapport = await _rapportJournalierService.GetRecettesParEquipeParMois(startDate, endDate, ListNumEquipes);
            if (rapport == null || !rapport.Any())
            {
                return Ok("Aucune recette trouvée pour cette période.");
            }
            return Ok(rapport);
        }





        //Endpoint pour Récupérer les Recettes Groupées Par Annee
        [HttpGet("recettes/equipes/par-annee")]
        public async Task<IActionResult> GetRecettesParProduitParAnnee(DateTime startDate, DateTime endDate, [FromQuery] List<int> ListNumEquipes)
        {
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            var rapport = await _rapportJournalierService.GetRecettesParEquipeParAnnee(startDate, endDate, ListNumEquipes);
            if (rapport == null || !rapport.Any())
            {
                return Ok("Aucune recette trouvée pour cette période.");
            }
            return Ok(rapport);
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        [HttpGet("quantites/produits/par-jour")]
        public async Task<IActionResult> GetQuantitesParProduitParJour([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] List<string> ListNamesProducts)
        {
            // Vérifiez que la date de début est antérieure à la date de fin
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            try
            {
                List<ProductQDayKey> rapport = await _rapportJournalierService.GetQuantitesParProduitParJour(startDate, endDate, ListNamesProducts);
                if (rapport == null || !rapport.Any())
                {
                    return Ok("Aucune dépense trouvée pour cette période.");
                }
                return Ok(rapport);
            }
            catch (Exception ex)
            {
                // Logger l'exception ici (par exemple avec un service de logging)
                // Log.Error(ex, "Erreur lors de la récupération des dépenses par produit par semaine");
                return StatusCode(500, "Une erreur interne est survenue : " + ex.Message);
            }
        }


        [HttpPost("quantites/produits/par-day")]
        public async Task<IActionResult> GetQuantitesParProduitParDay([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromBody] List<string> listNamesProducts)
        {
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (listNamesProducts == null || !listNamesProducts.Any())
            {
                return BadRequest("La liste des noms de produits ne doit pas être vide.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            try
            {
                var rapport = await _rapportJournalierService.GetQuantitesParProduitParJour(startDate, endDate, listNamesProducts);

                return Ok(rapport ?? new List<ProductQDayKey>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Une erreur interne est survenue : " + ex.Message);
            }
        }





        [HttpGet("quantites/produits/par-semaine")]
        public async Task<IActionResult> GetQuantitesParProduitParSemaine([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] List<string> ListNamesProducts)
        {
            // Vérifiez que la date de début est antérieure à la date de fin
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            try
            {
                List<ProductQWeekKey> rapport = await _rapportJournalierService.GetQuantitesParProduitParSemaine(startDate, endDate, ListNamesProducts);
                if (rapport == null || !rapport.Any())
                {
                    return Ok("Aucune dépense trouvée pour cette période.");
                }
                return Ok(rapport);
            }
            catch (Exception ex)
            {
                // Logger l'exception ici (par exemple avec un service de logging)
                // Log.Error(ex, "Erreur lors de la récupération des dépenses par produit par semaine");
                return StatusCode(500, "Une erreur interne est survenue : " + ex.Message);
            }
        }


        [HttpPost("quantites/produits/par-week")]
        public async Task<IActionResult> GetQuantitesParProduitParWeek([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromBody] List<string> listNamesProducts)
        {
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (listNamesProducts == null || !listNamesProducts.Any())
            {
                return BadRequest("La liste des noms de produits ne doit pas être vide.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            try
            {
                var rapport = await _rapportJournalierService.GetQuantitesParProduitParSemaine(startDate, endDate, listNamesProducts);

                return Ok(rapport ?? new List<ProductQWeekKey>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Une erreur interne est survenue : " + ex.Message);
            }
        }





        //Endpoint pour Récupérer les Dépenses Groupées Par Mois
        [HttpGet("quantites/produits/par-mois")]
        public async Task<IActionResult> GetQuantitesParProduitParMois(DateTime startDate, DateTime endDate, [FromQuery] List<string> ListNamesProducts)
        {
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            var rapport = await _rapportJournalierService.GetQuantitesParProduitParMois(startDate, endDate, ListNamesProducts);
            if (rapport == null)
            {
                return NotFound();
            }
            return Ok(rapport);
        }


        [HttpPost("quantites/produits/par-month")]
        public async Task<IActionResult> GetQuantitesParProduitParMonth([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromBody] List<string> listNamesProducts)
        {
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (listNamesProducts == null || !listNamesProducts.Any())
            {
                return BadRequest("La liste des noms de produits ne doit pas être vide.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            try
            {
                var rapport = await _rapportJournalierService.GetQuantitesParProduitParMois(startDate, endDate, listNamesProducts);

                return Ok(rapport ?? new List<ProductQMonthKey>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Une erreur interne est survenue : " + ex.Message);
            }
        }








        //Endpoint pour Récupérer les Dépenses Groupées Par Annee
        [HttpGet("quantites/produits/par-annee")]
        public async Task<IActionResult> GetQuantitesParProduitParAnnee(DateTime startDate, DateTime endDate, [FromQuery] List<string> ListNamesProducts)
        {
            if (startDate > endDate)
            {
                return BadRequest("La date de début doit être antérieure à la date de fin.");
            }

            if (!IsUserAuthorized("RapportJournalierPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rapport journalier.");
            }

            var rapport = await _rapportJournalierService.GetQuantitesParProduitParAnnee(startDate, endDate, ListNamesProducts);
            if (rapport == null)
            {
                return NotFound();
            }
            return Ok(rapport);
        }





        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------



    }

}
