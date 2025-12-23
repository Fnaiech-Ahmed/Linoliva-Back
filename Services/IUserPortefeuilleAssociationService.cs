
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IUserPortefeuilleAssociationService
    {
        Task<(bool, string, string, Portefeuille)> AddUserPortefeuilleAssociation(string userReference, Portefeuille existingPortefeuille);
        Portefeuille Affect_UserPortefeuilleAssociation_To_Portefeuille(UserPortefeuilleAssociation userPortefeuilleAssociation, Portefeuille _Portefeuille);

    }
}
