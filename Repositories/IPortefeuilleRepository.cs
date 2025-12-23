namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface IPortefeuilleRepository<T>
    {
        T? GetById(int id);
        //T GetByRef(string refT);
        Task<(bool,T entity)> AddAsync(T entity);
        Task<bool> Update(T entity);
        void Delete(T entity);
        IEnumerable<T> GetAll();

    }
}
