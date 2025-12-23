using tech_software_engineer_consultant_int_backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Services;
using tech_software_engineer_consultant_int_backend.DTO.BonsDeSortieDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BonDeSortieController : ControllerBase
    {
        private readonly IBonDeSortieService _bonDeSortieService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public BonDeSortieController(IBonDeSortieService bonDeSortieService, ITokenAuthenticationService tokenAuthService)
        {
            _bonDeSortieService = bonDeSortieService;
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

        [HttpPost("Create-Bon-De-Sortie")]
        public ActionResult<bool> AddBonDeSortie([FromBody] BonDeSortieCreateDTO bonDeSortieCreateDTO)
        {
            if (!IsUserAuthorized("BonDeSortieSeniorPolicy"))
            {
                return Unauthorized();
            }

            try
            {
                _bonDeSortieService.AddBonDeSortie(bonDeSortieCreateDTO);
                return true;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("Get-List-Bons-De-Sortie")]
        public async Task<ActionResult<List<BonDeSortieDTO>>> GetBonDeSorties()
        {
            if (!IsUserAuthorized("BonDeSortiePolicy", "BonDeSortieSeniorPolicy"))
            {
                return Unauthorized();
            }

            var bonsDeSortie = await _bonDeSortieService.GetBonDeSorties();
            return Ok(bonsDeSortie);
        }

        [HttpGet("get-Bon-De-Sortie-ByID/{id}")]
        public async Task<ActionResult<BonDeSortieDTO?>> GetBonDeSortieById(int id)
        {
            if (!IsUserAuthorized("BonDeSortiePolicy", "BonDeSortieSeniorPolicy"))
            {
                return Unauthorized();
            }

            var bonDeSortie = await _bonDeSortieService.GetBonDeSortieById(id);
            if (bonDeSortie == null)
            {
                return NotFound();
            }
            return Ok(bonDeSortie);
        }

        [HttpGet("get-Bon-De-Sortie-ByRef/{reference}")]
        public async Task<ActionResult<BonDeSortieDTO?>> GetBonDeSortieByRef(string reference)
        {
            if (!IsUserAuthorized("BonDeSortiePolicy", "BonDeSortieSeniorPolicy"))
            {
                return Unauthorized();
            }

            var bonDeSortie = await _bonDeSortieService.GetBonDeSortieByReference(reference);
            if (bonDeSortie == null)
            {
                return NotFound();
            }
            return Ok(bonDeSortie);
        }

        [HttpPut("update-BonDeSortie/{id}")]
        public async Task<ActionResult<bool>> UpdateBonDeSortie(int bonDeSortieId, [FromBody] BonDeSortieUpdateDTO newBonDeSortie)
        {
            if (!IsUserAuthorized("BonDeSortieSeniorPolicy"))
            {
                return Unauthorized();
            }

            if (newBonDeSortie == null)
            {
                return BadRequest("Bon de sortie data is null");
            }

            try
            {
                var resultat = await _bonDeSortieService.UpdateBonDeSortie(bonDeSortieId, newBonDeSortie);
                if (resultat)
                {
                    return NoContent();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update bon de sortie");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("delete-BonDeSortie/{id}")]
        public async Task<ActionResult<bool>> DeleteBonDeSortie(int id)
        {
            if (!IsUserAuthorized("BonDeSortieSeniorPolicy"))
            {
                return Unauthorized();
            }

            var result = await _bonDeSortieService.DeleteBonDeSortie(id);
            if (result)
            {
                return Ok(true);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete bon de sortie");
            }
        }
    }
}
