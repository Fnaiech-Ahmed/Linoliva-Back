using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface IFactureRepository
    {
        Task<bool> AddFacture(Facture facture);


        Task<Facture?> GetFactureById(int factureId);
        Task<Facture?> GetFactureByReference(string reference);
        Task<List<Facture>> GetAllFactures();
        Task<List<Facture>> GetListeFacturesByOwner(string OwnerId);


        Task<bool> UpdateFacture(Facture facture);


        Task<bool> DeleteFacture(int factureId);
    }
}
