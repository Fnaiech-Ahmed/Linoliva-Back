using Microsoft.AspNetCore.Mvc;
using tech_software_engineer_consultant_int_backend.DTO;
using tech_software_engineer_consultant_int_backend.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EcheanceController : ControllerBase
    {
        private readonly IEcheanceService _echeanceService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public EcheanceController(IEcheanceService echeanceService, ITokenAuthenticationService tokenAuthService)
        {
            _echeanceService = echeanceService;
            _tokenAuthService = tokenAuthService;
        }

        private bool IsUserAuthorized(string requiredPolicy)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            return _tokenAuthService.IsUserAuthorized(token, requiredPolicy);
        }

        [HttpGet("Get-List-Echeances")]
        public async Task<ActionResult<IEnumerable<EcheanceDto>>> GetAllEcheances()
        {
            if (!IsUserAuthorized("EcheancePolicy")) // Remplacez "requiredPolicy" par la politique réelle requise
            {
                return Unauthorized();
            }

            var echeances = await _echeanceService.GetAllAsync();
            return Ok(echeances);
        }

        [HttpGet("getEcheance/{id}")]
        public async Task<ActionResult<EcheanceDto>> GetById(int id)
        {
            if (!IsUserAuthorized("EcheancePolicy")) // Remplacez "requiredPolicy" par la politique réelle requise
            {
                return Unauthorized();
            }

            var echeance = await _echeanceService.GetByIdAsync(id);
            if (echeance == null)
            {
                return NotFound();
            }
            return Ok(echeance);
        }

        [HttpPost("Create-Echeance")]
        public async Task<ActionResult> Create([FromBody] CreateEcheanceDto createEcheanceDto)
        {
            if (!IsUserAuthorized("EcheanceSeniorPolicy")) // Remplacez "requiredPolicy" par la politique réelle requise
            {
                return Unauthorized();
            }

            var result = await _echeanceService.AddAsync(createEcheanceDto);
            if (result)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create echeance");
            }
        }

        [HttpPut("update-Echeance/{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateEcheanceDto updateEcheanceDto)
        {
            if (!IsUserAuthorized("EcheanceSeniorPolicy")) // Remplacez "requiredPolicy" par la politique réelle requise
            {
                return Unauthorized();
            }

            try
            {
                await _echeanceService.UpdateAsync(id, updateEcheanceDto);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Echeance not found")
                {
                    return NotFound();
                }
                throw;
            }
            return NoContent();
        }

        [HttpDelete("delete-Echeance/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (!IsUserAuthorized("EcheanceSeniorPolicy")) // Remplacez "requiredPolicy" par la politique réelle requise
            {
                return Unauthorized();
            }

            await _echeanceService.DeleteAsync(id);
            return NoContent();
        }
    }
}
