using System.Collections.Generic;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _userRoleRepository;

    public UserRoleService(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesAsync()
    {
        return await _userRoleRepository.GetAll();
    }

    /*public async Task<UserRole> GetUserRoleAsync(int id)
    {
        return await _userRoleRepository.GetById(id);
    }*/

    public async Task AddUserRoleAsync(UserRole userRole)
    {
        await _userRoleRepository.Add(userRole);
    }

    /*public async Task UpdateUserRoleAsync(int id, UserRole userRole)
    {
        var existingUserRole = await _userRoleRepository.GetById(id);
        if (existingUserRole == null)
        {
            throw new ArgumentException("UserRole not found.");
        }

        existingUserRole.UserId = userRole.UserId;
        existingUserRole.RoleId = userRole.RoleId;

        await _userRoleRepository.Update(existingUserRole);
    }*/

    /*public async Task DeleteUserRoleAsync(int id)
    {
        var existingUserRole = await _userRoleRepository.GetById(id);
        if (existingUserRole == null)
        {
            throw new ArgumentException("UserRole not found.");
        }

        await _userRoleRepository.Delete(existingUserRole);
    }*/
}
