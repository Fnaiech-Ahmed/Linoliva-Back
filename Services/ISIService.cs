using tech_software_engineer_consultant_int_backend.DTO.SIsDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface ISIService
    {
        Task<bool> AddSI(SICreateDTO siCreateDTO);
        Task<List<SIDTO>> GetSIs();
        Task<SIDTO?> GetSIById(int id);
        
        Task<bool> UpdateSI(int SIId, SIUpdateDTO siUpdateDTO);
        Task<bool> DeleteSI(int id);

    }
}
