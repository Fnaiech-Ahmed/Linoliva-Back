using Microsoft.AspNetCore.Mvc;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Services;
using tech_software_engineer_consultant_int_backend.DTO.FacturesDTOs;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FactureController : ControllerBase
    {
        private readonly IFactureService factureService;
        private readonly ITokenAuthenticationService _tokenAuthService; // Ajout du service d'authentification

        public FactureController(IFactureService _factureService, ITokenAuthenticationService tokenAuthService)
        {
            factureService = _factureService;
            _tokenAuthService = tokenAuthService; // Injection du service d'authentification
        }

        private bool IsUserAuthorized(params string[] requiredPolicies)
        {
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

        [HttpPost("Create-Facture")]
        public async Task<IActionResult> AddFacture([FromBody] FactureCreateDTO facture)
        {
            if (IsUserAuthorized("FactureSeniorPolicy", "AdminPolicy"))
            {
                try
                {
                    bool result = await factureService.AddFacture(facture);
                    if (result)
                    {
                        return Ok(); // Retourner un statut 200 OK si l'ajout est réussi
                    }
                    else
                    {
                        return BadRequest("Échec de l'ajout de la facture."); // Retourner un BadRequest si l'ajout échoue
                    }
                }
                catch (Exception ex)
                {
                    // Gérer l'exception si nécessaire
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                return Unauthorized(); // Retourner Unauthorized si la politique n'est pas remplie
            }
            
        }



        [HttpGet("Get-List-Factures")]
        public async Task<ActionResult<List<FactureDTO>>> GetFactures()
        {
            if (IsUserAuthorized("FacturePolicy", "FactureSeniorPolicy", "AdminPolicy"))
            {
                try
                {
                    var factures = await factureService.GetFactures();
                    return Ok(factures); // Retourner les factures avec un statut 200 OK
                }
                catch (Exception ex)
                {
                    // Gérer l'exception si nécessaire
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else if (IsUserAuthorized("UserPolicy"))
            {
                try
                {
                    string currentUserId = _tokenAuthService.GetCurrentUserId(); // Méthode pour récupérer l'ID de l'utilisateur courant
                    var factures = await factureService.GetListeFacturesByOwner(currentUserId);
                    return Ok(factures); // Retourner les factures avec un statut 200 OK
                }
                catch (Exception ex)
                {
                    // Gérer l'exception si nécessaire
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                return Unauthorized(); // Retourner Unauthorized si la politique n'est pas remplie
            }            
                       
        }



        [HttpGet("get-Facture-ID/{id}")]
        public async Task<ActionResult<FactureDTO>> GetFactureById(int id)
        {
            if (IsUserAuthorized("FacturePolicy", "FactureSeniorPolicy", "AdminPolicy"))
            {
                try
                {
                    var facture = await factureService.GetFactureById(id);
                    if (facture == null)
                    {
                        return NotFound(); // Retourner NotFound si la facture n'existe pas
                    }
                    return Ok(facture); // Retourner la facture avec un statut 200 OK
                }
                catch (Exception ex)
                {
                    // Gérer l'exception si nécessaire
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else if (IsUserAuthorized("UserPolicy"))
            {
                try
                {
                    string currentUserId = _tokenAuthService.GetCurrentUserId(); // Méthode pour récupérer l'ID de l'utilisateur courant
                    var facture = await factureService.GetFactureById(id);
                    if (facture == null)
                    {
                        return NotFound(); // Retourner NotFound si la facture n'existe pas
                    }
                    else if (facture.ProprietaireId.ToString() == currentUserId)
                    {
                        return Ok(facture); // Retourner la facture avec un statut 200 OK
                    }
                    else
                    {
                        return Unauthorized();
                    }
                    
                }
                catch (Exception ex)
                {
                    // Gérer l'exception si nécessaire
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                return Unauthorized(); // Retourner Unauthorized si la politique n'est pas remplie
            }                            
        }





        [HttpGet("get-Facture-Ref/{RefFacture}")]
        public async Task<IActionResult> GetFactureByRef(string RefFacture)
        {
            if (IsUserAuthorized("FacturePolicy", "FactureSeniorPolicy", "AdminPolicy"))
            {
                try
                {
                    var facture = await factureService.GetFactureByReference(RefFacture);
                    if (facture == null)
                    {
                        return NotFound(); // Retourner NotFound si la facture n'existe pas
                    }
                    return Ok(facture); // Retourner la facture avec un statut 200 OK           
                }
                catch (Exception ex)
                {
                    // Gérer l'exception si nécessaire
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }

            }
            else if (IsUserAuthorized("UserPolicy"))
            {
                try
                {
                    string currentUserId = _tokenAuthService.GetCurrentUserId(); // Méthode pour récupérer l'ID de l'utilisateur courant
                    var facture = await factureService.GetFactureByReference(RefFacture);
                    if (facture == null)
                    {
                        return NotFound(); // Retourner NotFound si la facture n'existe pas
                    }
                    else if (facture.ProprietaireId.ToString() == currentUserId)
                    {
                        return Ok(facture); // Retourner la facture avec un statut 200 OK
                    }
                    else
                    {
                        return Unauthorized();
                    }

                }
                catch (Exception ex)
                {
                    // Gérer l'exception si nécessaire
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                return Unauthorized(); // Retourner Unauthorized si la politique n'est pas remplie
            }
            
        }





        [HttpPut("update-Facture/{id}")]
        public async Task<IActionResult> UpdateFacture(int factureId, [FromBody] FactureUpdateDTO factureUpdateDTO)
        {
            if (factureUpdateDTO == null)
            {
                return BadRequest("Invalid facture data");
            }

            if (!IsUserAuthorized("requiredPolicy"))
            {
                return Unauthorized();
            }

            try
            {
                // Convertir FactureUpdateDTO en Facture
                Facture facture = factureUpdateDTO.ToFactureEntity();                

                // Appeler le service pour mettre à jour la facture
                bool resultat = await factureService.UpdateFacture(factureId, factureUpdateDTO);
                if (resultat)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update facture");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] FacturePdfDTO request)
        {
            if (request == null)
                return BadRequest("Invalid data.");

            if (request.ListeBL == null || !request.ListeBL.Any())
                return BadRequest("La facture doit contenir au moins un bon de livraison.");

            // Montant HT Facture = somme des HT de toutes les commandes
            request.MontantHTFacture = request.ListeBL
                .SelectMany(bl => bl.Commandes)
                .Sum(cmd => cmd.MontantTotalHT);

            // TVA Facture = somme TVA des commandes
            request.TVA = request.ListeBL
                .SelectMany(bl => bl.Commandes)
                .Sum(cmd => cmd.TVA);

            // FODEC (1% du HT si inclus)
            decimal fodecAmount = request.IncludeFodec
                ? request.MontantHTFacture * 0.01m
                : 0m;

            // Timbre fiscal (Tunisie)
            const decimal TimbreFiscal = 1.000m;

            // TOTAL TTC FACTURE
            request.MontantTTCFacture =
                request.MontantHTFacture
                + request.TVA
                + fodecAmount
                + TimbreFiscal
                - request.Remise;

            // Dates de sécurité
            request.DateMiseAJour = DateTime.UtcNow;
            if (request.DateEmission == default)
                request.DateEmission = DateTime.UtcNow;

            var pdf = factureService.Generate(request);

            return File(
                pdf,
                "application/pdf",
                $"Facture-{request.ReferenceFacture}.pdf"
            );
        }


        [HttpDelete("delete-Facture/{id}")]
        public async Task<IActionResult> DeleteFacture(int id)
        {
            if (!IsUserAuthorized("AdminPolicy"))
                return Unauthorized(); // Retourner Unauthorized si la politique n'est pas remplie

            try
            {
                bool result = await factureService.DeleteFacture(id);
                if (result)
                {
                    return Ok(); // Retourner un statut 200 OK si la suppression est réussie
                }
                else
                {
                    return BadRequest("Échec de la suppression de la facture."); // Retourner BadRequest si la suppression échoue
                }
            }
            catch (Exception ex)
            {
                // Gérer l'exception si nécessaire
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
