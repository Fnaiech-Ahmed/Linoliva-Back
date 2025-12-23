using Microsoft.AspNetCore.Mvc;
using tech_software_engineer_consultant_int_backend.Services;
using tech_software_engineer_consultant_int_backend.DTO.AbonnementsDTOs;
using tech_software_engineer_consultant_int_backend.DTO.ActivationDTOs;
using tech_software_engineer_consultant_int_backend.Models;
using TSE_Consultant_INT_Backend.Models.DTO.AbonnementsDTOs;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbonnementController : ControllerBase
    {
        private readonly IAbonnementService _abonnementService;
        private readonly IActivationService _activationService;
        private readonly ITokenAuthenticationService _tokenAuth;

        public AbonnementController(
            IAbonnementService abonnementService,
            IActivationService activationService,
            ITokenAuthenticationService tokenAuth)
        {
            _abonnementService = abonnementService;
            _activationService = activationService;
            _tokenAuth = tokenAuth;
        }

        private bool IsAuthorized(params string[] policies)
        {
            var token = Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            foreach (var policy in policies)
            {
                if (_tokenAuth.IsUserAuthorized(token, policy))
                    return true;
            }
            return false;
        }

        // -------------------------------------------
        // 1. CRÉATION D’UN ABONNEMENT
        // -------------------------------------------
        [HttpPost("create")]
        public async Task<IActionResult> CreateAbonnement([FromBody] CreateAbonnementDTO dto)
        {
            if (!IsAuthorized("AbonnementSeniorPolicy", "AdminPolicy"))
                return Unauthorized();

            var abo = await _abonnementService
                .CreateAbonnementAsync(dto.ProductId, dto.SubscriptionType, dto.MaxDevicesAllowed, dto.MaxUsersAllowed);

            return CreatedAtAction(nameof(GetAbonnement), new { id = abo.Id }, abo);
        }

        // -------------------------------------------
        // 2. AFFECTATION DU CODE À UN UTILISATEUR
        // -------------------------------------------
        [HttpPost("assign")]
        public async Task<IActionResult> AssignActivation([FromBody] AssignActivationDTO dto)
        {
            if (!IsAuthorized("AbonnementSeniorPolicy", "AdminPolicy"))
                return Unauthorized();

            var result = await _activationService.AssignActivationToUserAsync(dto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // -------------------------------------------
        // 3. OBTENIR UN ABONNEMENT PAR ID
        // -------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAbonnement(int id)
        {
            if (!IsAuthorized("AbonnementSeniorPolicy", "AdminPolicy"))
                return Unauthorized();

            var abo = await _abonnementService.GetAbonnementAsync(id.ToString());

            if (abo == null) return NotFound();

            return Ok(abo);
        }

        // -------------------------------------------
        // 4. LISTE DES ABONNEMENTS
        // -------------------------------------------
        /*[HttpGet("list")]
        public async Task<IActionResult> GetAll()
        {
            if (!IsAuthorized("AbonnementSeniorPolicy", "AdminPolicy"))
                return Unauthorized();

            var abonnements = await _abonnementService.GetAllAbonnements();

            return Ok(abonnements);
        }*/

        // -------------------------------------------
        // 5. MODIFICATION D’UN ABONNEMENT
        // -------------------------------------------
        /*[HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Abonnement model)
        {
            if (!IsAuthorized("AbonnementSeniorPolicy", "AdminPolicy"))
                return Unauthorized();

            if (model.Id != id)
                return BadRequest("ID mismatch");

            await _abonnementService.UpdateAbonnementAsync(model);

            return NoContent();
        }*/

        // -------------------------------------------
        // 6. SUPPRESSION
        // -------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAuthorized("AbonnementSeniorPolicy", "AdminPolicy"))
                return Unauthorized();

            await _abonnementService.DeleteAbonnementAsync(id.ToString());

            return NoContent();
        }

        // -------------------------------------------
        // 7. VENTE D’UN ABONNEMENT
        // -------------------------------------------
        [HttpPut("sell/{id}")]
        public async Task<IActionResult> Sell(int id, [FromBody] AbonnementSellDTO dto)
        {
            if (!IsAuthorized("AbonnementSeniorPolicy", "AdminPolicy"))
                return Unauthorized();

            var ok = await _abonnementService.SellAbonnementAsync(id, dto);

            if (!ok)
                return BadRequest("Impossible de vendre l'abonnement.");

            return NoContent();
        }
    }
}
