using tech_software_engineer_consultant_int_backend.DTO.RoleDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetRolesAsync();
        Task<RoleDto> GetRoleAsync(int id);
        Task<int> CreateRoleAsync(RoleCreateDto roleCreateDto);
        Task UpdateRoleAsync(int id, RoleUpdateDto roleUpdateDto);
        Task DeleteRoleAsync(int id);


        Task AssignRoleToUser(int userId, int roleId);
        Task UnassignRoleFromUser(int userId, int roleId);

        
        Task<List<string>> GetAllowedRolesForPolicy(string policyName);
        Task<List<string>> GetPoliciesForRoleAsync(int roleId);



        Task AssignRoleToPolicy(int roleId, int policyId);
        Task UnassignRoleFromPolicy(int roleId, int policyId);
    }
}
