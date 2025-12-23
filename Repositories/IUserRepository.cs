using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface IUserRepository<T>
    {
        Task<User?> GetById(int id);
        Task<User?> FindUserByReference(string reference);
        Task<bool> Add(T entity);
        Task<User?> Update(T entity);
        Task<(bool,User)> Affect_UserPortefeuilleAssociation_To_User(User user);
        Task<bool> Delete(T entity);
        /*IEnumerable<T> GetAll();*/


        Task<List<T>> GetAll();
        Task<List<User>?> GetUsersByReferencePrefix(string referencePrefix);
    }
}
