using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class RoleRepository : IRoleRepository<Role>
    {
        private readonly MyDbContext _context;

        public RoleRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAll()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> GetById(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<Role> GetByName(string name)
        {
            return await _context.Roles
                                 .FirstOrDefaultAsync(r => r.Name == name);
        }

        /*public async Task<IEnumerable<string>> GetAllowedRolesForPolicy(string policyName)
        {
            var roles = await _context.RolePolicies
                                      .Where(rp => rp.PolicyName == policyName)
                                      .Join(
                                          _context.RolePolicyRoles,
                                          rp => rp.Id,
                                          rpr => rpr.RolePolicyId,
                                          (rp, rpr) => rpr.RoleId
                                      )
                                      .Join(
                                          _context.Roles,
                                          rprRoleId => rprRoleId,
                                          r => r.Id,
                                          (rprRoleId, r) => r.Name
                                      )
                                      .Distinct()
                                      .ToListAsync();

            return roles;
        }*/




        public async Task Add(Role entity)
        {
            _context.Roles.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Role entity)
        {
            _context.Roles.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Role entity)
        {
            _context.Roles.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

}
