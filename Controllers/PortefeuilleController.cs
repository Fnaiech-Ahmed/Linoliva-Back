using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.AspNetCore.Mvc;
using tech_software_engineer_consultant_int_backend.Services;
using System.Security.Claims;
using tech_software_engineer_consultant_int_backend.DTO.PortfoliosDTOs;
using tech_software_engineer_consultant_int_backend.Responses;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortefeuilleController : ControllerBase
    {
        private readonly IPortefeuilleService portefeuilleService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public PortefeuilleController(IPortefeuilleService service, ITokenAuthenticationService tokenAuthService)
        {
            portefeuilleService = service;
            _tokenAuthService = tokenAuthService;
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

        [HttpGet("getPortefeuilles")]
        public async Task<ActionResult<List<Portefeuille>>> GetPortefeuilles()
        {
            if (!IsUserAuthorized("PortefeuilleSeniorPolicy", "PortefeuillePolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à accéder à cette ressource." });
            }

            var portefeuilles = await portefeuilleService.GetPortefeuillesAsync();
            return Ok(portefeuilles);
        }

        [HttpGet("getPortefeuille/{id}")]
        public async Task<ActionResult<Portefeuille>> GetPortefeuille(int id)
        {
            if (!IsUserAuthorized("PortefeuilleSeniorPolicy", "PortefeuillePolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à accéder à cette ressource." });
            }

            var portefeuille = await portefeuilleService.GetPortefeuilleAsync(id);

            if (portefeuille == null)
            {
                return NotFound();
            }

            return Ok(portefeuille);
        }

        [HttpPost("addPortefeuille")]
        public async Task<ActionResult<string>> CreatePortefeuille([FromBody] PortefeuilleCreateDTO portefeuilleCreateDTO, [FromQuery] List<string> listeREFsRecouvreurs)
        {
            if (!IsUserAuthorized("PortefeuilleSeniorPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à ajouter un portefeuille." });
            }

            try
            {
                (bool success, int id, List<(string, string)> values) resultat = await portefeuilleService.CreatePortefeuilleAsync(portefeuilleCreateDTO, listeREFsRecouvreurs);
                if (resultat.success)
                    return Ok(new SuccessResponse { Message = "Portefeuille ajouté avec succès avec l'id: " + resultat.id });
                else
                    return BadRequest(new ErrorResponse { Error = resultat.values[0].Item1, Message = resultat.values[0].Item2 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Erreur interne du serveur", Message = "Une erreur interne s'est produite lors du traitement de votre demande." + ex.Message });
            }
        }

        [HttpPut("updatePortefeuille/{id}")]
        public async Task<ActionResult> UpdatePortefeuille(int id, Portefeuille portefeuille)
        {
            if (!IsUserAuthorized("PortefeuilleSeniorPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à mettre à jour un portefeuille." });
            }

            try
            {
                await portefeuilleService.UpdatePortefeuilleAsync(id, portefeuille);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("deletePortefeuille/{id}")]
        public async Task<ActionResult> DeletePortefeuille(int id)
        {
            if (!IsUserAuthorized("PortefeuilleSeniorPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à supprimer un portefeuille." });
            }

            try
            {
                await portefeuilleService.DeletePortefeuilleAsync(id);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
