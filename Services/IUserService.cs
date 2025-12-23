using tech_software_engineer_consultant_int_backend.DTO.UsersDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IUserService
    {
        /*Task<IEnumerable<UserDto>> GetUsersAsync();*/
        Task<List<UserDto>> GetUsersAsync();
        Task<UserDto> GetUserAsync(int id);
        Task<UserDto?> GetUserByRef(string UserRef);
        Task<UserListeUPAsDTO?> GetUserListPortefeuilleAssociations(string UserRef);
        /*Task<int> CreateUserAsync(UserCreateDTO userCreateDTO);*/
        Task<bool> CreateUserAsync(UserCreateDTO userCreateDTO);
        Task<(bool, string)> generatePasswordAsync(int id);
        Task<User> UpdateUserAsync(int id, UserUpdateDTO userUpdateDTO);
        Task<(bool success, string message)> UpdateListePortefeuilleAssociationsUserAsync(int id, UserUpdatePortefeuillesAssociationsDTO userUpdatePortefeuillesAssociationsDTO);
        Task<(bool success, string message)> UpdatePasswordUserAsync(int id, UserUpdateMPDto userUpdateMPDto, string confirmedMP);
        Task<bool> DeleteUserAsync(int id);
        /*string GenerateNextRef();*/
        Task<string> GenerateNextRef();
    }
}
