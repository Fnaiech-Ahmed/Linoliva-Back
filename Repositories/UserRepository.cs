using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class UserRepository : IUserRepository<User>
{
    private readonly MyDbContext _dbContext;

    public UserRepository(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<User?> GetById(int id)
    {
        User? existingUser = await _dbContext.Users.FindAsync(id);
        return existingUser;
    }


    public async Task<User?> FindUserByReference(string reference)
    {
        try
        {
            User? user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Ref == reference);

            return user;
        }
        catch (Exception ex)
        {
            // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
            // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
            return null;
        }

    }


    public async Task<bool> Add(User entity)
    {
        try
        {
            EntityEntry <User> entityEntry = await _dbContext.Users.AddAsync(entity);
            User addedEntity = entityEntry.Entity;

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

    public async Task<User> Update(User entity)
    {
        try
        {
            EntityEntry<User> entityEntry = _dbContext.Users.Update(entity);
            User updatedEntity = entityEntry.Entity;

            int numRowsAffected = await _dbContext.SaveChangesAsync();

            if (numRowsAffected > 0)
            {
                User? existingUser = await _dbContext.Users.FindAsync(entity.Id);
                return existingUser;
                //return true;
            }
            else
            {
                return null;
                //return false;
            }
            
        }
        catch (Exception ex)
        {
            // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
            // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
            return null;
            //return true;
        }
    }


    public async Task<(bool,User)> Affect_UserPortefeuilleAssociation_To_User(User entity)
    {
        try
        {
            EntityEntry<User> entityEntry = _dbContext.Users.Update(entity);
            User updatedEntity = entityEntry.Entity;

            int numRowsAffected = await _dbContext.SaveChangesAsync();

            if (numRowsAffected > 0)
            {
                User? existingUser = await _dbContext.Users.FindAsync(entity.Id);
                return (true,existingUser);
            }
            else
            {
                return (false,null);
            }

        }
        catch (Exception ex)
        {
            // Gérer l'exception, journaliser les détails de l'erreur, effectuer un rollback si nécessaire, etc.
            // Vous pourriez également lancer une exception personnalisée si cela a du sens dans le contexte de votre application.
            return (false,null);
        }
    }

    public async Task<bool> Delete(User entity)
    {
        try
        {
            User? existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == entity.Id);

            if (existingUser == null)
            {
                return false; // L'utilisateur n'existe pas
            }

            _dbContext.Users.Remove(existingUser);
            await _dbContext.SaveChangesAsync();

            return true; // L'utilisateur a été supprimé avec succès
        }
        catch (Exception ex)
        {
            // Gérer spécifiquement les exceptions, journaliser les détails, etc.
            // Vous pourriez également lancer une exception personnalisée si nécessaire.
            return false;
        }
    }


    public async Task<List<User>?> GetUsersByReferencePrefix(string referencePrefix)
    {
        List<User>? users = await _dbContext.Users
            .Where(u => u.Ref.StartsWith(referencePrefix))
            .ToListAsync();

        return users;
    }


    /*public IEnumerable<User> GetAll()
    {
        return _dbContext.Users.ToList();
    }*/

    public async Task<List<User>> GetAll()
    {
        return await _dbContext.Users.ToListAsync();
    }
}
