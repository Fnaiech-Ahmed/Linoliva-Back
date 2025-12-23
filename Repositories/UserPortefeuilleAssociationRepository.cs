using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class UserPortefeuilleAssociationRepository:IUserPortefeuilleAssociationRepository<UserPortefeuilleAssociation>
    {
        private readonly MyDbContext _dbContext;

        public UserPortefeuilleAssociationRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(bool, UserPortefeuilleAssociation)> AddAsync(UserPortefeuilleAssociation entity)
        {
            try
            {
                EntityEntry<UserPortefeuilleAssociation> entityEntry = await _dbContext.UserPortefeuilleAssociations.AddAsync(entity);
                UserPortefeuilleAssociation addedEntity = entityEntry.Entity;

                int numRowsAffected = await _dbContext.SaveChangesAsync();

                if (numRowsAffected > 0 && addedEntity.Id > 0)
                    return (true, addedEntity);
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


        public async Task<bool> ValidateExistingUserPortefeuilleAssociation(int UserID, int PortefeuilleID)
        {
            try
            {
                UserPortefeuilleAssociation? existingUserPortefeuilleAssociation = await _dbContext.UserPortefeuilleAssociations.FirstOrDefaultAsync(u => u.UserId == UserID && u.PortefeuilleId == PortefeuilleID);
                if (existingUserPortefeuilleAssociation != null)    
                    return (true);
                else
                    return false;
            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
                return (false);
            }

        }

        public async Task<UserPortefeuilleAssociation?> FindByIDsAsync(int UserID, int PortefeuilleID)
        {
            try
            {
                UserPortefeuilleAssociation? existingUserPortefeuilleAssociation = await _dbContext.UserPortefeuilleAssociations.FirstOrDefaultAsync(u => u.UserId == UserID && u.PortefeuilleId == PortefeuilleID);
                if (existingUserPortefeuilleAssociation != null)
                    return existingUserPortefeuilleAssociation;
                else
                    return null;
            }
            catch (Exception ex)
            {
                // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
                // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
                return (null);
            }

        }
    }
}
