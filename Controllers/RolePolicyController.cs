using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Services;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePolicyController : ControllerBase
    {
        private readonly IRolePolicyRolesService _rolePolicyService;
        private readonly ITokenAuthenticationService _tokenAuthService; // Assuming you have this service for token authorization

        public RolePolicyController(IRolePolicyRolesService rolePolicyService, ITokenAuthenticationService tokenAuthService)
        {
            _rolePolicyService = rolePolicyService;
            _tokenAuthService = tokenAuthService;
        }

        // GET: api/RolePolicy/GetRolePolicies
        [HttpGet("GetRolePolicies")]
        public async Task<ActionResult<IEnumerable<RolePolicy>>> GetRolePolicies()
        {
            if (IsUserAuthorized("AdminPolicy"))
            {
                var rolePolicies = await _rolePolicyService.GetAllRolePolicyRolesAsync();
                return Ok(rolePolicies);
            }
            else
            {
                return Unauthorized();
            }
        }

        // GET: api/RolePolicy/GetRolePolicy/{id}
        
        /*[HttpGet("GetRolePolicy/{id}")]
        public async Task<ActionResult<RolePolicy>> GetRolePolicy(int id)
        {
            if (IsUserAuthorized("AdminPolicy", "UserPolicy"))
            {
                var rolePolicy = await _rolePolicyService.GetRolePolicyByIdAsync(id);

                if (rolePolicy == null)
                {
                    return NotFound();
                }

                return Ok(rolePolicy);
            }
            else
            {
                return Unauthorized();
            }
        }

        // POST: api/RolePolicy/CreateRolePolicy
        [HttpPost("CreateRolePolicy")]
        public async Task<ActionResult<RolePolicy>> CreateRolePolicy(RolePolicy rolePolicy)
        {
            if (IsUserAuthorized("AdminPolicy"))
            {
                await _rolePolicyService.CreateRolePolicyAsync(rolePolicy);
                return CreatedAtAction(nameof(GetRolePolicy), new { id = rolePolicy.Id }, rolePolicy);
            }
            else
            {
                return Unauthorized();
            }
        }

        // PUT: api/RolePolicy/UpdateRolePolicy/{id}
        [HttpPut("UpdateRolePolicy/{id}")]
        public async Task<IActionResult> UpdateRolePolicy(int id, RolePolicy rolePolicy)
        {
            if (IsUserAuthorized("AdminPolicy"))
            {
                if (id != rolePolicy.Id)
                {
                    return BadRequest();
                }

                await _rolePolicyService.UpdateRolePolicyAsync(rolePolicy);

                return NoContent();
            }
            else
            {
                return Unauthorized();
            }
        }

        // DELETE: api/RolePolicy/DeleteRolePolicy/{id}
        [HttpDelete("DeleteRolePolicy/{id}")]
        public async Task<IActionResult> DeleteRolePolicy(int id)
        {
            if (IsUserAuthorized("AdminPolicy"))
            {
                var rolePolicy = await _rolePolicyService.GetRolePolicyByIdAsync(id);

                if (rolePolicy == null)
                {
                    return NotFound();
                }

                await _rolePolicyService.DeleteRolePolicyAsync(id);

                return NoContent();
            }
            else
            {
                return Unauthorized();
            }
        }*/

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
    }
}
