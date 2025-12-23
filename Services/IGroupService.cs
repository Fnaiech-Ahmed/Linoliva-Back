using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IGroupService
    {
        Task<(bool, string, int)> AddGroupAsync(string id, string group);
        Task<Boolean> AddUserToGroup(int groupId, int userId);
        Task<Group> GetGroupByName(string groupName);
        Task<Group> GetGroupById(int groupID);
        Task<bool> IsUserInGroup(int userIdInt, string groupName);
        Task<bool> IsUserInGroupByIds(int userIdInt, int groupId);
        Task<bool> GroupExists(int groupId);
        Task RemoveUserFromGroup(int groupId, int userId);
        Task DeleteGroup(int groupId);
    }
}