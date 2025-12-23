using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SignalRWebpack.Hubs;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class GroupService : IGroupService
    {
        private readonly MyDbContext _dbContext;
        private readonly IHubContext<ChatHub> _hubContext;

        public GroupService(IHubContext<ChatHub> hubContext, MyDbContext dbContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        public async Task<(bool, string, int)> AddGroupAsync(string connectionId, string groupName)
        {
            var existingGroup = await GetGroupByName(groupName);
            if (existingGroup != null)
            {
                return (false, "Le groupe existe déjà.", 0);
            }

            var newGroup = new Group
            {
                Name = groupName
            };

            _dbContext.Groups.Add(newGroup);
            await _dbContext.SaveChangesAsync();            

            if (!connectionId.IsNullOrEmpty() && int.TryParse(connectionId, out int userIdInt))
            {
                bool resultatAddUserToDBGroup = await AddUserToGroup(newGroup.Id, userIdInt);
                if (resultatAddUserToDBGroup)
                {
                    await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);
                    await _hubContext.Clients.Client(connectionId).SendAsync("GroupAdded", newGroup);

                    return (true, "Le groupe " + newGroup.Name + " a été ajouté avec succès.", newGroup.Id);
                }
                else
                {
                    await DeleteGroup(newGroup.Id);
                    return (false, "L'utilisateur n' a pas été ajouté au groupe. Veuillez réessayer.", 0);
                }
            }
            else
            {
                await DeleteGroup(newGroup.Id);
                return (false, "Problème d'identification de l'utilisateur. Veuillez réessayer.", 0);
            }
        }

        public async Task<bool> AddUserToGroup(int groupId, int userId)
        {
            var group = await _dbContext.Groups.FindAsync(groupId);
            var user = await _dbContext.Users.FindAsync(userId);

            if (group != null && user != null)
            {
                group.UserGroupAssociations ??= new List<UserGroupAssociation>();
                group.UserGroupAssociations.Add(new UserGroupAssociation { GroupId = groupId, UserId = userId });
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> IsUserInGroup(int userIdInt, string groupName) 
        {
            Group? existingGroup = await _dbContext.Groups.FindAsync(groupName);
            User? existingUser = await _dbContext.Users.FindAsync(userIdInt);

            if (existingGroup != null && existingUser != null)
            {
                bool isUserInGroup = await _dbContext.UserGroupAssociations
                    .AnyAsync(uga => uga.Group.Name.Equals(groupName) && uga.UserId == userIdInt);

                if (isUserInGroup)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            return false;
        }

        public async Task<bool> IsUserInGroupByIds(int userIdInt, int groupId)
        {
            Group? existingGroup = await _dbContext.Groups.FindAsync(groupId);
            User? existingUser = await _dbContext.Users.FindAsync(userIdInt);

            if (existingGroup != null && existingUser != null)
            {
                bool isUserInGroup = await _dbContext.UserGroupAssociations
                    .AnyAsync(uga => uga.Group.Id == groupId && uga.UserId == userIdInt);

                if (isUserInGroup)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            return false;
        }

        public async Task<Group> GetGroupByName(string groupName)
        {
            // Ajoutez la logique pour trouver un groupe dans la base de données
            return await _dbContext.Groups.FirstOrDefaultAsync(g => g.Name == groupName);
        }

        public async Task<Group> GetGroupById(int groupID)
        {
            // Ajoutez la logique pour trouver un groupe dans la base de données
            return await _dbContext.Groups.FindAsync(groupID);
        }

        public async Task<bool> GroupExists(int groupId)
        {
            Group? existingGroup = await _dbContext.Groups.FindAsync(groupId);
            if (existingGroup != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task RemoveUserFromGroup(int groupId, int userId)
        {
            var association = await _dbContext.UserGroupAssociations.FirstOrDefaultAsync(uga => uga.GroupId == groupId && uga.UserId == userId);

            if (association != null)
            {
                _dbContext.UserGroupAssociations.Remove(association);
                await _dbContext.SaveChangesAsync();
            }
        }


        public async Task DeleteGroup(int groupId)
        {
            // Ajoutez la logique pour supprimer un groupe de la base de données
            var group = await _dbContext.Groups.FindAsync(groupId);

            _dbContext.Groups.Remove(group);
            await _dbContext.SaveChangesAsync();
        }
    }
}