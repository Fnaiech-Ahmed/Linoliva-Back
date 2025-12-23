using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

public class RolePolicyRolesRepository : IRolePolicyRolesRepository
{
    private readonly MyDbContext _context;

    public RolePolicyRolesRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RolePolicyRole>> GetRolePolicyRolesAsync()
    {
        return await _context.RolePolicyRoles
                             .Include(rpr => rpr.Role)
                             .Include(rpr => rpr.RolePolicy)
                             .ToListAsync();
    }


    public async Task<RolePolicy> GetRolePolicyById(int policyId)
    {
        return await _context.RolePolicies.FindAsync(policyId);
    }


    public async Task<RolePolicyRole> GetByRoleIdAndPolicyId(int roleId, int policyId)
    {
        return await _context.RolePolicyRoles
                             .FirstOrDefaultAsync(rpr => rpr.RoleId == roleId && rpr.RolePolicyId == policyId);
    }

    public async Task AddRolePolicyRoleAsync(RolePolicyRole rolePolicyRole)
    {
        await _context.RolePolicyRoles.AddAsync(rolePolicyRole);
        await _context.SaveChangesAsync();
    }

    public async Task Add(RolePolicyRole rolePolicyRole)
    {
        _context.RolePolicyRoles.Add(rolePolicyRole);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRolePolicyRoleAsync(RolePolicyRole rolePolicyRole)
    {
        _context.RolePolicyRoles.Update(rolePolicyRole);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(RolePolicyRole rolePolicyRole)
    {
        _context.RolePolicyRoles.Remove(rolePolicyRole);
        await _context.SaveChangesAsync();
    }
}
