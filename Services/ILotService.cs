using tech_software_engineer_consultant_int_backend.DTO.LotsDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface ILotService
    {
        Task<int> AddLot(LotCreateDTO lotCreateDTO);
        Task<List<LotDTO>> GetLots();
        Task<LotDTO?> GetLotById(int id);
        Task<bool> UpdateLot(int lotId, LotUpdateDTO lotUpdateDTO);
        Task<VenteResult> VenteQuantite(int productId, int quantiteSaisie);

        Task<(bool Success, int LotId, string Message)> AchatQuantite(LotCreateDTO lotCreateDTO);
        Task<bool> ReplenishLotsWithOldQuantity(string RefLot, int Quantite);
        Task<bool> DeleteLot(int id);
    }
}
