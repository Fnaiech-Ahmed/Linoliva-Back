using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface IUserPortefeuilleAssociationRepository<T>
    {
        Task<(bool,T)> AddAsync(UserPortefeuilleAssociation entity);
        Task<bool> ValidateExistingUserPortefeuilleAssociation(int UserId, int PortefeuilleId);
        Task<UserPortefeuilleAssociation?> FindByIDsAsync(int UserId, int PortefeuilleId);
    }
}
