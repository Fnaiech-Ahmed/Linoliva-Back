using tech_software_engineer_consultant_int_backend.DTO.RoleDTOs;
using tech_software_engineer_consultant_int_backend.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository<Role> roleRepository;
        private readonly IUserRepository<User> userRepository;
        private readonly IRolePolicyRolesRepository rolePolicyRolesRepository;
        private readonly IUserRoleRepository userRoleRepository;

        public RoleService(IRoleRepository<Role> _roleRepository, IUserRepository<User> _userRepository, IRolePolicyRolesRepository rolePolicyRolesRepository, IUserRoleRepository _userRoleRepository)
        {
            roleRepository = _roleRepository;
            userRepository = _userRepository;
            this.rolePolicyRolesRepository = rolePolicyRolesRepository;
            this.userRoleRepository = _userRoleRepository;
        }

        public async Task<IEnumerable<RoleDto>> GetRolesAsync()
        {
            var roles = await roleRepository.GetAll();
            return roles.Select(role => new RoleDto(role)).ToList();
        }


        public async Task<RoleDto> GetRoleAsync(int id)
        {
            var role = await roleRepository.GetById(id);
            if (role == null) throw new ArgumentException("Role not found.");
            return new RoleDto(role);
        }


        public async Task<int> CreateRoleAsync(RoleCreateDto roleCreateDto)
        {
            var role = roleCreateDto.ToRoleEntity();
            await roleRepository.Add(role);
            return role.Id;
        }


        public async Task UpdateRoleAsync(int id, RoleUpdateDto roleUpdateDto)
        {
            var existingRole = await roleRepository.GetById(id);
            if (existingRole == null) throw new ArgumentException("Role not found.");

            var updatedRole = roleUpdateDto.ToRoleEntity();
            existingRole.Name = updatedRole.Name ?? existingRole.Name;
            existingRole.Description = updatedRole.Description ?? existingRole.Description;

            await roleRepository.Update(existingRole);
        }


        public async Task DeleteRoleAsync(int id)
        {
            var role = await roleRepository.GetById(id);
            if (role == null) throw new ArgumentException("Role not found.");
            await roleRepository.Delete(role);
        }


        public async Task AssignRoleToUser(int userId, int roleId)
        {
            var user = await userRepository.GetById(userId);
            var role = await roleRepository.GetById(roleId);

            if (user == null || role == null) throw new ArgumentException("User or Role not found.");

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };

            await userRoleRepository.Add(userRole);
        }

        public async Task UnassignRoleFromUser(int userId, int roleId)
        {
            var userRole = await userRoleRepository.GetByUserIdAndRoleId(userId, roleId);

            if (userRole == null) throw new ArgumentException("UserRole not found.");

            await userRoleRepository.Delete(userRole);
        }    


        


        public async Task<List<string>> GetAllowedRolesForPolicy(string policyName)
        {
            var rolePolicyRoles = await rolePolicyRolesRepository.GetRolePolicyRolesAsync();
            var roles = rolePolicyRoles
                .Where(rpr => rpr.RolePolicy.PolicyName == policyName)
                .Select(rpr => rpr.Role.Name)
                .Distinct()
                .ToList();

            if (!roles.Any())
            {
                throw new InvalidOperationException($"At least one role must be specified for policy: {policyName}");
            }

            return roles;
        }


        public async Task<List<string>> GetPoliciesForRoleAsync(int roleId)
        {
            // Récupérer les politiques associées au rôle
            var rolePolicyRoles = await rolePolicyRolesRepository.GetRolePolicyRolesAsync();
            var policies = rolePolicyRoles
                .Where(rpr => rpr.RoleId == roleId)
                .Select(rpr => rpr.RolePolicy.PolicyName)
                .Distinct()
                .ToList();

            if (!policies.Any())
            {
                throw new InvalidOperationException($"No policies found for role with ID: {roleId}");
            }

            return policies;
        }

        public async Task AssignRoleToPolicy(int roleId, int policyId)
        {
            var role = await roleRepository.GetById(roleId);
            var policy = await rolePolicyRolesRepository.GetRolePolicyById(policyId);

            if (role == null || policy == null) throw new ArgumentException("Role or Policy not found.");

            var rolePolicyRole = new RolePolicyRole
            {
                RoleId = roleId,
                RolePolicyId = policyId
            };

            await rolePolicyRolesRepository.Add(rolePolicyRole);
        }

        public async Task UnassignRoleFromPolicy(int roleId, int policyId)
        {
            var rolePolicyRole = await rolePolicyRolesRepository.GetByRoleIdAndPolicyId(roleId, policyId);

            if (rolePolicyRole == null) throw new ArgumentException("RolePolicyRole not found.");

            await rolePolicyRolesRepository.Delete(rolePolicyRole);
        }

    }
}
