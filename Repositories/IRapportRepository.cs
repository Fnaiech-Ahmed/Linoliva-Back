namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface IRapportRepository<T>
    {
        T GetById(int id);
        //T GetByRef(string refT);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        IEnumerable<T> GetAll();

    }
}
