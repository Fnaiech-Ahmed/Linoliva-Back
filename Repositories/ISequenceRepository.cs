using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface ISequenceRepository<T>
    {
        Task<T?> GetById(int id);
        //T GetByRef(string refT);
        Sequence GetSequenceByName(string Name);
        Task AddSequence(T entity);
        Task UpdateSequence(T entity);
        Task Delete(T entity);
        IEnumerable<T> GetAll();
    }
}
