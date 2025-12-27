using System.Collections.Generic;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface IInventaireProduitRepository
    {
        Task<List<InventaireProduit>> GetInventaireProduits();
        Task<InventaireProduit> GetInventaireProduitById(int inventaireProduitId);
        Task<InventaireProduit> GetInventaireProduitByProductId(int productId);
        Task<bool> AddInventaireProduit(InventaireProduit inventaireProduit);
        Task<bool> UpdateInventaireProduit(InventaireProduit inventaireProduit);
        Task<bool> UpdateQuantite(int productId, int newQuantite);
        Task<bool> DeleteInventaireProduit(int inventaireProduitId);
        Task<bool> UpdateInventaireProduitQuantity(int productId, string newProductName, int newQuantity);
    }
}
