using Microsoft.AspNetCore.Mvc;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Services;
using tech_software_engineer_consultant_int_backend.DTO.ImpayesDTOs;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImpayeController : ControllerBase
    {
        private readonly IImpayeService impayeService;
        private readonly ITokenAuthenticationService _tokenAuthService; // Ajout du service d'authentification

        public ImpayeController(IImpayeService service, ITokenAuthenticationService tokenAuthService)
        {
            impayeService = service;
            _tokenAuthService = tokenAuthService; // Injection du service d'authentification
        }

        private bool IsUserAuthorized(string requiredPolicy)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            return _tokenAuthService.IsUserAuthorized(token, requiredPolicy);
        }

        [HttpGet("GetImpayesList")]
        public async Task<ActionResult<List<Impaye>>> GetImpayes()
        {
            if (!IsUserAuthorized("AdminPolicy"))
                return Unauthorized();

            var impayes = await impayeService.GetImpayesAsync();
            return Ok(impayes);
        }

        [HttpGet("GetImpaye/{id}")]
        public async Task<ActionResult<Impaye>> GetImpaye(int id)
        {
            if (!IsUserAuthorized("AdminPolicy"))
                return Unauthorized();

            var impaye = await impayeService.GetImpayeAsync(id);
            if (impaye == null)
            {
                return NotFound();
            }

            return Ok(impaye);
        }

        [HttpPost("CreateImpaye/{idEcheance}")]
        public async Task<ActionResult<int>> CreateImpaye(ImpayeCreateDTO impaye, int idEcheance)
        {
            if (!IsUserAuthorized("AdminPolicy"))
                return Unauthorized();

            var id = await impayeService.CreateImpayeAsync(impaye);
            if (id > 0)
            {
                return Ok(id);
            }
            else
            {
                return BadRequest("Problème d'ajout de l'impayé de l'échéance portante de l'id: " + idEcheance);
            }
        }

        [HttpPut("UpdateImpaye/{id}")]
        public async Task<ActionResult> UpdateImpaye(int id, Impaye impaye)
        {
            if (!IsUserAuthorized("AdminPolicy"))
                return Unauthorized();

            try
            {
                await impayeService.UpdateImpayeAsync(id, impaye);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("AffecterImpayeAuDossier/{idImpaye}/{idDossier}")]
        public async Task<ActionResult> AffectImpayetoDossier(int idImpaye, int idDossier)
        {
            if (!IsUserAuthorized("AdminPolicy"))
                return Unauthorized();

            try
            {
                await impayeService.AffectImpayertoDossier(idImpaye, idDossier);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("DeleteImpaye/{id}")]
        public async Task<ActionResult> DeleteImpaye(int id)
        {
            if (!IsUserAuthorized("AdminPolicy"))
                return Unauthorized();

            try
            {
                await impayeService.DeleteImpayeAsync(id);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
