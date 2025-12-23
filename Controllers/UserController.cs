using tech_software_engineer_consultant_int_backend.DTO.UsersDTOs;
using tech_software_engineer_consultant_int_backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _service;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public UserController(IUserService service, ITokenAuthenticationService tokenAuthService)
        {
            _service = service;
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

        /*[HttpGet("GetUsers")]
        //[Authorize (Policy = "AdminPolicy")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            // Récupérer le jeton d'authentification de l'en-tête de la requête
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            Console.WriteLine(token);

            // Valider le jeton
            bool isValidToken = _tokenAuthService.ValidateToken(token);

            if (isValidToken)
            {
                // Récupérer l'identité de l'utilisateur à partir du jeton
                ClaimsPrincipal user = _tokenAuthService.GetPrincipalFromToken(token);

                // Récupérer le rôle de l'utilisateur à partir de la revendication
                string? userRole = user.FindFirst("RoleName")?.Value;

                // Vérifier si l'utilisateur a le rôle requis
                if (userRole == "admin")
                {
                    var users = await _service.GetUsersAsync();
                    return Ok(users);
                }
                else if (userRole == null)
                {
                    return BadRequest("l'utilisateur n'a pas encore de rôle.");
                }
                else
                {
                    return BadRequest("Vous n'êtes pas autorisé à utiliser ce service.");
                }

            }
            return BadRequest("Vous êtes déconnecté. Veuillez vous authentifier svp."); // Retourner une réponse d'erreur d'authentification
        }*/

        [HttpGet("GetUsers")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            if (IsUserAuthorized("AdminPolicy"))
            {
                var users = await _service.GetUsersAsync();
                return Ok(users);

            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpGet("getUser/{id}")]
        /*public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            // Récupérer le jeton d'authentification de l'en-tête de la requête
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Valider le jeton
            bool isValidToken = _tokenAuthService.ValidateToken(token);
            if (isValidToken)
            {
                // Récupérer l'identité de l'utilisateur à partir du jeton
                ClaimsPrincipal user = _tokenAuthService.GetPrincipalFromToken(token);

                // Récupérer le rôle de l'utilisateur à partir de la revendication
                string userRole = user.FindFirst("RoleName")?.Value;

                // Vérifier si l'utilisateur a le rôle requis
                if (userRole == "admin")
                {
                    var user_recherche = await _service.GetUserAsync(id);

                    if (user_recherche == null)
                    {
                        return NotFound();
                    }

                    return Ok(user_recherche);
                }
                else if (userRole == null)
                {
                    return BadRequest("l'utilisateur n'a pas encore de rôle.");
                }
                else
                {
                    return BadRequest("Vous n'êtes pas autorisé à utiliser ce service.");
                }
            }
            else {
                return BadRequest("Vous êtes invités à s'authentifier svp.");
            }
        }*/
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            if (IsUserAuthorized("AdminPolicy"))
            {
                var user_recherche = await _service.GetUserAsync(id);

                if (user_recherche == null)
                {
                    return NotFound();
                }

                return Ok(user_recherche);
            }
            else if (IsUserAuthorized("UserPolicy"))
            {
                string currentUserId = _tokenAuthService.GetCurrentUserId(); // Méthode pour récupérer l'ID de l'utilisateur courant
                var user_recherche = await _service.GetUserAsync(id);
                if (user_recherche == null)
                {
                    return NotFound();
                }
                else if (user_recherche.Id.ToString() == currentUserId)
                {
                    return Ok(user_recherche);
                }
                else
                {
                    return Unauthorized();
                }

            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("addUser")]
        public async Task<ActionResult<int>> CreateUser([FromBody] UserCreateDTO userCreateDTO)
        {
            try
            {
                var id = await _service.CreateUserAsync(userCreateDTO);
                return Ok(id);
            }
            catch (Exception ex)
            {
                // Log the exception and return a proper error response
                return BadRequest("Error deserializing JSON: " + ex.Message);
            }
        }

        [HttpPut("updateUser/{id}")]
        /*
        public async Task<ActionResult> UpdateUser(int id, UserUpdateDTO userUpdateDTO)
        {
            // Récupérer le jeton d'authentification de l'en-tête de la requête
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Valider le jeton
            bool isValidToken = _tokenAuthService.ValidateToken(token);
            if (isValidToken)
            {
                // Récupérer l'identité de l'utilisateur à partir du jeton
                ClaimsPrincipal authentified_user = _tokenAuthService.GetPrincipalFromToken(token);

                // Récupérer le rôle de l'utilisateur à partir de la revendication
                string? userRole = authentified_user.FindFirst("RoleName")?.Value;

                // Vérifier si l'utilisateur a le rôle requis
                if (userRole == "admin")
                {
                    try
                    {
                        await _service.UpdateUserAsync(id, userUpdateDTO);
                        return Ok("The user is updated successfully.");
                    }
                    catch (ArgumentException ex)
                    {
                        return NotFound(ex.Message);
                    }
                }
                else if (userRole == null)
                {
                    return BadRequest("l'utilisateur n'a pas encore de rôle.");
                }
                else
                {
                    return BadRequest("Vous n'êtes pas autorisé à utiliser ce service.");
                }
            }
            else
            {
                return BadRequest("Vous êtes invités à s'authentifier svp.");
            }
        }
        */
        public async Task<ActionResult> UpdateUser(int id, UserUpdateDTO userUpdateDTO)
        {
            if (IsUserAuthorized("AdminPolicy"))
            {
                var user_recherche = await _service.GetUserAsync(id);

                if (user_recherche == null)
                {
                    return NotFound();
                }


                try
                {
                    await _service.UpdateUserAsync(id, userUpdateDTO);
                    return Ok("The user is updated successfully.");
                }
                catch (ArgumentException ex)
                {
                    return NotFound(ex.Message);
                }

            }
            else if (IsUserAuthorized("UserPolicy"))
            {
                string currentUserId = _tokenAuthService.GetCurrentUserId(); // Méthode pour récupérer l'ID de l'utilisateur courant
                var user_recherche = await _service.GetUserAsync(id);
                if (user_recherche == null)
                {
                    return NotFound();
                }
                else if (user_recherche.Id.ToString() == currentUserId)
                {
                    try
                    {
                        await _service.UpdateUserAsync(id, userUpdateDTO);
                        return Ok("The user is updated successfully.");
                    }
                    catch (ArgumentException ex)
                    {
                        return NotFound(ex.Message);
                    }
                }
                else
                {
                    return Unauthorized();
                }

            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut("ModifierMotDePasse/{id}")]
        public async Task<ActionResult<string>> ModifierMotDePasse(int id)
        {
            (bool succes, string message) resultat = await _service.generatePasswordAsync(id);
            if (resultat.succes)
            {
                return Ok(resultat.message);
            }
            else
            {
                return BadRequest(resultat.message);
            }
        }

        [HttpPut("updatePasswordUser/{id}")]
        public async Task<ActionResult> UpdatePasswordUser(int id, UserUpdateMPDto userUpdateMPDto, string? ancienMP)
        {
            // Récupérer le jeton d'authentification de l'en-tête de la requête
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Valider le jeton
            bool isValidToken = _tokenAuthService.ValidateToken(token);
            if (isValidToken)
            {
                try
                {
                    (bool succes, string message) resultat = await _service.UpdatePasswordUserAsync(id, userUpdateMPDto, ancienMP);
                    if (resultat.succes == true)
                        return Ok(resultat.message);
                    else
                        return BadRequest(resultat.message);
                }
                catch (ArgumentException ex)
                {
                    return NotFound(ex.Message);
                }
            }
            else
            {
                return BadRequest("Vous êtes invités à s'authentifier svp.");
            }
        }


        [HttpDelete("deleteUser/{id}")]
        /*public async Task<ActionResult> DeleteUser(int id)
        {
            // Récupérer le jeton d'authentification de l'en-tête de la requête
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Valider le jeton
            bool isValidToken = _tokenAuthService.ValidateToken(token);
            if (isValidToken)
            {
                // Récupérer l'identité de l'utilisateur à partir du jeton
                ClaimsPrincipal authentified_user = _tokenAuthService.GetPrincipalFromToken(token);

                // Récupérer le rôle de l'utilisateur à partir de la revendication
                string? userRole = authentified_user.FindFirst("RoleName")?.Value;

                // Vérifier si l'utilisateur a le rôle requis
                if (userRole == "admin")
                {
                    try
                    {
                        await _service.DeleteUserAsync(id);
                        return Ok();
                    }
                    catch (ArgumentException ex)
                    {
                        return NotFound(ex.Message);
                    }
                }
                else if (userRole == null)
                {
                    return BadRequest("l'utilisateur n'a pas encore de rôle.");
                }
                else
                {
                    return BadRequest("Vous n'êtes pas autorisé à utiliser ce service.");
                }
            }
            else
            {
                return BadRequest("Vous êtes invités à s'authentifier svp.");
            }
        }*/
        public async Task<ActionResult> DeleteUser(int id)
        {
            if (IsUserAuthorized("AdminPolicy"))
            {
                try
                {
                    await _service.DeleteUserAsync(id);
                    return Ok();
                }
                catch (ArgumentException ex)
                {
                    return NotFound(ex.Message);
                }

            }
            else
            {
                return Unauthorized();
            }

        }
    }
}
