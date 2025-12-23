using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using tech_software_engineer_consultant_int_backend.DTO;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class EcheanceService : IEcheanceService
    {
        private readonly IEcheanceRepository _echeanceRepository;
        private readonly IMapper _mapper;

        public EcheanceService(IEcheanceRepository echeanceRepository, IMapper mapper)
        {
            _echeanceRepository = echeanceRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EcheanceDto>> GetAllAsync()
        {
            var echeances = await _echeanceRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EcheanceDto>>(echeances);
        }

        public async Task<EcheanceDto> GetByIdAsync(int id)
        {
            var echeance = await _echeanceRepository.GetByIdAsync(id);
            return _mapper.Map<EcheanceDto>(echeance);
        }

        public async Task<bool> AddAsync(CreateEcheanceDto createEcheanceDto)
        {
            var echeance = _mapper.Map<Echeance>(createEcheanceDto);
            return await _echeanceRepository.AddAsync(echeance);
        }

        public async Task<bool> UpdateAsync(int id, UpdateEcheanceDto updateEcheanceDto)
        {
            var echeance = await _echeanceRepository.GetByIdAsync(id);
            if (echeance == null) return false;           

            _mapper.Map(updateEcheanceDto, echeance);
            Echeance? updatedEcheance = await _echeanceRepository.UpdateAsync(echeance);

            if (updatedEcheance == null || updatedEcheance.Id.ToString().IsNullOrEmpty() ) return false;

            return true;
        }

        public async Task DeleteAsync(int id)
        {
            await _echeanceRepository.DeleteAsync(id);
        }
    }
}
