using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.Models;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly MyDbContext _dbContext;

    public UserRoleRepository(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserRole>> GetAll()
    {
        return await _dbContext.UserRoles.ToListAsync();
    }

    /*public async Task<UserRole> GetById(int id)
    {
        return await _dbContext.UserRoles.FindAsync(id);
    }*/

    public async Task<UserRole> GetByUserIdAndRoleId(int userId, int roleId)
    {
        return await _dbContext.UserRoles
                             .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }


    public async Task Add(UserRole userRole)
    {
        await _dbContext.UserRoles.AddAsync(userRole);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(UserRole userRole)
    {
        _dbContext.UserRoles.Update(userRole);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(UserRole userRole)
    {
        _dbContext.UserRoles.Remove(userRole);
        await _dbContext.SaveChangesAsync();
    }
}
