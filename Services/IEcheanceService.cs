using tech_software_engineer_consultant_int_backend.DTO;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IEcheanceService
    {
        Task<IEnumerable<EcheanceDto>> GetAllAsync();
        Task<EcheanceDto> GetByIdAsync(int id);
        Task<bool> AddAsync(CreateEcheanceDto createEcheanceDto);
        Task<bool> UpdateAsync(int id, UpdateEcheanceDto updateEcheanceDto);
        Task DeleteAsync(int id);
    }
}
