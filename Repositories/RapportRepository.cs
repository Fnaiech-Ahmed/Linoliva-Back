using tech_software_engineer_consultant_int_backend.Models;
using System;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class RapportRepository:IRapportRepository<Rapport>
    {
        private readonly MyDbContext _dbContext;

        public RapportRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Rapport GetById(int id)
        {
            return _dbContext.Rapports.Find(id);
        }

        public void Add(Rapport entity)
        {
            _dbContext.Rapports.Add(entity);
        }

        public void Update(Rapport entity)
        {
            _dbContext.Rapports.Update(entity);
        }

        public void Delete(Rapport entity)
        {
            _dbContext.Rapports.Remove(entity);
        }

        public IEnumerable<Rapport> GetAll()
        {
            return _dbContext.Rapports.ToList();
        }
    }
}
