using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class RolePolicyRolesService : IRolePolicyRolesService
    {
        private readonly IRolePolicyRolesRepository _rolePolicyRolesRepository;

        public RolePolicyRolesService(IRolePolicyRolesRepository rolePolicyRolesRepository)
        {
            _rolePolicyRolesRepository = rolePolicyRolesRepository;
        }

        public async Task<IEnumerable<RolePolicyRole>> GetAllRolePolicyRolesAsync()
        {
            return await _rolePolicyRolesRepository.GetRolePolicyRolesAsync();
        }

        /*public async Task<RolePolicyRole> GetRolePolicyRoleByIdAsync(int id)
        {
            return await _rolePolicyRolesRepository.GetByIdAsync(id);
        }*/

        public async Task<RolePolicyRole> CreateRolePolicyRoleAsync(RolePolicyRole rolePolicyRole)
        {
            await _rolePolicyRolesRepository.AddRolePolicyRoleAsync(rolePolicyRole);
            return rolePolicyRole;
        }

        /*public async Task UpdateRolePolicyRoleAsync(RolePolicyRole rolePolicyRole)
        {
            await _rolePolicyRolesRepository.UpdateAsync(rolePolicyRole);
        }

        public async Task DeleteRolePolicyRoleAsync(int id)
        {
            var rolePolicyRole = await _rolePolicyRolesRepository.GetByIdAsync(id);
            if (rolePolicyRole != null)
            {
                await _rolePolicyRolesRepository.DeleteAsync(rolePolicyRole);
            }
        }*/
    }

}
