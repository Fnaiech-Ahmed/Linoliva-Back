using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenAuthenticationService _tokenAuthenticationService;

    public AuthController(ITokenAuthenticationService tokenAuthenticationService)
    {
        _tokenAuthenticationService = tokenAuthenticationService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        // Vérifier les informations d'identification, générer un jeton dans le cas favorable et l'envoyer à l'utilisateur. Sinon, envoyer un message d'erreur (informations d'identification invalides).
        if (_tokenAuthenticationService.ValidateCredentials(model))
        {
            string UserId = _tokenAuthenticationService.GetUserId(model);
            Role role = _tokenAuthenticationService.GetUserRole(model);
            if (role != null)
            {
                string token = await _tokenAuthenticationService.GenerateTokenAsync(UserId, model.Username, role);
                // Retourner le jeton au client
                return Ok(new { Token = token });
            }
            else
            {
                return BadRequest("Aucun role n'a encore été affecté à votre profil.");
            }
        }
        else
        {
            return BadRequest(new { Error = "Invalid credentials" });
        }
    }

    [HttpGet("validate")]
    public IActionResult ValidateToken(string token)
    {
        // Valider le jeton
        bool isValid = _tokenAuthenticationService.ValidateToken(token);

        // Retourner le résultat de validation au client
        return Ok(new { IsValid = isValid });
    }
}