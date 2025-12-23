using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DossierController : ControllerBase
    {
        private readonly IDossierService _dossierService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public DossierController(IDossierService service, ITokenAuthenticationService tokenAuthService)
        {
            _dossierService = service;
            _tokenAuthService = tokenAuthService;
        }

        private bool IsUserAuthorized(params string[] requiredPolicies)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var user = _tokenAuthService.GetPrincipalFromToken(token);

            if (user == null)
            {
                return false;
            }

            foreach (var policy in requiredPolicies)
            {
                if (user.HasClaim(c => c.Type == "Policy" && c.Value == policy))
                {
                    return true;
                }
            }
            return false;
        }

        [HttpGet("GetDossiers")]
        public async Task<ActionResult<List<Dossier>>> GetDossiers()
        {
            if (!IsUserAuthorized("DossierPolicy", "DossierSeniorPolicy"))
            {
                return Unauthorized();
            }

            var dossiers = await _dossierService.GetDossiersAsync();
            return Ok(dossiers);
        }

        [HttpGet("GetDossier/{id}")]
        public async Task<ActionResult<Dossier>> GetDossier(int id)
        {
            if (!IsUserAuthorized("DossierPolicy", "DossierSeniorPolicy"))
            {
                return Unauthorized();
            }

            var dossier = await _dossierService.GetDossierAsync(id);
            if (dossier == null)
            {
                return NotFound();
            }
            return Ok(dossier);
        }

        [HttpPost("CreateDossier")]
        public async Task<ActionResult<int>> CreateDossier(Dossier dossier)
        {
            if (!IsUserAuthorized("DossierPolicy", "DossierSeniorPolicy"))
            {
                return Unauthorized();
            }

            var id = await _dossierService.CreateDossierAsync(dossier);
            return Ok(id);
        }

        [HttpPut("UpdateDossier/{id}")]
        public async Task<ActionResult> UpdateDossier(int id, Dossier dossier)
        {
            if (!IsUserAuthorized("DossierPolicy", "DossierSeniorPolicy"))
            {
                return Unauthorized();
            }

            try
            {
                await _dossierService.UpdateDossierAsync(id, dossier);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("affectFoldertoPortfolio/{idFolder}/{idPortfolio}")]
        public async Task<ActionResult> AffectFoldertoPortfolio(int idFolder, int idPortfolio)
        {
            if (!IsUserAuthorized("DossierPolicy", "DossierSeniorPolicy"))
            {
                return Unauthorized();
            }

            try
            {
                await _dossierService.AffectFoldertoPortfolio(idFolder, idPortfolio);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpDelete("DeleteDossier/{id}")]
        public async Task<ActionResult> DeleteDossier(int id)
        {
            if (!IsUserAuthorized("DossierPolicy", "DossierSeniorPolicy"))
            {
                return Unauthorized();
            }

            try
            {
                await _dossierService.DeleteDossierAsync(id);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
