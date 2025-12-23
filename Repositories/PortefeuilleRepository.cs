using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class PortefeuilleRepository: IPortefeuilleRepository<Portefeuille>
    {
        private readonly MyDbContext _dbContext;

        public PortefeuilleRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Portefeuille? GetById(int id)
        {
            return _dbContext.Portefeuilles.Find(id);
        }


        public async Task<(bool,Portefeuille)> AddAsync(Portefeuille entity)
        {
            try
            {
                EntityEntry<Portefeuille> entityEntry = await _dbContext.Portefeuilles.AddAsync(entity);
                Portefeuille addedEntity = entityEntry.Entity;

                int numRowsAffected = await _dbContext.SaveChangesAsync();

                bool validateAdd = numRowsAffected > 0 && addedEntity.Id > 0;
                if (validateAdd)
                {
                    return (validateAdd, addedEntity);
                }
                else
                    return (false, null);
                
            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
                return (false, null);
            }
        }


        public async Task<bool> Update(Portefeuille entity)
        {
            try
            {
                EntityEntry<Portefeuille> entityEntry = _dbContext.Portefeuilles.Update(entity);
                Portefeuille addedEntity = entityEntry.Entity;
                int numRowsAffected = await _dbContext.SaveChangesAsync();

                return numRowsAffected > 0 && addedEntity.Id > 0;
            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
                return false;
            }

        }

        public void Delete(Portefeuille entity)
        {
            _dbContext.Portefeuilles.Remove(entity);
        }

        public IEnumerable<Portefeuille> GetAll()
        {
            return _dbContext.Portefeuilles.ToList();
        }
    }
}
