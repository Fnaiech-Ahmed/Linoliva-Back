using Microsoft.AspNetCore.Mvc;
using tech_software_engineer_consultant_int_backend.DTO.ActivationCodesDTOs;
using tech_software_engineer_consultant_int_backend.DTO.ActivationDTOs;
using tech_software_engineer_consultant_int_backend.DTO.Activations;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Services;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivationController : ControllerBase
    {
        private readonly IActivationService _activationService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public ActivationController(IActivationService activationService, ITokenAuthenticationService tokenAuthService)
        {
            _activationService = activationService;
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

        // ---------------------------
        // 1) Create activation code
        // ---------------------------
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateActivationDTO code)
        {
            if (!IsUserAuthorized("AbonnementSeniorPolicy"))
                return Unauthorized(new { message = "Permission refusée" });

            var entity = await _activationService.CreateCodeActivationAsync(code);

            if (entity == null)
                return BadRequest(new { message = "Impossible de créer le code d’activation." });

            return CreatedAtAction(
                nameof(GetByReference),
                "Activation",
                new { reference = entity.ActivationCodeReference },
                entity
                );

        }


        // ---------------------------
        // 2) Assign code to user
        // ---------------------------
        [HttpPost("assign")]
        public async Task<IActionResult> Assign([FromBody] AssignActivationDTO dto)
        {
            if (!IsUserAuthorized("AdminPolicy"))
                return Unauthorized(new { message = "Permission refusée" });

            var res = await _activationService.AssignActivationToUserAsync(dto);
            if (!res.Success) return BadRequest(new { message = res.Message });

            return Ok(new { message = res.Message });
        }

        // ---------------------------
        // 3) Register device
        // (Accessible par client app, donc pas de login obligatoire)
        // ---------------------------
        [HttpPost("register-device/{activationCode}")]
        public async Task<IActionResult> RegisterDevice(string activationCode, [FromBody] ActivationDeviceDTO device)
        {
            var res = await _activationService.RegisterDeviceAsync(activationCode, device);
            if (!res.Success) return BadRequest(new { message = res.Message });

            return Ok(new { message = res.Message });
        }

        // ---------------------------
        // 4) Validate activation
        // ---------------------------
        [HttpGet("validate/{activationCode}/{deviceId?}")]
        public async Task<IActionResult> Validate(
            string activationCode,
            string? deviceId = null,
            [FromQuery] string? email = null,
            [FromQuery] string? phone = null)
        {
            var res = await _activationService.ValidateActivationAsync(activationCode, deviceId, email, phone);
            if (!res.Success) return Unauthorized(new { message = res.Message });

            return Ok(new { message = res.Message, accessType = res.AccessType, expiration = res.Expiration });
        }

        // ---------------------------
        // 5) Get activation by reference
        // ---------------------------
        [HttpGet("{reference}")]
        public async Task<IActionResult> GetByReference(string reference)
        {
            if (!IsUserAuthorized("AbonnementPolicy", "AbonnementSeniorPolicy"))
                return Unauthorized(new { message = "Permission refusée" });

            var code = await _activationService.GetByReferenceAsync(reference);
            if (code == null) return NotFound();

            return Ok(code);
        }
    }
}
