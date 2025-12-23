using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.AspNetCore.Mvc;
using tech_software_engineer_consultant_int_backend.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RapportController : ControllerBase
    {
        private readonly IRapportService rapportService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public RapportController(IRapportService service, ITokenAuthenticationService tokenAuthService)
        {
            rapportService = service;
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

        [HttpGet("GetRapportsList")]
        public async Task<ActionResult<List<Rapport>>> GetRapports()
        {
            if (!IsUserAuthorized("RapportSeniorPolicy", "RapportPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à accéder à cette ressource." });
            }

            var rapports = await rapportService.GetRapportsAsync();
            return Ok(rapports);
        }

        [HttpGet("GetRapport/{id}")]
        public async Task<ActionResult<Rapport>> GetRapport(int id)
        {
            if (!IsUserAuthorized("RapportSeniorPolicy", "RapportPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à accéder à cette ressource." });
            }

            var rapport = await rapportService.GetRapportAsync(id);

            if (rapport == null)
            {
                return NotFound();
            }

            return Ok(rapport);
        }

        [HttpPost("CreateRapport")]
        public async Task<ActionResult<int>> CreateRapport([FromBody] Rapport rapport)
        {
            if (!IsUserAuthorized("RapportSeniorPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à créer un rapport." });
            }

            var id = await rapportService.CreateRapportAsync(rapport);
            return Ok(id);
        }

        [HttpPut("UpdateRapport/{id}")]
        public async Task<ActionResult> UpdateRapport(int id, [FromBody] Rapport rapport)
        {
            if (!IsUserAuthorized("RapportSeniorPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à mettre à jour un rapport." });
            }

            try
            {
                await rapportService.UpdateRapportAsync(id, rapport);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("AssignReporttoPortfolio/{idReport}/{idPortfolio}")]
        public async Task<ActionResult> AssignReportToPortfolio(int idReport, int idPortfolio)
        {
            if (!IsUserAuthorized("RapportSeniorPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à attribuer un rapport à un portefeuille." });
            }

            try
            {
                await rapportService.AssignReportToPortfolio(idReport, idPortfolio);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("DeleteRapport/{id}")]
        public async Task<ActionResult> DeleteRapport(int id)
        {
            if (!IsUserAuthorized("RapportSeniorPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à supprimer un rapport." });
            }

            try
            {
                await rapportService.DeleteRapportAsync(id);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
