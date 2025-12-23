
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface IRoleRepository<T> where T : Role
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task<T> GetByName(string name);
        /*Task<IEnumerable<string>> GetAllowedRolesForPolicy(string policyName);*/
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }

}
