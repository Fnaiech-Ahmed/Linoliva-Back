using tech_software_engineer_consultant_int_backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserRoleRepository
{
    Task<IEnumerable<UserRole>> GetAll();

    Task<UserRole> GetByUserIdAndRoleId(int userId, int roleId);
    Task Add(UserRole userRole);
    Task Update(UserRole userRole);
    Task Delete(UserRole userRole);
}
