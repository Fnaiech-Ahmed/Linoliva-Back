using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IRolePolicyRolesService
    {
        Task<IEnumerable<RolePolicyRole>> GetAllRolePolicyRolesAsync();
        //Task<RolePolicyRole> GetRolePolicyRoleByIdAsync(int id);
        Task<RolePolicyRole> CreateRolePolicyRoleAsync(RolePolicyRole rolePolicyRole);
        //Task UpdateRolePolicyRoleAsync(RolePolicyRole rolePolicyRole);
        //Task DeleteRolePolicyRoleAsync(int id);
    }
}
