using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.DTO.RoleDTOs;
using tech_software_engineer_consultant_int_backend.Services;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public RoleController(IRoleService roleService, ITokenAuthenticationService tokenAuthService)
        {
            _roleService = roleService;
            _tokenAuthService = tokenAuthService;
        }

        private bool IsUserAuthorized(string token, string requiredPolicy)
        {
            // Validate the token
            bool isValidToken = _tokenAuthService.ValidateToken(token);
            if (!isValidToken) return false;

            // Retrieve user identity from the token
            ClaimsPrincipal user = _tokenAuthService.GetPrincipalFromToken(token);

            // Check if user has the required policy
            var userPolicies = user.FindAll("policy").Select(p => p.Value);
            return userPolicies.Contains(requiredPolicy);
        }

        // GET: api/role
        /*[HttpGet("get-list-Roles")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "RolePolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter les rôles.");
            }

            var roles = await _roleService.GetRolesAsync();
            return Ok(roles);
        }*/

        [HttpGet("get-list-Roles")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter les rôles.");
            }

            var roles = await _roleService.GetRolesAsync();
            return Ok(roles);
        }


        // GET: api/role/{id}
        [HttpGet("get-Role-By-Id/{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "RolePolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce rôle.");
            }

            try
            {
                var role = await _roleService.GetRoleAsync(id);
                return Ok(role);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/role
        [HttpPost("Create-Role")]
        public async Task<ActionResult<int>> CreateRole([FromBody] RoleCreateDto roleCreateDto)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            //if (!IsUserAuthorized(token, "RoleAdminPolicy"))
            if (!IsUserAuthorized(token, "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à créer des rôles.");
            }

            try
            {
                var roleId = await _roleService.CreateRoleAsync(roleCreateDto);
                return CreatedAtAction(nameof(GetRole), new { id = roleId }, roleId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/role/{id}
        [HttpPut("Update-Role/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleUpdateDto roleUpdateDto)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "RoleAdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à mettre à jour des rôles.");
            }

            try
            {
                await _roleService.UpdateRoleAsync(id, roleUpdateDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/role/{id}
        [HttpDelete("Delete-Role/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            //if (!IsUserAuthorized(token, "RoleAdminPolicy"))
            if (!IsUserAuthorized(token, "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à supprimer des rôles.");
            }

            try
            {
                await _roleService.DeleteRoleAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/role/affectroleto/user/{userId}
        [HttpPost("affectroleto/user/{userId}")]
        /*public async Task<IActionResult> AffectRoleToUser(int userId, [FromBody] int roleId)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "RolesSeniorPolicies"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à affecter des rôles aux utilisateurs.");
            }

            try
            {
                await _roleService.AssignRoleToUser(userId, roleId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }*/
        public async Task<IActionResult> AffectRoleToUser(int userId, [FromBody] int roleId)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (IsUserAuthorized(token,"AdminPolicy"))
            {
                try
                {
                    await _roleService.AssignRoleToUser(userId, roleId);
                    return NoContent();
                }
                catch (ArgumentException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return Unauthorized("Vous n'êtes pas autorisé à affecter des rôles aux utilisateurs.");
            }


        }


        // DELETE: api/role/removerolefrom/user/{userId}
        [HttpDelete("removerolefrom/user/{userId}")]
        public async Task<IActionResult> RemoveRoleFromUser(int userId, [FromBody] int roleId)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "RolesSeniorPolicies"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à retirer les rôles des utilisateurs.");
            }

            try
            {
                await _roleService.UnassignRoleFromUser(userId, roleId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // GET: api/role/allowedrolesforpolicy/{policyName}
        [HttpGet("allowedrolesforpolicy/{policyName}")]
        public async Task<ActionResult<List<string>>> GetAllowedRolesForPolicy(string policyName)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "RolesPolicies"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter les rôles autorisés pour cette politique.");
            }

            try
            {
                var roles = await _roleService.GetAllowedRolesForPolicy(policyName);
                return Ok(roles);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }



        // POST: api/role/assignroleto/policy
        /*[HttpPost("assignroleto/policy")]
        public async Task<IActionResult> AssignRoleToPolicy([FromBody] AssignRoleToPolicyDto assignRoleToPolicyDto)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "RoleAdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à assigner des rôles aux politiques.");
            }

            try
            {
                await _roleService.AssignRoleToPolicy(assignRoleToPolicyDto.RoleId, assignRoleToPolicyDto.PolicyId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }*/

        [HttpPost("assignroleto/policy")]
        public async Task<IActionResult> AssignRoleToPolicy([FromBody] AssignRoleToPolicyDto assignRoleToPolicyDto)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à assigner des rôles aux politiques.");
            }

            try
            {
                await _roleService.AssignRoleToPolicy(assignRoleToPolicyDto.RoleId, assignRoleToPolicyDto.PolicyId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/role/unassignrolefrom/policy
        [HttpDelete("unassignrolefrom/policy")]
        public async Task<IActionResult> UnassignRoleFromPolicy([FromBody] UnassignRoleFromPolicyDto unassignRoleFromPolicyDto)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "RolesSeniorPolicies"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à désassigner des rôles des politiques.");
            }

            try
            {
                await _roleService.UnassignRoleFromPolicy(unassignRoleFromPolicyDto.RoleId, unassignRoleFromPolicyDto.PolicyId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
