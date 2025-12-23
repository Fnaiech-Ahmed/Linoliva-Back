using tech_software_engineer_consultant_int_backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserRoleService
{
    Task<IEnumerable<UserRole>> GetUserRolesAsync();
    //Task<UserRole> GetUserRoleAsync(int id);
    Task AddUserRoleAsync(UserRole userRole);
    //Task UpdateUserRoleAsync(int id, UserRole userRole);
    //Task DeleteUserRoleAsync(int id);
}
