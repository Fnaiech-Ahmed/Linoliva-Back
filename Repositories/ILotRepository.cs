using System.Collections.Generic;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public interface ILotRepository
    {
        Task<List<Lot>> GetLots();
        Task<Lot?> GetLotById(int lotId);
        Task<List<Lot>> GetLotsByProductId(int productId);
        Task<int> GetTotalQuantiteByProductId(int productId);
        Task<Lot?> GetLotByReference(string reference);
        Task<int> AddLot(Lot lot);
        Task UpdateLotsBatch(List<Lot> lots);
        Task<bool> UpdateLot(int lotId, Lot lot);
        Task<bool> DeleteLot(int lotId);
    }
}
