using System.Collections.Generic;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

public interface IRolePolicyRolesRepository
{
    Task<IEnumerable<RolePolicyRole>> GetRolePolicyRolesAsync();

    //Task<RolePolicyRole> GetRolePolicyRoleByIdAsync(int id);

    Task AddRolePolicyRoleAsync(RolePolicyRole rolePolicyRole);
    Task Add(RolePolicyRole rolePolicyRole);

    Task UpdateRolePolicyRoleAsync(RolePolicyRole rolePolicyRole);

    

    Task<RolePolicyRole> GetByRoleIdAndPolicyId(int roleId, int policyId);
    
    Task Delete(RolePolicyRole rolePolicyRole);

    Task<RolePolicy> GetRolePolicyById(int policyId);
}
