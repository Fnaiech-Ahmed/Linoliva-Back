using tech_software_engineer_consultant_int_backend.DTO.InventaireProduitDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IInventaireProduitService
    {
        Task<bool> AddInventaireProduit(InventaireProduitCreateDTO NewInventaireProduitCreateDTO);
        Task<List<InventaireProduitDTO>> GetInventaireProduits();
        Task<InventaireProduitDTO> GetInventaireProduitById(int id);
        Task<(bool, int)> VerifInventaireProduitByProductId(int ProductId);
        Task<bool> UpdateInventaireProduit(int inventaireProduitId, InventaireProduitUpdateDTO inventaireProduitUpdateDTO);
        Task<(bool, int)> ModifierQuantiteProduit(int inventaireProduitId, string NomProduit, int Quantite, TypeTransaction typeTransaction);
        Task<bool> DeleteInventaireProduit(int id);
    }
}
